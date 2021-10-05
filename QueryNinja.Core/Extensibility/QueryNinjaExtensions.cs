using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Contains all descriptors related to all plugins.
    /// </summary>
    public static class QueryNinjaExtensions
    {
        private static class ExtensionsCollection<TExtension>
            where TExtension : IQueryComponentExtension
        {
            private static readonly List<TExtension> ExtensionsList = new List<TExtension>();

            //prevents any extension to be registered twice
            internal static IReadOnlyList<TExtension> Extensions => ExtensionsList;

            internal static void AddExtension(TExtension extension)
            {
                if (Extensions.Any(componentExtension => componentExtension.GetType() == extension.GetType()))
                {
                    return;
                }

                ExtensionsList.Add(extension);
            }
        }
        
        private static readonly HashSet<Type> KnownQueryComponentsSet = new HashSet<Type>();

        /// <summary>
        /// Allows to get all known Types of <see cref="IQueryComponent"/>. <br/>
        /// Intended only for extensibility purposes.
        /// </summary>
        public static IReadOnlyCollection<Type> KnownQueryComponents => KnownQueryComponentsSet;
        
        /// <summary>
        /// Allows to get all extensions of Desired type. <br/>
        /// Should mainly be used by Targets or Sources to access required extensions.
        /// </summary>
        /// <typeparam name="TExtension">In most cases, interface or abstract class that registered extensions may derive from or implement.</typeparam>
        /// <returns></returns>
        public static IReadOnlyList<TExtension> Extensions<TExtension>()
            where TExtension : IQueryComponentExtension
        {
            return ExtensionsCollection<TExtension>.Extensions;
        }

        /// <summary>
        /// Allows to modify Extensions List and develop extension methods to do it in a simple way. <br/>
        /// <b>It is not possible to register two instances of the extension with the same Type. This type of actions will be ignored.</b>
        /// </summary>
        public static IExtensionsSettings Configure { get; } = new ExtensionsSettings();

        private class ExtensionTypeSettings<TExtension> : IExtensionTypeSettings<TExtension>
            where TExtension : IQueryComponentExtension
        {
            private readonly IExtensionsSettings nestedSettings;

            public ExtensionTypeSettings(IExtensionsSettings nestedSettings)
            {
                this.nestedSettings = nestedSettings;
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TOther> ForType<TOther>()
                where TOther : IQueryComponentExtension
            {
                return nestedSettings.ForType<TOther>();
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent(Type componentType)
            {
                return nestedSettings.RegisterComponent(componentType);
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent<TComponent>()
                where TComponent : IQueryComponent
            {
                return nestedSettings.RegisterComponent<TComponent>();
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TExtension> Register<TNewExtension>()
                where TNewExtension : TExtension, new()
            {
                Register(new TNewExtension());
                return this;
            }

            /// <inheritdoc />
            public IExtensionTypeSettings<TExtension> Register(TExtension instance)
            {
                ExtensionsCollection<TExtension>.AddExtension(instance);
                KnownQueryComponentsSet.Add(instance.QueryComponent);
                return this;
            }
        }

        /// <inheritdoc/>
        private class ExtensionsSettings
            : IExtensionsSettings
        {
            public IExtensionTypeSettings<TExtension> ForType<TExtension>()
                where TExtension : IQueryComponentExtension
            {
                return new ExtensionTypeSettings<TExtension>(this);
            }
            
            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent(Type componentType)
            {
                if (typeof(IQueryComponent).IsAssignableFrom(componentType))
                {
                    KnownQueryComponentsSet.Add(componentType);
                }
                else
                {
                    //todo: custom exception
                    throw new ArgumentException("Type should implement IQueryComponent", nameof(componentType));
                }

                return this;
            }

            /// <inheritdoc />
            public IExtensionsSettings RegisterComponent<TComponent>()
                where TComponent : IQueryComponent
            {
                KnownQueryComponentsSet.Add(typeof(TComponent));

                return this;
            }
        }
    }

    /// <summary>
    /// Settings related to specific <see cref="IQueryComponentExtension"/> type. <br/>
    /// Used to configure collections of Extensions with same parent.
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    public interface IExtensionTypeSettings<in TExtension> : IExtensionsSettings
    {
        public IExtensionTypeSettings<TExtension> Register<TNewExtension>()
            where TNewExtension : TExtension, new();

        public IExtensionTypeSettings<TExtension> Register(TExtension instance);
    }
}