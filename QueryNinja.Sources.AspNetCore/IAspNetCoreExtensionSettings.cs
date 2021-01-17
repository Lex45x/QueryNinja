using System;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore.Factory;

namespace QueryNinja.Sources.AspNetCore
{
    /// <summary>
    /// Decorates <see cref="IExtensionsSettings"/> and extend them with additional configuration for <see cref="DefaultFilterFactory"/>
    /// </summary>
    public interface IAspNetCoreExtensionSettings : IExtensionsSettings
    {
        /// <summary>
        /// Allows to extend <see cref="DefaultFilterFactory"/> with user-defined filters.
        /// </summary>
        /// <param name="configure">Delegate to configure <see cref="DefaultFilterFactory"/> with user-defined filters.</param>
        void ConfigureFilterFactory(Action<DefaultFilterFactory> configure);
    }
}