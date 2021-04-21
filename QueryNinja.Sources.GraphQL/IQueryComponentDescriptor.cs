using System;
using System.Collections.Generic;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;

namespace QueryNinja.Sources.GraphQL
{
    /// <summary>
    /// Allows schema generation and creating instances of <see cref="IQueryComponent"/>
    /// </summary>
    public interface IQueryComponentDescriptor : IQueryComponentExtension
    {
        /// <summary>
        /// Get list of Types for which desired IQueryComponent is defined.
        /// </summary>
        IReadOnlyList<Type> DefinedOn { get; }
    }
}