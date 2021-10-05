using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Core;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QueryNinja.Extensions.AspNetCore.Swagger
{
    /// <summary>
    ///   Allows to extend Swagger Generator with extensions needed to properly display <see cref="IQuery" /> parameter.
    /// </summary>
    public static class SwaggerGenExtensions
    {
        /// <summary>
        ///   Adds <see cref="QueryParameterFilter" /> to <see cref="SwaggerGenOptions" />
        /// </summary>
        /// <param name="options"></param>
        public static void WithQueryNinja(this SwaggerGenOptions options)
        {
            options.ParameterFilter<QueryParameterFilter>();
        }
    }
}