using QueryNinja.Core.Attributes;

namespace QueryNinja.Core.OrderingRules
{
    /// <summary>
    /// Represent a rule to define Target collection order.
    /// </summary>
    [DefinedForPrimitives]
    public sealed class OrderingRule : ISpecificQueryComponent
    {
        /// <summary>
        /// Creates ordering rule.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
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

        /// <summary>
        /// Debug-friendly implementation. Return string representation of <see cref="OrderingRule"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Property} {Direction}";
        }
    }
}
