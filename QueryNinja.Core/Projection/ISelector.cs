using System.Collections.Generic;

namespace QueryNinja.Core.Projection
{
    /// <summary>
    ///   Allows to specify how the original properties should be selected into result projection.
    /// </summary>
    public interface ISelector
    {
        /// <summary>
        ///   Name of the source property
        /// </summary>
        string Source { get; }

        /// <summary>
        ///   Optionally represents selectors applied after to property selected in <see cref="Source" />
        /// </summary>
        IReadOnlyList<ISelector> NestedSelectors { get; }
    }
}