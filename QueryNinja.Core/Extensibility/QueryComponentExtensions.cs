using System.Collections.Generic;
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
        public class ExtensionsSettings
            : IExtensionsSettings
        {
            /// <inheritdoc/>
            public void Register(IQueryComponentExtension extension)
            {
                ExtensionsSet.Add(extension);
            }

            /// <inheritdoc/>
            public void Register<TExtension>()
                where TExtension : IQueryComponentExtension, new()
            {
                Register(new TExtension());
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
    }
}