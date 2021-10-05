namespace QueryNinja.Core.OrderingRules
{
    /// <summary>
    ///   Represent desired order direction for the <see cref="OrderingRule" />
    /// </summary>
    public enum OrderDirection
    {
        /// <summary>
        ///   Ascending direction of order.
        /// </summary>
        Ascending = 1 << 0,

        /// <summary>
        ///   Descending direction of order.
        /// </summary>
        Descending = 1 << 1
    }
}