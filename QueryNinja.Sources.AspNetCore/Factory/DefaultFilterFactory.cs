﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using BindingFlags = System.Reflection.BindingFlags;

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

            var factoryPresent = filterFactories.Keys.Any(operationType => operationType.IsEnumDefined(operation));

            if (factoryPresent)
            {
                return true;
            }

            //in case we don't have factory, we will attempt to create a factory dynamically.
            var factoryCreated = TryCreateFactory(operation);

            return factoryCreated;
        }

        /// <summary>
        /// Search for suitable <see cref="IDefaultFilter{TOperation}"/> in <see cref="QueryNinjaExtensions.KnownQueryComponents"/> and artificially creates factory for it.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns><code>true</code> when factory created and registered.</returns>
        private bool TryCreateFactory(string operation)
        {
            // find all implementations of IDefaultFilter
            var knownDefaultFilters = QueryNinjaExtensions.KnownQueryComponents
                .Where(@interface => @interface.GetInterface("IDefaultFilter`1") != null)
                .Where(defaultFilter => defaultFilter.GetInterface("IDefaultFilter`1").GetGenericArguments()[0]
                    .IsEnumDefined(operation))
                .ToList();

            if (knownDefaultFilters.Count == 0)
            {
                return false;
            }
            
            if (knownDefaultFilters.Count > 1)
            {
                //todo: custom exception
                throw new InvalidOperationException("Multiple filters with same operation name found!");
            }

            var filter = knownDefaultFilters.Single();

            var operationType = filter.GetInterface("IDefaultFilter`1").GetGenericArguments()[0];

            var genericCreateFactory = GetType()
                .GetMethod("TryCreateFactory", genericParameterCount: 1, BindingFlags.Instance | BindingFlags.NonPublic, binder: null, new[] {typeof(Type)}, modifiers: null);

            //can be caused only by changing the codebase.
            if (genericCreateFactory == null)
            {
                throw new InvalidOperationException(
                    "Instance member responsible for the next step in factory creation is not found!");
            }

            //TryCreateFactory has to be generic method to simplify subsequent generic method calls.
            return (bool) genericCreateFactory.MakeGenericMethod(operationType).Invoke(this, new object[] {filter});
        }

        /// <summary>
        /// Search for suitable constructor in <paramref name="filterType"/>. <br/>
        /// Create <see cref="FactoryMethod{TOperation}"/> and register it via <see cref="RegisterFilterFactory{TOperation}"/>.
        /// </summary>
        /// <typeparam name="TOperation"></typeparam>
        /// <param name="filterType"></param>
        /// <returns><code>true</code> when factory created and registered.</returns>
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used via Reflection")]
        private bool TryCreateFactory<TOperation>(Type filterType)
            where TOperation : struct, Enum
        {
            var operationParameter = Expression.Parameter(typeof(TOperation), "operation");
            var propertyParameter = Expression.Parameter(typeof(string), "property");
            var valueParameter = Expression.Parameter(typeof(string), "value");

            var constructor = filterType.GetConstructor(new[] {typeof(TOperation), typeof(string), typeof(string)});

            if (constructor == null)
            {
                return false;
            }

            var newFilterExpression =
                Expression.New(constructor, operationParameter, propertyParameter, valueParameter);

            var lambda = Expression.Lambda<FactoryMethod<TOperation>>(newFilterExpression, operationParameter,
                propertyParameter, valueParameter);

            var factoryMethod = lambda.Compile();

            RegisterFilterFactory(factoryMethod);

            return true;
        }

        ///<inheritdoc/>
        public IQueryComponent Create(string name, string value)
        {
            var segments = name.AsSpan();

            var firstDot = segments.IndexOf(value: '.');
            var lastDot = segments.LastIndexOf(value: '.');

            var operation = segments.Slice(lastDot + 1).ToString();
            var property = segments.Slice(firstDot + 1, lastDot - firstDot - 1).ToString();

            var desiredFactory = filterFactories.Single(factory => factory.Key.IsEnumDefined(operation));

            var filter = desiredFactory.Value(operation, property, value);

            return filter;
        }
    }
}