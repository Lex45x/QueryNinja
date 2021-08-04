using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Allows to execute <see cref="Source"/> method and write result to <see cref="Source"/> property.
    /// </summary>
    public class ExecuteSelector : ISelector
    {
        /// <summary>
        /// Creates new instance of selector.
        /// </summary>
        /// <param name="source">Source method to call.</param>
        /// <param name="arguments">Arguments for <paramref name="source"/> method.</param>
        /// <param name="nestedSelectors"></param>
        public ExecuteSelector(string source, IReadOnlyDictionary<string, string> arguments, IReadOnlyList<ISelector>? nestedSelectors = null)
        {
            Source = source;
            Arguments = arguments;
            NestedSelectors = nestedSelectors ?? new List<ISelector>();
        }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public IReadOnlyList<ISelector> NestedSelectors { get; }

        /// <summary>
        /// Arguments to be passed into <see cref="Source"/> method. <br/>
        /// If <see cref="KeyValuePair{TKey,TValue}.Value"/> is not matching expected parameter type then <see cref="TypeDescriptor"/> will be used for conversion.
        /// </summary>
        public IReadOnlyDictionary<string, string> Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var arguments = Arguments.Select(pair => $"{pair.Key}: {pair.Value}");
            var stringOfArguments = string.Join(",", arguments);

            var nestedSelectors = NestedSelectors.Select(selector => selector.ToString()).ToList();
            var stringOfNestedSelectors = string.Join(separator: ',', nestedSelectors);

            return $"{Source}({stringOfArguments}) \n{{\n {stringOfNestedSelectors} \n}}";
        }
    }
}