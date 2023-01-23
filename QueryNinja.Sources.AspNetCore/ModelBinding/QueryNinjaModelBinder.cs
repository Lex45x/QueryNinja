using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Projection;

// Performance optimization here. All LINQ is replaced with for-loops.
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace QueryNinja.Sources.AspNetCore.ModelBinding
{
    /// <summary>
    /// ModelBinder that can create <see cref="IQuery"/> instance from request parameters. <br/>
    /// Currently, only binding from <see cref="BindingSource.Query"/> is supported.
    /// </summary>
    public class QueryNinjaModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //if we have multiple queries, then we have to use original model name as query parameter prefix.
            var queriesCount = 0;
            for (var index = 0; index < bindingContext.ActionContext.ActionDescriptor.Parameters.Count; index++)
            {
                var descriptor = bindingContext.ActionContext.ActionDescriptor.Parameters[index];
                if (typeof(IQuery).IsAssignableFrom(descriptor.ParameterType)) queriesCount++;
            }

            var queryPrefix = queriesCount > 1 ? bindingContext.OriginalModelName : null;

            var components = GetQueryComponents(bindingContext.HttpContext.Request.Query, queryPrefix);

            if (bindingContext.ModelType == typeof(IQuery))
            {
                var result = new Query(components);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            if (bindingContext.ModelType == typeof(IDynamicQuery))
            {
                var selectors = GetSelectors(bindingContext.HttpContext.Request.Query, queryPrefix);

                var result = new DynamicQuery(components, selectors);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private static IReadOnlyList<ISelector> GetSelectors(IQueryCollection queryParameters, string queryName = null)
        {
            var selectors = new List<ISelector>();

            foreach ((ReadOnlySpan<char> key, var value) in queryParameters)
            {
                if (!key.Contains("select", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var queryIndependentKey = queryName != null && key.StartsWith(queryName)
                    ? key[(queryName.Length + 1)..]
                    : key;

                if (queryIndependentKey.Equals("select", StringComparison.OrdinalIgnoreCase))
                {
                    for (var valueIndex = 0; valueIndex < value.Count; valueIndex++)
                    {
                        var selector = new Selector(value[valueIndex]);
                        selectors.Add(selector);
                    }
                }
                else
                {
                    var propertyNameStart = queryIndependentKey.IndexOf(value: '.') + 1;
                    var sourceProperty = queryIndependentKey[propertyNameStart..].ToString();

                    for (var valueIndex = 0; valueIndex < value.Count; valueIndex++)
                    {
                        var selector = new RenameSelector(sourceProperty, value[valueIndex]);
                        selectors.Add(selector);
                    }
                }
            }

            return selectors;
        }

        private static IReadOnlyList<IQueryComponent> GetQueryComponents(IQueryCollection queryParameters,
            string queryName = null)
        {
            var queryComponents = new List<IQueryComponent>();

            var factories = QueryNinjaExtensions.Extensions<IQueryComponentSerializer>();

            foreach ((ReadOnlySpan<char> key, var value) in queryParameters)
            {
                for (var factoryIndex = 0; factoryIndex < factories.Count; factoryIndex++)
                {
                    var factory = factories[factoryIndex];

                    var queryIndependentKey = queryName != null && key.StartsWith(queryName)
                        ? key[(queryName.Length + 1)..]
                        : key;

                    if (!factory.CanDeserialize(queryIndependentKey, value))
                    {
                        continue;
                    }

                    var queryComponent = factory.Deserialize(queryIndependentKey, value);
                    queryComponents.Add(queryComponent);
                    break;
                }
            }

            return queryComponents;
        }
    }
}