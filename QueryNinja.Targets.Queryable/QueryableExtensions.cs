using System;
using System.Collections.Generic;
using QueryNinja.Core;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.Projection;
using QueryNinja.Targets.Queryable.QueryBuilders;
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable LoopCanBeConvertedToQuery

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    /// Extensions that allow to use QueryNinja on <see cref="IQueryable{T}"/> interface.
    /// </summary>
    public static class QueryableExtensions
    {
        private static readonly IReadOnlyDictionary<Type, ITypedQueryBuilder> QueryBuilders;

        static QueryableExtensions()
        {
            QueryBuilders = QueryNinjaExtensions
                .Extensions<ITypedQueryBuilder>()
                .ToDictionary(builder => builder.ComponentType);
        }


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
            var components = query.GetComponents();
            for (var componentIndex = 0; componentIndex < components.Count; componentIndex++)
            {
                var component = components[componentIndex];
                // for the typed query builders we have a quicker execution path with constant access time.
                if (QueryBuilders.TryGetValue(component.GetType(), out var typedBuilder))
                {
                    queryable = typedBuilder.Append(queryable, component);
                }
                else
                {
                    IQueryBuilder? builder = null;
                    var list = QueryNinjaExtensions.Extensions<IQueryBuilder>();
                    for (var builderIndex = 0; builderIndex < list.Count; builderIndex++)
                    {
                        var item = list[builderIndex];

                        if (!item.CanAppend(component))
                        {
                            continue;
                        }

                        builder = item;
                        break;
                    }

                    if (builder == null)
                    {
                        throw new NoMatchingExtensionsException(component,
                            QueryNinjaExtensions.Extensions<IQueryBuilder>());
                    }

                    queryable = builder.Append(queryable, component);
                }
            }

            return queryable;
        }

        /// <summary>
        /// Appends <paramref name="query"/> to the <paramref name="queryable"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <exception cref="NoMatchingExtensionsException">When <paramref name="query"/> contains component that no <see cref="IQueryBuilder"/> can append.</exception>
        /// <returns></returns>
        public static IQueryable<T> WithQuery<T>(this IQueryable<T> queryable, IQuery<T> query)
        {
            return WithQuery(queryable, (IQuery) query);
        }

        /// <summary>
        /// Appends <paramref name="query"/> to the <paramref name="queryable"/> and allows to project <see cref="T"/> into dynamic object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<dynamic> WithQuery<T>(this IQueryable<T> queryable, IDynamicQuery query)
        {
            var withQuery = WithQuery(queryable, query as IQuery);

            return withQuery.Project(query.GetSelectors());
        }

        /// <summary>
        /// Appends <paramref name="query"/> to the <paramref name="queryable"/> and allows to project <see cref="T"/> into dynamic object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable<dynamic> WithQuery<T>(this IQueryable<T> queryable, IDynamicQuery<T> query)
        {
            return WithQuery(queryable, (IDynamicQuery) query);
        }
    }
}