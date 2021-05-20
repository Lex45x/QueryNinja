using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Allows to execute <see cref="Source"/> method and write result to <see cref="Target"/> property.
    /// </summary>
    public class ExecuteSelector : ISelector
    {
        /// <summary>
        /// Creates new instance of selector.
        /// </summary>
        /// <param name="source">Source method to call.</param>
        /// <param name="target">Target property to write results.</param>
        /// <param name="arguments">Arguments for <paramref name="source"/> method.</param>
        public ExecuteSelector(string source, string target, IReadOnlyDictionary<string, object> arguments)
        {
            Source = source;
            Target = target;
            Arguments = arguments;
        }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public string Target { get; }

        /// <summary>
        /// Arguments to be passed into <see cref="Source"/> method. <br/>
        /// If <see cref="KeyValuePair{TKey,TValue}.Value"/> is not matching expected parameter type then <see cref="TypeDescriptor"/> will be used for conversion.
        /// </summary>
        public IReadOnlyDictionary<string, object> Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var arguments = Arguments.Select(pair => $"{pair.Key}: {pair.Value}");

            var stringOfArguments = string.Join(",", arguments);

            return $"{Source}({stringOfArguments}) -> {Target}";
        }
    }
}