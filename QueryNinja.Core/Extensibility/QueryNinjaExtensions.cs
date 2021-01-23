using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Contains all descriptors related to all plugins.
    /// </summary>
    public static class QueryNinjaExtensions
    {
        //prevents any extension to be registered twice
        private static readonly HashSet<IQueryComponentExtension> ExtensionsSet =
            new HashSet<IQueryComponentExtension>(new TypeBasedEqualityComparer());

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
        public static IEnumerable<TExtension> Extensions<TExtension>()
            where TExtension : IQueryComponentExtension
        {
            return ExtensionsSet.OfType<TExtension>();
        }

        /// <summary>
        /// Allows to modify Extensions List and develop extension methods to do it in a simple way. <br/>
        /// <b>It is not possible to register two instances of the extension with the same Type. This type of actions will be ignored.</b>
        /// </summary>
        public static IExtensionsSettings Configure { get; } = new ExtensionsSettings();


        /// <inheritdoc/>
        private class ExtensionsSettings
            : IExtensionsSettings
        {
            /// <inheritdoc/>
            public IExtensionsSettings Register(IQueryComponentExtension extension)
            {
                ExtensionsSet.Add(extension);
                RegisterComponent(extension.QueryComponent);
                return this;
            }

            /// <inheritdoc/>
            public IExtensionsSettings Register<TExtension>()
                where TExtension : IQueryComponentExtension, new()
            {
                Register(new TExtension());
                return this;
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

        /// <summary>
        /// Compares <see cref="IQueryComponentExtension"/> by Type.
        /// </summary>
        private class TypeBasedEqualityComparer : IEqualityComparer<IQueryComponentExtension>
        {
            public bool Equals(IQueryComponentExtension x, IQueryComponentExtension y)
            {
                return x.GetType() == y.GetType();
            }

            public int GetHashCode(IQueryComponentExtension obj)
            {
                return obj.GetType().GetHashCode();
            }
        }

        /// <summary>
        /// Clears <see cref="Extensions{TExtension}"/> and <see cref="KnownQueryComponents"/>. <br/>
        /// Usage intended only for testing purposes to reset static state.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Nice to have.")]
        public static void Clear()
        {
            ExtensionsSet.Clear();
            KnownQueryComponentsSet.Clear();
        }
    }
}