using Microsoft.EntityFrameworkCore;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.EntityFrameworkCore.Filters;
using QueryNinja.Targets.Queryable;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.EntityFrameworkCore
{
    /// <summary>
    /// Allows to register IQueryable Target in <see cref="QueryNinjaExtensions"/>
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Extends <paramref name="settings"/> with Queryable Target and registers default filters.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Settings for Queryable Target</returns>
        public static IQueryableExtensionsSettings WithEntityFrameworkTarget(this IExtensionsSettings settings)
        {
            return settings.WithQueryableTarget()
                .AddFilter<DatabaseFunctionFilter, DatabaseFunction>(AddDatabaseFunctionFilter);
        }

        /// <summary>
        /// Will configure <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}"/> for <see cref="DatabaseFunctionFilter"/>
        /// </summary>
        /// <param name="queryBuilder"></param>
        private static void AddDatabaseFunctionFilter(DefaultFilterQueryBuilder<DatabaseFunctionFilter, DatabaseFunction> queryBuilder)
        {
            queryBuilder.Define<string>(DatabaseFunction.Like,
                (property, value) => EF.Functions.Like(property, value)
            );
        }
    }
}