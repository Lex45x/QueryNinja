using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core
{
    /// <summary>
    /// Represent default query class with filters and ordering rules. <br/>
    /// </summary>
    public class Query : IQuery
    {
        private readonly IReadOnlyList<IQueryComponent> components;

        /// <summary>
        /// Creates instance of <see cref="Query"/> with defined query components.
        /// </summary>
        /// <param name="components"></param>
        public Query(IReadOnlyList<IQueryComponent> components)
        {
            this.components = components;
        }

        /// <summary>
        /// Represent a collection of filters to be applied on Target collection. <br/>
        /// All filters are join with AND operator.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
        public IReadOnlyList<IFilter> Filters => components.OfType<IFilter>().ToList();

        /// <summary>
        /// Represent a collection of order rules to be applied on Target collection. <br/>
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
        public IReadOnlyList<OrderingRule> OrderingRules => components.OfType<OrderingRule>().ToList();

        ///<inheritdoc/>
        public IReadOnlyList<IQueryComponent> GetComponents()
        {
            return components;
        }

        /// <summary>
        /// Debug-friendly implementation. Represent whole query.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = components
                .Aggregate(new StringBuilder(), (builder, component) => builder.Append($"| {component} |"))
                .ToString();
            return result;
        }

        /// <summary>
        /// Returns true if all underlying components in both queries are the same and in the same order.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(Query other)
        {
            if (components.Count != other.components.Count)
            {
                return false;
            }

            for (var i = 0; i < components.Count; i++)
            {
                if (!components[i].Equals(other.components[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(IQuery? other)
        {
            return Equals((object?)other);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Query)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return components.GetHashCode();
        }
    }
}