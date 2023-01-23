using System;
using System.Diagnostics.CodeAnalysis;
using QueryNinja.Core.Factories;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Interface that allow to configure <see cref="QueryNinjaExtensions"/>
    /// </summary>
    public interface IExtensionsSettings
    {
        /// <summary>
        /// Allows configuration for specific <see cref="IQueryComponentExtension"/> descendant.
        /// </summary>
        /// <typeparam name="TExtension">Extension to configure.</typeparam>
        /// <returns>Settings related to specific Extension.</returns>
        public IExtensionTypeSettings<TExtension> ForType<TExtension>()
            where TExtension : IQueryComponentExtension;

        /// <summary>
        /// Registers existence of a specific query component type. <br/>
        /// Later on, Sources or Targets <b>may</b> use this information.
        /// </summary>
        /// <param name="componentType">Type that implements <see cref="IQueryComponent"/></param>
        IExtensionsSettings RegisterComponent(Type componentType);

        /// <summary>
        /// Registers existence of a specific query component type. <br/>
        /// Later on, Sources or Targets <b>may</b> use this information.
        /// </summary>
        /// <typeparam name="TComponent">Type that implements <see cref="IQueryComponent"/></typeparam>
        IExtensionsSettings RegisterComponent<TComponent>()
            where TComponent : IQueryComponent;

        /// <summary>
        /// Allows to extend <see cref="DefaultFilterSerializer"/> with user-defined filters.
        /// </summary>
        /// <param name="configure">Delegate to configure <see cref="DefaultFilterSerializer"/> with user-defined filters.</param>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
        IExtensionsSettings ConfigureFilterFactory(Action<DefaultFilterSerializer> configure);
    }
}