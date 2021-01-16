using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core
{

    /// <summary>
    /// Represent default query class with filters and ordering rules.
    /// </summary>
    public sealed class Query : IQuery
    {
        public Query(ICollection<IFilter> filters, ICollection<OrderingRule> orderingRules)
        {
            Filters = filters;
            OrderingRules = orderingRules;
        }

        /// <summary>
        /// Represent a collection of filters to be applied on Target collection. <br/>
        /// All filters are join with AND operator.
        /// </summary>
        public ICollection<IFilter> Filters { get; }

        /// <summary>
        /// Represent a collection of order rules to be applied on Target collection. <br/>
        /// </summary>
        public ICollection<OrderingRule> OrderingRules { get; }

        ///<inheritdoc/>
        public IEnumerable<IQueryComponent> GetComponents()
        {
            return Filters.Cast<IQueryComponent>().Concat(OrderingRules);
        }

        public override string ToString()
        {
            var result = GetComponents()
                .Aggregate(new StringBuilder(), (builder, component) => builder.Append($"| {component} |"))
                .ToString();
            return result;
        }
    }
}
