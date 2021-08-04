using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.Exceptions;

namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Represent selector that leaves path to properties as-is.
    /// </summary>
    public class Selector : ISelector
    {
        /// <summary>
        /// Creates instance of Selector.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="nestedSelectors"></param>
        public Selector(string source, IReadOnlyList<ISelector>? nestedSelectors = null)
        {
            if (source.Contains(value: '.'))
            {
                throw new CompatibilityException("Selectors are no longer support full path to property. Use NestedSelectors property instead.");
            }

            Source = source;
            NestedSelectors = nestedSelectors ?? new List<ISelector>();
        }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public IReadOnlyList<ISelector> NestedSelectors { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var nestedSelectors = NestedSelectors.Select(selector => selector.ToString()).ToList();
            var nested = string.Join(separator: ',', nestedSelectors);

            return $"{Source} \n{{\n {nested} \n}}";
        }
    }
}