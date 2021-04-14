using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Projection;

namespace QueryNinja.Sources.AspNetCore.ModelBinding
{
    /// <summary>
    /// ModelBinder that can create <see cref="IQuery"/> instance from request parameters. <br/>
    /// Currently, only binding from <see cref="BindingSource.Query"/> is supported.
    /// </summary>
    public class QueryNinjaModelBinderLINQ : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var components = GetQueryComponents(bindingContext.HttpContext.Request.Query).ToList();

            if (bindingContext.ModelType == typeof(IQuery))
            {
                var result = new Query(components);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            if (bindingContext.ModelType == typeof(IDynamicQuery))
            {
                var selectors = GetSelectors(bindingContext.HttpContext.Request.Query).ToList();

                var result = new DynamicQuery(components, selectors);

                bindingContext.Result = ModelBindingResult.Success(result);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        private static IEnumerable<ISelector> GetSelectors(IQueryCollection queryParameters)
        {
            foreach (var (key, value) in queryParameters.Where(parameter => parameter.Key.Contains("select")))
            {
                if (string.Equals(key, "select", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var sourceProperty in value)
                    {
                        yield return new Selector(sourceProperty);
                    }
                }
                else
                {
                    var keySpan = key.AsSpan();

                    var propertyNameStart = keySpan.IndexOf(value: '.') + 1;
                    var sourceProperty = keySpan.Slice(propertyNameStart).ToString();

                    foreach (var targetProperty in value)
                    {
                        yield return new RenameSelector(sourceProperty, targetProperty);
                    }
                }
            }
        }

        private static IEnumerable<IQueryComponent> GetQueryComponents(IQueryCollection queryParameters)
        {
            var factories = QueryNinjaExtensions.Extensions<IQueryComponentFactory>().ToList();

            foreach (var (key, value) in queryParameters)
            {
                var suitableFactory = factories.FirstOrDefault(factory => factory.CanApply(key, value));

                if (suitableFactory != null)
                {
                    yield return suitableFactory.Create(key, value);
                }
            }
        }
    }
}