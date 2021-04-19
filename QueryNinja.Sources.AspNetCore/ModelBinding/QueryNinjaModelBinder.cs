using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Projection;
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
            var components = GetQueryComponents(bindingContext.HttpContext.Request.Query);

            if (bindingContext.ModelType == typeof(IQuery))
            {
                var result = new Query(components);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            if (bindingContext.ModelType == typeof(IDynamicQuery))
            {
                var selectors = GetSelectors(bindingContext.HttpContext.Request.Query);

                var result = new DynamicQuery(components, selectors);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private static IReadOnlyList<ISelector> GetSelectors(IQueryCollection queryParameters)
        {
            var selectors = new List<ISelector>();

            foreach (var (key, value) in queryParameters)
            {
                if (!key.Contains("select"))
                {
                    continue;
                }

                if (string.Equals(key, "select", StringComparison.OrdinalIgnoreCase))
                {
                    for (var valueIndex = 0; valueIndex < value.Count; valueIndex++)
                    {
                        var selector = new Selector(value[valueIndex]);
                        selectors.Add(selector);
                    }
                }
                else
                {
                    var keySpan = key.AsSpan();

                    var propertyNameStart = keySpan.IndexOf(value: '.') + 1;
                    var sourceProperty = keySpan.Slice(propertyNameStart).ToString();

                    for (var valueIndex = 0; valueIndex < value.Count; valueIndex++)
                    {
                        var selector = new RenameSelector(sourceProperty, value[valueIndex]);
                        selectors.Add(selector);
                    }
                }
            }

            return selectors;
        }

        private static IReadOnlyList<IQueryComponent> GetQueryComponents(IQueryCollection queryParameters)
        {
            var queryComponents = new List<IQueryComponent>();

            var factories = QueryNinjaExtensions.Extensions<IQueryComponentFactory>();
            
            foreach (var (key, value) in queryParameters)
            {
                for (var factoryIndex = 0; factoryIndex < factories.Count; factoryIndex++)
                {
                    var factory = factories[factoryIndex];

                    if (!factory.CanApply(key, value))
                    {
                        continue;
                    }

                    var queryComponent = factory.Create(key, value);
                    queryComponents.Add(queryComponent);
                    break;
                }
            }

            return queryComponents;
        }
    }
}