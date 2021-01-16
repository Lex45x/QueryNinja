using System;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;

namespace QueryNinja.Sources.AspNetCore.Factory
{
    /// <summary>
    /// Default factory that unify creation of all operation-based filter. <br/>
    /// This factory can be configured with user-defined filters with <see cref="RegisterFilterFactory{TOperation}"/>
    /// </summary>
    public class DefaultFilterFactory : AbstractComponentExtension<IFilter>, IQueryComponentFactory
    {
        /// <summary>
        /// Create instance of the factory and initialize it with default filters.
        /// </summary>
        public DefaultFilterFactory()
        {
            RegisterFilterFactory<CollectionOperation>((operation, property, value) => new CollectionFilter(operation, property, value));
            RegisterFilterFactory<ComparisonOperation>((operation, property, value) => new ComparisonFilter(operation, property, value));
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
        public delegate IFilter FactoryMethod(string operation, string property, string value);

        private readonly Dictionary<Type, FactoryMethod> filterFactories =
            new Dictionary<Type, FactoryMethod>();

        /// <summary>
        /// Allows to register user-defined operation-based filter.
        /// </summary>
        /// <typeparam name="TOperation">Enum that describes operation.</typeparam>
        /// <param name="factory">Factory hat creates <see cref="IFilter"/> from operation, property and value.</param>
        public void RegisterFilterFactory<TOperation>(FactoryMethod<TOperation> factory)
            where TOperation : struct, Enum
        {

            IFilter GeneralFactory(string operation, string property, string value)
            {
                var operationEnum = Enum.Parse<TOperation>(operation);

                return factory(operationEnum, property, value);
            }

            filterFactories.Add(typeof(TOperation), GeneralFactory);
        }

        ///<inheritdoc/>
        public bool CanApply(string name, string value)
        {
            if (!name.StartsWith("filters", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var segments = name.AsSpan();

            var lastDot = segments.LastIndexOf(value: '.');

            var operation = segments.Slice(lastDot + 1).ToString();

            return filterFactories.Keys.Any(operationType => operationType.IsEnumDefined(operation));
        }

        ///<inheritdoc/>
        public IQueryComponent Create(string name, string value)
        {
            var segments = name.AsSpan();

            var firstDot = segments.IndexOf(value: '.');
            var lastDot = segments.LastIndexOf(value: '.');

            var operation = segments.Slice(lastDot + 1).ToString();
            var property = segments.Slice(firstDot + 1, lastDot - firstDot - 1).ToString();

            var desiredFactory = filterFactories.First(factory => factory.Key.IsEnumDefined(operation));

            var filter = desiredFactory.Value(operation, property, value);

            return filter;
        }
    }
    
}