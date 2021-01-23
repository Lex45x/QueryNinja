using System;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Interface that allow to configure <see cref="QueryNinjaExtensions"/>
    /// </summary>
    public interface IExtensionsSettings
    {
        /// <summary>
        /// Add new instance of the <see cref="IQueryComponentExtension"/> to extensions collection. <br/>
        /// Also, will automatically add <see cref="IQueryComponentExtension.QueryComponent"/> to known components. <br/>
        /// <b>It is not possible to register two instances of the extension with the same Type. This type of actions will be ignored.</b>
        /// </summary>
        /// <param name="extension"></param>
        IExtensionsSettings Register(IQueryComponentExtension extension);

        /// <summary>
        /// Creates and adds new instance of the <typeparamref name="TExtension"/> to extensions collection <br/>
        /// <b>It is not possible to register two instances of the extension with the same Type. This type of actions will be ignored.</b>
        /// </summary>
        /// <typeparam name="TExtension"></typeparam>
        IExtensionsSettings Register<TExtension>()
            where TExtension : IQueryComponentExtension, new();

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
    }
}