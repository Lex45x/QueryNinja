using System.Collections.Generic;
using QueryNinja.Core.Projection;

namespace QueryNinja.Core
{
    /// <summary>
    /// Represent default query class with filters, ordering rules and selectors. <br/>
    /// </summary>
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
}