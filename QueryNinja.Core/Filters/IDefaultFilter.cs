using System;

namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Represent a rule to filter Target collection with selected <see cref="Operation"/>.
    /// </summary>
    public interface IDefaultFilter<out TOperation> : IFilter
        where TOperation : Enum
    {
        /// <summary>
        /// Enum that will describe all possible operations in this filter.
        /// </summary>
        TOperation Operation { get; }

        /// <summary>
        /// Target property name
        /// </summary>
        string Property { get; }

        /// <summary>
        /// Value to use for the <see cref="Operation"/> with property
        /// </summary>
        string Value { get; }
    }
}