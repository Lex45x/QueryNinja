using System;
using System.Linq;
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
    public class QueryParameterFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var queries = context.ApiDescription.ParameterDescriptions
                .Where(description => description.Source == BindingSource.Query)
                .Where(description => typeof(IQuery).IsAssignableFrom(description.Type))
                .ToList();

            foreach (var parameterInfo in queries)
            {
                var parameterPrefix = queries.Count > 1 ? $"{parameterInfo.Name}." : "";
                var openApiParameter = operation.Parameters.FirstOrDefault(parameter => parameter.Name == parameterInfo.Name);

                if (openApiParameter == null)
                    continue;

                var isDynamicQuery = typeof(IDynamicQuery).IsAssignableFrom(parameterInfo.Type);

                if (isDynamicQuery)
                {
                    openApiParameter.Explode = true;
                    openApiParameter.Style = ParameterStyle.Form;
                    openApiParameter.In = ParameterLocation.Query;
                    openApiParameter.Example = new OpenApiObject
                    {
                        [$"{parameterPrefix}filters.Property.Equals"] = new OpenApiInteger(value: 0),
                        [$"{parameterPrefix}order.Property"] = new OpenApiString("Ascending"),
                        [$"{parameterPrefix}select"] = new OpenApiString("Property")
                    };
                    return;
                }

                var isQuery = typeof(IQuery).IsAssignableFrom(parameterInfo.Type);

                if (isQuery)
                {
                    openApiParameter.Explode = true;
                    openApiParameter.Style = ParameterStyle.Form;
                    openApiParameter.In = ParameterLocation.Query;
                    openApiParameter.Example = new OpenApiObject
                    {
                        [$"{parameterPrefix}filters.Property.Equals"] = new OpenApiInteger(value: 0),
                        [$"{parameterPrefix}order.Property"] = new OpenApiString("Ascending")
                    };
                }
            }
        }
    }
}