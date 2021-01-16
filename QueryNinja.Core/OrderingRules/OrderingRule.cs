namespace QueryNinja.Core.OrderingRules
{
    /// <summary>
    /// Represent a rule to define Target collection order.
    /// </summary>
    public sealed class OrderingRule : IQueryComponent
    {
        public OrderingRule(string property, OrderDirection direction)
        {
            Property = property;
            Direction = direction;
        }

        /// <summary>
        /// Target property name
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Represent ordering direction for the selected property.
        /// </summary>
        public OrderDirection Direction { get; }

        public override string ToString()
        {
            return $"{Property} {Direction}";
        }
    }
}
