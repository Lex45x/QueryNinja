using System;
using QueryNinja.Core;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    /// <summary>
    ///   Introduce type-specific query builder. <br />
    ///   Same as <see cref="IQueryBuilder" />, but <see cref="IQueryBuilder.CanAppend" />
    ///   returns true for <see cref="IQueryComponent" /> of <see cref="ComponentType" />.
    /// </summary>
    public interface ITypedQueryBuilder : IQueryBuilder
    {
        /// <summary>
        ///   Type of <see cref="IQueryComponent" /> supported by this Builder.
        /// </summary>
        public Type ComponentType { get; }
    }
}