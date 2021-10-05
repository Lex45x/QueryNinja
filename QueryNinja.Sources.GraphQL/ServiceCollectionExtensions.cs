using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore;
using QueryNinja.Sources.AspNetCore.Factory;
using QueryNinja.Sources.GraphQL.Middleware;
using QueryNinja.Sources.GraphQL.SchemaGeneration;
using QueryNinja.Sources.GraphQL.Serializers;
using QueryNinja.Targets.Queryable;

namespace QueryNinja.Sources.GraphQL
{
    /// <summary>
    ///   Provides extensions methods to register GraphQL Source for QueryNinja.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///   Registers AspNetCore Source and all required GraphQL-related services.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>
        ///   <see cref="QueryNinjaExtensions.Configure" />
        /// </returns>
        public static IGraphQLExtensionSettings AddQueryNinjaGraphQL(this IServiceCollection collection)
        {
            var extensionSettings = collection.AddQueryNinja();
            extensionSettings.WithQueryableTarget();

            collection.Add(ServiceDescriptor.Singleton<IQuerySerializer<IDynamicQuery>>(_ =>
                new DynamicQuerySerializer()));

            collection.Add(ServiceDescriptor.Singleton<IActionsScanner>(provider =>
                new ActionsScanner(provider.GetRequiredService<IActionDescriptorCollectionProvider>())));

            collection.Add(ServiceDescriptor.Singleton<IGraphQLQueriesSource>(provider =>
                new GraphQLQueriesSource(provider.GetRequiredService<IActionsScanner>(),
                    provider.GetRequiredService<IQuerySerializer<IDynamicQuery>>())));

            collection.Add(ServiceDescriptor.Singleton<IGraphQLRequestHandler>(provider =>
                new GraphQLRequestHandler(provider.GetRequiredService<IGraphQLQueriesSource>())));

            return new ExtensionSettings(extensionSettings);
        }

        private class ExtensionSettings : IGraphQLExtensionSettings
        {
            private readonly IAspNetCoreExtensionSettings extensionSettings;

            public ExtensionSettings(IAspNetCoreExtensionSettings extensionSettings)
            {
                this.extensionSettings = extensionSettings;
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TExtension> ForType<TExtension>()
                where TExtension : IQueryComponentExtension
            {
                return extensionSettings.ForType<TExtension>();
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent(Type componentType)
            {
                return extensionSettings.RegisterComponent(componentType);
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent<TComponent>()
                where TComponent : IQueryComponent
            {
                return extensionSettings.RegisterComponent<TComponent>();
            }

            /// <inheritdoc />
            public IAspNetCoreExtensionSettings ConfigureFilterFactory(Action<DefaultFilterFactory> configure)
            {
                return extensionSettings.ConfigureFilterFactory(configure);
            }
        }
    }

    /// <summary>
    ///   Allows to configure GraphQL Source
    /// </summary>
    public interface IGraphQLExtensionSettings : IAspNetCoreExtensionSettings
    {
    }
}