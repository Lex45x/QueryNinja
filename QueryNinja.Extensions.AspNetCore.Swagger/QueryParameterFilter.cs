using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using QueryNinja.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QueryNinja.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// Treats all <see cref="IQuery"/> implementations in query parameters as object.
    /// </summary>
    public class QueryParameterFilter : IParameterFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var isTypeCorrect = typeof(IQuery).IsAssignableFrom(context.ParameterInfo.ParameterType);
            var isBindingSourceCorrect = context.ApiParameterDescription.Source == BindingSource.Query;

            if (!isTypeCorrect && !isBindingSourceCorrect)
            {
                return;
            }
            
            parameter.Explode = true;
            parameter.Style = ParameterStyle.Form;
            parameter.In = ParameterLocation.Query;
            parameter.Example = new OpenApiObject
            {
                ["filters.Property.Equals"] = new OpenApiInteger(value: 0),
                ["order.Property"] = new OpenApiString("Ascending")
            };
        }
    }
}
