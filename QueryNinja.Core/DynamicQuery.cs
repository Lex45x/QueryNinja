using System.Collections.Generic;
using QueryNinja.Core.Projection;

namespace QueryNinja.Core
{
    /// <inheritdoc cref="IDynamicQuery" />
    public class DynamicQuery : Query, IDynamicQuery
    {
        private readonly IReadOnlyList<ISelector> selectors;

        /// <inheritdoc />
        public DynamicQuery(IReadOnlyList<IQueryComponent> components, IReadOnlyList<ISelector> selectors)
            : base(components)
        {
            this.selectors = selectors;
        }

        /// <inheritdoc />
        public IReadOnlyList<ISelector> GetSelectors()
        {
            return selectors;
        }
    }

    /// <inheritdoc cref="IDynamicQuery{TEntity}" />
    public class DynamicQuery<TEntity> : DynamicQuery, IDynamicQuery<TEntity>
    {
        /// <inheritdoc />
        public DynamicQuery(IReadOnlyList<IQueryComponent> components, IReadOnlyList<ISelector> selectors)
            : base(components, selectors)
        {
        }
    }
}