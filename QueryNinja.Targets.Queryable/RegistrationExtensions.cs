using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
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
            settings.Register<CollectionFilterQueryBuilder>();
            settings.Register<ComparisonFilterQueryBuilder>();
            settings.Register<OrderQueryBuilder>();

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
            public IExtensionsSettings Register(IQueryComponentExtension extension)
            {
                return extensionsSettings.Register(extension);
            }

            /// <inheritdoc />
            public IExtensionsSettings Register<TExtension>()
                where TExtension : IQueryComponentExtension, new()
            {
                return extensionsSettings.Register<TExtension>();
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

            /// <inheritdoc />
            public IQueryableExtensionsSettings AddFilter<TFilter, TOperation>(Action<DefaultFilterQueryBuilder<TFilter, TOperation>> configure)
                where TFilter : IDefaultFilter<TOperation> where TOperation : Enum
            {
                var queryBuilder = new DefaultFilterQueryBuilder<TFilter, TOperation>();

                configure(queryBuilder);

                RegisterComponent<TFilter>();
                Register(queryBuilder);

                return this;
            }
        }
    }
}