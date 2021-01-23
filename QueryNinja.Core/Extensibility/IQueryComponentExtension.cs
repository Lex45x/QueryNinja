using System;
using System.Diagnostics.CodeAnalysis;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Interface that used to use <see cref="IQueryComponent"/> in Target or Source package <br/>
    /// Do not use this interface directly. For developing own query components use interfaces defined in Target or Source package.
    /// </summary>
    public interface IQueryComponentExtension
    {
        /// <summary>
        /// Type of the component this extension is related to.
        /// </summary>
        Type QueryComponent { get; }
    }
}