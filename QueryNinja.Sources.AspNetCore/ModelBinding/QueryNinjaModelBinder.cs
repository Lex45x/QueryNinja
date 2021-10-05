using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Projection;
using QueryNinja.Sources.AspNetCore.Reflection;

// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace QueryNinja.Sources.AspNetCore.ModelBinding
{
    /// <summary>
    ///   ModelBinder that can create <see cref="IQuery" /> instance from request parameters. <br />
    ///   Currently, only binding from <see cref="BindingSource.Query" /> is supported.
    /// </summary>
    public class QueryNinjaModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var components = GetQueryComponents(bindingContext.HttpContext.Request.Query);

            if (typeof(IDynamicQuery).IsAssignableFrom(bindingContext.ModelType))
            {
                var selectors = GetSelectors(bindingContext.HttpContext.Request.Query["select"]);

                var result = bindingContext.ModelType.IsGenericType
                    ? GenericQueryFactory.DynamicQuery(bindingContext.ModelType, components, selectors)
                    : new DynamicQuery(components, selectors);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            if (typeof(IQuery).IsAssignableFrom(bindingContext.ModelType))
            {
                var result = bindingContext.ModelType.IsGenericType
                    ? GenericQueryFactory.Query(bindingContext.ModelType, components)
                    : new Query(components);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private static IReadOnlyList<ISelector> GetSelectors(IReadOnlyList<string> selects)
        {
            var selectorComponents = new string[selects.Count][];

            for (var valueIndex = 0; valueIndex < selects.Count; valueIndex++)
            {
                selectorComponents[valueIndex] = selects[valueIndex].Split(separator: '.');
            }

            var result = BuildSelectorsLayer(selectorComponents, layer: 0);
            return result;
        }

        private static IReadOnlyList<ISelector> BuildSelectorsLayer(IEnumerable<string[]> selectors, int layer)
        {
            var result = new List<ISelector>();

            var groupBy = selectors
                .Where(selector => selector.Length >= layer + 1)
                .GroupBy(selector => selector[layer]);

            foreach (var group in groupBy)
            {
                var nestedSelectors = BuildSelectorsLayer(group, layer + 1);
                result.Add(new Selector(group.Key, nestedSelectors: nestedSelectors));
            }

            return result;
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