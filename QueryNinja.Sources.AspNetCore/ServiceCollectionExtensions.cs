using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Sources.AspNetCore.ModelBinding;
using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore.Factory;

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
        public static IAspNetCoreExtensionSettings AddQueryNinja(this IServiceCollection collection)
        {
            var factory = new DefaultFilterFactory();
            QueryNinjaExtensions.Configure.Register(factory);

            QueryNinjaExtensions.Configure.Register<OrderingRuleFactory>();


            collection.Configure<MvcOptions>(options =>
                options.ModelBinderProviders.Insert(index: 0, new QueryNinjaModelBinderProvider()));

            return new ExtensionSettings(QueryNinjaExtensions.Configure, factory);
        }

        private class ExtensionSettings : IAspNetCoreExtensionSettings
        {
            private readonly IExtensionsSettings parent;
            private readonly DefaultFilterFactory factory;

            public ExtensionSettings(IExtensionsSettings parent, DefaultFilterFactory factory)
            {
                this.parent = parent;
                this.factory = factory;
            }

            /// <inheritdoc />
            public IExtensionsSettings Register(IQueryComponentExtension extension)
            {
                parent.Register(extension);
                return parent;
            }

            /// <inheritdoc />
            public IExtensionsSettings Register<TExtension>()
                where TExtension : IQueryComponentExtension, new()
            {
                parent.Register<TExtension>();
                return parent;
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent(Type componentType)
            {
                parent.RegisterComponent(componentType);
                return parent;
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent<TComponent>()
                where TComponent : IQueryComponent
            {
                parent.RegisterComponent<TComponent>();
                return parent;
            }

            /// <inheritdoc />
            public IAspNetCoreExtensionSettings ConfigureFilterFactory(Action<DefaultFilterFactory> configure)
            {
                configure(factory);
                return this;
            }
        }
    }
}