using System;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    /// Extends <see cref="IExtensionsSettings"/> with ability to define <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}"/> for custom filters.
    /// </summary>
    public interface IQueryableExtensionsSettings : IExtensionsSettings
    {
        /// <summary>
        /// Registers instance of <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}"/> with configured operations.
        /// </summary>
        /// <param name="configure">Allows to configure <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}"/>.</param>
        /// <typeparam name="TFilter">User-defined filters.</typeparam>
        /// <typeparam name="TOperation">Operations enum for user-defined filters.</typeparam>
        public IQueryableExtensionsSettings AddFilter<TFilter, TOperation>(Action<DefaultFilterQueryBuilder<TFilter, TOperation>> configure)
            where TFilter : IDefaultFilter<TOperation>
            where TOperation : Enum;
    }
}