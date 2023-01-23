using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Factories;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    /// Allows to register IQueryable Target in <see cref="QueryNinjaExtensions"/>
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Extends <paramref name="settings"/> with default query builders.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IQueryableExtensionsSettings WithQueryableTarget(this IExtensionsSettings settings)
        {
            settings.ForType<IQueryBuilder>()
                .Register<CollectionFilterQueryBuilder>()
                .Register<ComparisonFilterQueryBuilder>()
                .Register<ArrayEntryFilterQueryBuilder>()
                .Register<OrderQueryBuilder>();
                

            return new ExtensionSettings(settings);
        }

        private class ExtensionSettings : IQueryableExtensionsSettings
        {
            private readonly IExtensionsSettings extensionsSettings;

            public ExtensionSettings(IExtensionsSettings extensionsSettings)
            {
                this.extensionsSettings = extensionsSettings;
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TExtension> ForType<TExtension>()
                where TExtension : IQueryComponentExtension
            {
                return extensionsSettings.ForType<TExtension>();
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent(Type componentType)
            {
                return extensionsSettings.RegisterComponent(componentType);
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent<TComponent>()
                where TComponent : IQueryComponent
            {
                return extensionsSettings.RegisterComponent<TComponent>();
            }

            public IExtensionsSettings ConfigureFilterFactory(Action<DefaultFilterSerializer> configure)
            {
                return extensionsSettings.ConfigureFilterFactory(configure);
            }

            /// <inheritdoc />
            public IQueryableExtensionsSettings AddFilter<TFilter, TOperation>(
                Action<DefaultFilterQueryBuilder<TFilter, TOperation>> configure)
                where TFilter : IDefaultFilter<TOperation> where TOperation : Enum
            {
                var queryBuilder = new DefaultFilterQueryBuilder<TFilter, TOperation>();

                configure(queryBuilder);

                RegisterComponent<TFilter>();
                ForType<IQueryBuilder>().Register(queryBuilder);

                return this;
            }
        }
    }
}