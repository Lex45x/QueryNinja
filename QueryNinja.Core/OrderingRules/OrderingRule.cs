using QueryNinja.Core.Filters;
using System;

namespace QueryNinja.Core.OrderingRules
{
    /// <summary>
    /// Represent a rule to define Target collection order.
    /// </summary>
    public sealed class OrderingRule : IQueryComponent
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

        private bool Equals(OrderingRule other)
        {
            return Property == other.Property && Direction == other.Direction;
        }

        public bool Equals(IQueryComponent? other)
        {
            return Equals((object?)other);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) 
                   || (obj is OrderingRule other && Equals(other));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Property, (int)Direction);
        }
    }
}
