using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore.Factory;
using QueryNinja.Sources.AspNetCore.ModelBinding;

namespace QueryNinja.Sources.AspNetCore
{
    /// <summary>
    ///   Allows simplified registration of all required QueryNinja services in <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///   Adds <see cref="QueryNinjaModelBinderProvider" /> to <see cref="MvcOptions" /> and configures
        ///   <see cref="QueryNinjaExtensions" /> <br />
        ///   Also, allows to register user-defined operation-based filters via <see cref="IAspNetCoreExtensionSettings" />
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>
        ///   <see cref="QueryNinjaExtensions.Configure" />
        /// </returns>
        public static IAspNetCoreExtensionSettings AddQueryNinja(this IServiceCollection collection)
        {
            var factory = new DefaultFilterFactory();
            QueryNinjaExtensions.Configure
                .ForType<IQueryComponentFactory>()
                .Register(factory)
                .Register<OrderingRuleFactory>();

            collection.Configure<MvcOptions>(options =>
                options.ModelBinderProviders.Insert(index: 0, new QueryNinjaModelBinderProvider()));

            return new ExtensionSettings(QueryNinjaExtensions.Configure, factory);
        }

        /// <summary>
        ///   Add AspNetCores source to <see cref="QueryNinjaExtensions.Configure" /> only. Does not affects any AspNet services.
        ///   <br />
        ///   Has to be used only for testing purposes.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static IAspNetCoreExtensionSettings WithAspNetCoreSource(this IExtensionsSettings settings)
        {
            var factory = new DefaultFilterFactory();
            settings.ForType<IQueryComponentFactory>()
                .Register(factory)
                .Register<OrderingRuleFactory>();

            return new ExtensionSettings(QueryNinjaExtensions.Configure, factory);
        }

        private class ExtensionSettings : IAspNetCoreExtensionSettings
        {
            private readonly DefaultFilterFactory factory;
            private readonly IExtensionsSettings parent;

            public ExtensionSettings(IExtensionsSettings parent, DefaultFilterFactory factory)
            {
                this.parent = parent;
                this.factory = factory;
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TExtension> ForType<TExtension>()
                where TExtension : IQueryComponentExtension
            {
                return parent.ForType<TExtension>();
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