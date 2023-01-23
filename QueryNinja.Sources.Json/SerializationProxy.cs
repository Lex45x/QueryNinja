using Newtonsoft.Json;
using QueryNinja.Core;
using QueryNinja.Core.Projection;

namespace QueryNinja.Sources.Json;

/// <summary>
/// Internal class create to hide all implementation details for IQuery and IDynamic query from serialization. <br/>
/// It allows serializer to use only pre-defined set of fields. Also, <see cref="SerializationProxy"/> defines expected structure of JSON to deserialize.
/// </summary>
internal class SerializationProxy
{
    internal SerializationProxy(IReadOnlyDictionary<string, string> components, IReadOnlyDictionary<string, string>? selector)
    {
        Components = components;
        Selector = selector;
    }

    public IReadOnlyDictionary<string, string> Components { get; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IReadOnlyDictionary<string, string>? Selector { get; }
}