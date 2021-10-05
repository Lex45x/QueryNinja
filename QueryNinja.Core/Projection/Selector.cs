using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// <param name="arguments"></param>
        /// <param name="nestedSelectors"></param>
        public Selector(string source, IReadOnlyDictionary<string, string>? arguments = null,
            IReadOnlyList<ISelector>? nestedSelectors = null)
        {
            if (source.Contains(value: '.'))
            {
                throw new CompatibilityException(
                    "Selectors are no longer support full path to property. Use NestedSelectors property instead.");
            }

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
        public IReadOnlyDictionary<string, string>? Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var arguments = Arguments?.Select(pair => $"{pair.Key}: {pair.Value}") ?? Array.Empty<string>();
            var stringOfArguments = string.Join(",", arguments);

            var nestedSelectors = NestedSelectors.Select(selector => selector.ToString()).ToList();
            var stringOfNestedSelectors = string.Join(separator: ',', nestedSelectors);

            return $"{Source}({stringOfArguments}) \n{{\n {stringOfNestedSelectors} \n}}";
        }
    }
}