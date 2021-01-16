using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QueryNinja.Extensions.AspNetCore.Swagger
{
    public static class SwaggerGenExtensions
    {
        public static void WithQueryNinja(this SwaggerGenOptions options)
        {
            options.ParameterFilter<QueryParameterFilter>();
        }
    }
}