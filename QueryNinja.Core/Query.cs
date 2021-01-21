using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core
{

    /// <summary>
    /// Represent default query class with filters and ordering rules. <br/>
    /// </summary>
    public sealed class Query : IQuery
    {
        private readonly ICollection<IQueryComponent> components;

        /// <summary>
        /// Creates instance of <see cref="Query"/> with defined query components.
        /// </summary>
        /// <param name="components"></param>
        public Query(ICollection<IQueryComponent> components)
        {
            this.components = components;
        }

        /// <summary>
        /// Represent a collection of filters to be applied on Target collection. <br/>
        /// All filters are join with AND operator.
        /// </summary>
        public ICollection<IFilter> Filters => components.OfType<IFilter>().ToList();

        /// <summary>
        /// Represent a collection of order rules to be applied on Target collection. <br/>
        /// </summary>
        public ICollection<OrderingRule> OrderingRules => components.OfType<OrderingRule>().ToList();

        ///<inheritdoc/>
        public IEnumerable<IQueryComponent> GetComponents()
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
    }
}
