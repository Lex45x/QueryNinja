using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core
{
    /// <inheritdoc cref="IQuery" />
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
    }

    /// <inheritdoc cref="IQuery{TEntity}" />
    public class Query<TEntity> : Query, IQuery<TEntity>
    {
        /// <inheritdoc />
        public Query(IReadOnlyList<IQueryComponent> components)
            : base(components)
        {
        }
    }
}