using QueryNinja.Core.Attributes;

namespace QueryNinja.Core.Filters
{
    
    /// <summary>
    /// Filter that works with primitive types and comparison operations. <br/>
    /// List of supported operations could be fined here: <see cref="ComparisonOperation"/>.
    /// </summary>
    [DefinedForPrimitives]
    public sealed class ComparisonFilter : AbstractDefaultFilter<ComparisonOperation>
    {
        /// <inheritdoc />
        public ComparisonFilter(ComparisonOperation operation, string property, string value)
            : base(operation, property, value)
        {
        }
    }
}
