using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;
using System.Linq;
using System.Threading.Tasks;
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
            var components =
                bindingContext.HttpContext.Request.Query
                    .Select(queryParameter => QueryNinjaExtensions.Extensions<IQueryComponentFactory>()
                        .FirstOrDefault(factory => factory.CanApply(queryParameter.Key, queryParameter.Value))
                    ?.Create(queryParameter.Key, queryParameter.Value))
                .ToList();

            var result = new Query(components);

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}