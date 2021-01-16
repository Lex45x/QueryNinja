using QueryNinja.Core;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    /// Extensions that allow to use QueryNinja on <see cref="IQueryable{T}"/> interface.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Appends <paramref name="query"/> to the <paramref name="queryable"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <exception cref="NoMatchingExtensionsException">When <paramref name="query"/> contains component that no <see cref="IQueryBuilder"/> can append.</exception>
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
                    throw new NoMatchingExtensionsException(component,
                        QueryNinjaExtensions.Extensions<IQueryBuilder>().ToList());
                }

                queryable = builder.Append(queryable, component);
            }

            return queryable;
        }
    }
}