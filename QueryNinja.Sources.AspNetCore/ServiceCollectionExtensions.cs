using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Sources.AspNetCore.ModelBinding;
using System;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore.Factory;

namespace QueryNinja.Sources.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="QueryNinjaModelBinderProvider"/> to <see cref="MvcOptions"/> and configures <see cref="QueryNinjaExtensions"/> <br/>
        /// Also, allows to register user-defined operation-based filters via <paramref name="configureFactory"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="configureFactory">Delegate to configure <see cref="DefaultFilterFactory"/> with user-defined filters.</param>
        /// <returns><see cref="QueryNinjaExtensions.Configure"/></returns>
        public static IExtensionsSettings AddQueryNinja(this IServiceCollection collection, Action<DefaultFilterFactory> configureFactory = null)
        {
            var factory = new DefaultFilterFactory();
            configureFactory?.Invoke(factory);
            QueryNinjaExtensions.Configure.Register(factory);
            QueryNinjaExtensions.Configure.Register<OrderingRuleFactory>();

            collection.Configure<MvcOptions>(options => options.ModelBinderProviders.Insert(index: 0, new QueryNinjaModelBinderProvider()));

            return QueryNinjaExtensions.Configure;
        }
    }
}
