using QueryNinja.Core;
using System;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Appends <paramref name="query"/> to the <paramref name="queryable"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<T> WithQuery<T>(this IQueryable<T> queryable, IQuery query)
        {
            foreach (var component in query.GetComponents())
            {
                //todo: logarithmic or constant time should be here.
                var builder = QueryNinjaExtensions
                    .Extensions<IQueryBuilder>()
                    .FirstOrDefault(item => item.CanAppend(component));

                if (builder == null)
                {
                    //todo: custom exception here
                    throw new NotSupportedException("Query component has no matching descriptors");
                }

                queryable = builder.Append(queryable, component);
            }

            return queryable;
        }
    }

}
