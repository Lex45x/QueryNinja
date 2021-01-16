namespace QueryNinja.Core.OrderingRules
{
    /// <summary>
    /// Represent desired order direction for the <see cref="OrderingRule"/>
    /// </summary>
    public enum OrderDirection
    {
        Ascending = 1 << 0,
        Descending = 1 << 1
    }
}
