namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Filter with collection-related operations. Operations are defined in <see cref="CollectionOperation"/>
    /// </summary>
    public sealed class CollectionFilter : AbstractDefaultFilter<CollectionOperation>
    {
        /// <inheritdoc />
        public CollectionFilter(CollectionOperation operation, string property, string value)
            : base(operation, property, value)
        {
        }
    }
}
