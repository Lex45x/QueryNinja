using System;
using System.Collections.Generic;

namespace QueryNinja.Core.Extensibility
{
    /// <summary>
    /// Allows to create <see cref="IQueryComponent"/> instance from Query Parameters
    /// </summary>
    public interface IQueryComponentSerializer : IQueryComponentExtension
    {
        /// <summary>
        /// Checks whether current factory can be applied to the specified query parameter.
        /// </summary>
        /// <param name="path">Path to a property</param>
        /// <param name="value">Value of the filter</param>
        /// <returns></returns>
        bool CanDeserialize(ReadOnlySpan<char> path, string value);

        /// <summary>
        /// Creates <see cref="IQueryComponent"/> instance from the specified query parameter.
        /// </summary>
        /// <param name="path">Path to a property</param>
        /// <param name="value">Value of the filter</param>
        /// <returns></returns>
        IQueryComponent Deserialize(ReadOnlySpan<char> path, string value);

        /// <summary>
        /// Checks whether current factory can be applied to the specified query parameter.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool CanSerialize(IQueryComponent component);

        /// <summary>
        /// Allows to get serialized representation of a given component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        KeyValuePair<string, string> Serialize(IQueryComponent component);
    }
}