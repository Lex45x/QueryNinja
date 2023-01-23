using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using BindingFlags = System.Reflection.BindingFlags;

namespace QueryNinja.Core.Factories
{
    /// <summary>
    /// Default factory that unify creation of all operation-based filter. <br/>
    /// This factory can be configured with user-defined filters with <see cref="RegisterFilterFactory{TOperation}"/>
    /// </summary>
    public class DefaultFilterSerializer : AbstractComponentExtension<IFilter>, IQueryComponentSerializer
    {
        /// <summary>
        /// Create instance of the factory and initialize it with default filters.
        /// </summary>
        public DefaultFilterSerializer()
        {
            RegisterFilterFactory<CollectionOperation>((operation, property, value) =>
                new CollectionFilter(operation, property, value));
            RegisterFilterFactory<ComparisonOperation>((operation, property, value) =>
                new ComparisonFilter(operation, property, value));
        }

        /// <summary>
        /// Operation-specific factory method that can create <see cref="IFilter"/> from parameters.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <typeparam name="TOperation"></typeparam>
        public delegate IFilter FactoryMethod<in TOperation>(TOperation operation, string property, string value)
            where TOperation : struct, Enum;

        /// <summary>
        /// Factory method that can create <see cref="IFilter"/> from parameters without knowing exact <paramref name="operation"/> type.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public delegate IFilter DeserializerMethod(string operation, string property, string value);

        /// <summary>
        /// Corresponds to a serialization method for the given filter.
        /// </summary>
        public delegate KeyValuePair<string, string> SerializerMethod(IFilter filter);

        private readonly Dictionary<string, DeserializerMethod> filterDeserializers = new();
        private readonly Dictionary<Type, SerializerMethod> filterSerializers = new();

        /// <summary>
        /// Allows to register user-defined operation-based filter.
        /// </summary>
        /// <typeparam name="TOperation">Enum that describes operation.</typeparam>
        /// <param name="factory">Factory hat creates <see cref="IFilter"/> from operation, property and value.</param>
        public void RegisterFilterFactory<TOperation>(FactoryMethod<TOperation> factory)
            where TOperation : struct, Enum
        {
            IFilter GeneralizedFactory(string operation, string property, string value)
            {
                var operationEnum = Enum.Parse<TOperation>(operation);

                return factory(operationEnum, property, value);
            }

            foreach (var operation in typeof(TOperation).GetEnumNames())
            {
                filterDeserializers.Add(operation, GeneralizedFactory);
            }
        }

        ///<inheritdoc/>
        public bool CanDeserialize(ReadOnlySpan<char> path, string value)
        {
            if (!path.StartsWith("filter", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var lastDot = path.LastIndexOf(value: '.');

            var operation = path[(lastDot + 1)..].ToString();

            var factoryPresent = filterDeserializers.ContainsKey(operation);

            if (factoryPresent)
            {
                return true;
            }

            //in case we don't have factory, we will attempt to create a factory dynamically.
            var factoryCreated = TryCreateDeserializer(operation);

            return factoryCreated;
        }

        /// <summary>
        /// Search for suitable <see cref="IDefaultFilter{TOperation}"/> in <see cref="QueryNinjaExtensions.KnownQueryComponents"/> and artificially creates factory for it.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns><code>true</code> when factory created and registered.</returns>
        private bool TryCreateDeserializer(string operation)
        {
            // find all implementations of IDefaultFilter
            var knownDefaultFilters = QueryNinjaExtensions.KnownQueryComponents
                .Where(defaultFilter => defaultFilter.GetInterface("IDefaultFilter`1")?.GetGenericArguments()[0]
                    .IsEnumDefined(operation) ?? false)
                .ToList();

            switch (knownDefaultFilters.Count)
            {
                case 0:
                    return false;
                case > 1:
                    //todo: custom exception
                    throw new InvalidOperationException("Multiple filters with same operation name found!");
            }

            var filter = knownDefaultFilters[0];

            var operationType = filter.GetInterface("IDefaultFilter`1")!.GetGenericArguments()[0];

            var genericCreateFactory = GetType()
                .GetMethod("TryCreateDeserializer", genericParameterCount: 1, BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null, new[] { typeof(Type) }, modifiers: null);

            //can be caused only by changing the codebase.
            if (genericCreateFactory == null)
            {
                throw new InvalidOperationException(
                    "Instance member responsible for the next step in factory creation is not found!");
            }

            //TryCreateFactory has to be generic method to simplify subsequent generic method calls.
            return (bool)genericCreateFactory.MakeGenericMethod(operationType).Invoke(this, new object[] { filter })!;
        }

        /// <summary>
        /// Search for suitable constructor in <paramref name="filterType"/>. <br/>
        /// Create <see cref="FactoryMethod{TOperation}"/> and register it via <see cref="RegisterFilterFactory{TOperation}"/>.
        /// </summary>
        /// <typeparam name="TOperation"></typeparam>
        /// <param name="filterType"></param>
        /// <returns><code>true</code> when factory created and registered.</returns>
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used via Reflection")]
        private bool TryCreateDeserializer<TOperation>(Type filterType)
            where TOperation : struct, Enum
        {
            var operationParameter = Expression.Parameter(typeof(TOperation), "operation");
            var propertyParameter = Expression.Parameter(typeof(string), "property");
            var valueParameter = Expression.Parameter(typeof(string), "value");

            var constructor = filterType.GetConstructor(new[] { typeof(TOperation), typeof(string), typeof(string) });

            if (constructor == null)
            {
                return false;
            }

            //this statement implies fixed and pre-defined order of parameters.
            var newFilterExpression =
                Expression.New(constructor, operationParameter, propertyParameter, valueParameter);

            var lambda = Expression.Lambda<FactoryMethod<TOperation>>(newFilterExpression, operationParameter,
                propertyParameter, valueParameter);

            var factoryMethod = lambda.Compile();

            RegisterFilterFactory(factoryMethod);

            return true;
        }

        ///<inheritdoc/>
        public IQueryComponent Deserialize(ReadOnlySpan<char> path, string value)
        {
            var firstDot = path.IndexOf(value: '.');
            var lastDot = path.LastIndexOf(value: '.');

            var operation = path[(lastDot + 1)..].ToString();
            var property = path.Slice(firstDot + 1, lastDot - firstDot - 1).ToString();

            var desiredFactory = filterDeserializers[operation];

            var filter = desiredFactory(operation, property, value);

            return filter;
        }

        public bool CanSerialize(IQueryComponent component)
        {
            if (component is not IFilter filter)
            {
                return false;
            }

            var componentType = component.GetType();
            var filterInterface = componentType.GetInterface("IDefaultFilter`1");

            if (filterInterface == null)
            {
                return false;
            }

            var operationType = filterInterface.GetGenericArguments()[0];

            var serializerPresent = filterSerializers.ContainsKey(operationType);

            if (serializerPresent)
            {
                return true;
            }

            //in case we don't have factory, we will attempt to create a factory dynamically.
            var factoryCreated = TryCreateSerializer(operationType);

            return factoryCreated;
        }

        private bool TryCreateSerializer(Type filterType)
        {
            var genericCreateFactory = GetType()
                .GetMethod("TryCreateSerializer", genericParameterCount: 1, BindingFlags.Instance | BindingFlags.NonPublic,
                    binder: null, Type.EmptyTypes, modifiers: null);

            //can be caused only by changing the codebase.
            if (genericCreateFactory == null)
            {
                throw new InvalidOperationException(
                    "Instance member responsible for the next step in factory creation is not found!");
            }

            var factory = genericCreateFactory.MakeGenericMethod(filterType);

            return (bool)factory.Invoke(this, Array.Empty<object>())!;
        }

        private bool TryCreateSerializer<TOperation>()
            where TOperation : Enum
        {
            KeyValuePair<string,string> GeneralFactory(IFilter filter)
            {
                var defaultFilter = (IDefaultFilter<TOperation>)filter;

                return new KeyValuePair<string, string>($"filter.{defaultFilter.Property}.{defaultFilter.Operation.ToString()}",
                    defaultFilter.Value);
            }

            return filterSerializers.TryAdd(typeof(TOperation), GeneralFactory);
        }

        /// <inheritdoc />
        public KeyValuePair<string, string> Serialize(IQueryComponent component)
        {
            if (component is not IFilter filter)
            {
                throw new InvalidOperationException($"{nameof(component)} is expected to implement IFilter");
            }

            var componentType = component.GetType();
            var filterInterface = componentType.GetInterface("IDefaultFilter`1");
            if (filterInterface == null)
            {
                throw new InvalidOperationException($"{nameof(component)} is expected to implement IDefaultFilter`1");
            }

            var operationType = filterInterface.GetGenericArguments()[0];

            var pair = filterSerializers[operationType](filter);

            return pair;
        }
    }
}