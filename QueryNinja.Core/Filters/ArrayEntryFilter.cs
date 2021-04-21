using System;
using QueryNinja.Core.Attributes;

namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Checks whether target property is contained inside array
    /// </summary>
    [DefinedForPrimitives]
    public class ArrayEntryFilter : AbstractDefaultFilter<ArrayEntryOperations>
    {
        /// <inheritdoc />
        public ArrayEntryFilter(ArrayEntryOperations operation, string property, string value)
            : base(operation, property, value)
        {
        }
    }
}