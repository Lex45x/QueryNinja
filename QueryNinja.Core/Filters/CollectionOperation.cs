namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Represent operations applicable on the collection property.
    /// </summary>
    public enum CollectionOperation
    {
        /// <summary>
        /// Checks that collection contains desired value
        /// </summary>
        Contains = 1 << 0,

        /// <summary>
        /// Allows to check collection emptiness.
        /// </summary>
        IsEmpty = 1 << 1
    }
}
