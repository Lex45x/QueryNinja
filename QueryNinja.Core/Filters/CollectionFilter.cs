namespace QueryNinja.Core.Filters
{

    
    /// <summary>
    /// Filter with collection-related operations. Operations are defined in <see cref="CollectionOperation"/>
    /// </summary>
    public class CollectionFilter : AbstractDefaultFilter<CollectionOperation>
    {
        public CollectionFilter(CollectionOperation operation, string property, string value)
            : base(operation, property, value)
        {
        }
    }
}
