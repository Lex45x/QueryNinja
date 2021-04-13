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
            var isBindingSourceCorrect = context.ApiParameterDescription.Source == BindingSource.Query;

            if (!isBindingSourceCorrect)
            {
                return;
            }

            var isDynamicQuery = typeof(IDynamicQuery).IsAssignableFrom(context.ParameterInfo.ParameterType);

            if (isDynamicQuery)
            {
                parameter.Explode = true;
                parameter.Style = ParameterStyle.Form;
                parameter.In = ParameterLocation.Query;
                parameter.Example = new OpenApiObject
                {
                    ["filters.Property.Equals"] = new OpenApiInteger(value: 0),
                    ["order.Property"] = new OpenApiString("Ascending"),
                    ["select"] = new OpenApiString("Property")
                };
                return;
            }

            var isQuery = typeof(IQuery).IsAssignableFrom(context.ParameterInfo.ParameterType);

            if (isQuery)
            {
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
}