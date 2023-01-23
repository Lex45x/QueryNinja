using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Projection;

namespace QueryNinja.Sources.Json
{
    public class QueryNinjaConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var serializers = QueryNinjaExtensions.Extensions<IQueryComponentSerializer>();

            if (value is not IQuery query)
            {
                return;
            }

            var components = query.GetComponents();

            var componentsRepresentation = new Dictionary<string, string>(components.Count);
            Dictionary<string, string>? selectorsRepresentation = null;

            for (var i = 0; i < components.Count; i++)
            {
                var queryComponent = components[i];
                for (var serializerIndex = 0; serializerIndex < serializers.Count; serializerIndex++)
                {
                    if (!serializers[serializerIndex].CanSerialize(queryComponent))
                    {
                        continue;
                    }

                    var (componentKey, componentValue) = serializers[serializerIndex].Serialize(queryComponent);
                    componentsRepresentation.Add(componentKey, componentValue);
                    break;
                }
            }

            if (query is IDynamicQuery dynamicQuery)
            {
                var selectors = dynamicQuery.GetSelectors();
                selectorsRepresentation = new Dictionary<string, string>(selectors.Count);

                for (int i = 0; i < selectors.Count; i++)
                {
                    var selector = selectors[i];

                    if (selector is RenameSelector renameSelector)
                    {
                        selectorsRepresentation.Add("select", renameSelector.Source);
                    }
                    else
                    {
                        selectorsRepresentation.Add($"select.{selector.Source}", selector.Target);
                    }
                }
            }


            var serializationProxy = new SerializationProxy(componentsRepresentation, selectorsRepresentation);

            serializer.Serialize(writer, serializationProxy);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var deserializers = QueryNinjaExtensions.Extensions<IQueryComponentSerializer>();

            var proxy = serializer.Deserialize<DeserializationProxy>(reader);

            if (proxy?.Components == null)
            {
                return null;
            }

            var components = new List<IQueryComponent>(proxy.Components.Count);

            foreach (var component in proxy.Components.Properties())
            {
                for (var factoryIndex = 0; factoryIndex < deserializers.Count; factoryIndex++)
                {
                    if (component == null)
                    {
                        continue;
                    }

                    var value = component.Value.ToObject<string>()!;
                    var deserializer = deserializers[factoryIndex];
                    if (!deserializer.CanDeserialize(component.Name, value))
                    {
                        continue;
                    }

                    var queryComponent = deserializer.Deserialize(component.Name, value);
                    components.Add(queryComponent);
                }
            }

            if (proxy.Selectors == null)
            {
                return new Query(components);
            }

            var selectors = new List<ISelector>();

            foreach (var selector in proxy.Selectors.Properties())
            {
                var parts = selector.Name.Split('.');

                var value = selector.Value.ToObject<string>()!;
                if (parts.Length == 1)
                {
                    selectors.Add(new Selector(value));
                }
                else
                {
                    selectors.Add(new RenameSelector(parts[1], value));
                }
            }

            return new DynamicQuery(components, selectors);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableTo(typeof(IQuery));
        }
    }
}