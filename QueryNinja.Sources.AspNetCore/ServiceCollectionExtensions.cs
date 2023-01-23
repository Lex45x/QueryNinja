using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Sources.AspNetCore.ModelBinding;
using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;

namespace QueryNinja.Sources.AspNetCore
{
    /// <summary>
    /// Allows simplified registration of all required QueryNinja services in <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="QueryNinjaModelBinderProvider"/> to <see cref="MvcOptions"/> and configures <see cref="QueryNinjaExtensions"/> <br/>
        /// Also, allows to register user-defined operation-based filters via <see cref="IAspNetCoreExtensionSettings"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <returns><see cref="QueryNinjaExtensions.Configure"/></returns>
        public static IExtensionsSettings AddQueryNinja(this IServiceCollection collection)
        {
            collection.Configure<MvcOptions>(options =>
                options.ModelBinderProviders.Insert(index: 0, new QueryNinjaModelBinderProvider()));

            return QueryNinjaExtensions.Configure;
        }
    }
}