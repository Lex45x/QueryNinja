using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using QueryNinja.Core.Extensibility;

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
            var components = GetQueryComponents(bindingContext.HttpContext.Request.Query).ToList();

            var result = new Query(components);

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }


        private IEnumerable<IQueryComponent> GetQueryComponents(IQueryCollection queryParameters)
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