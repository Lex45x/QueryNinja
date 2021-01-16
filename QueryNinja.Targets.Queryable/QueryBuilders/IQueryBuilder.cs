using System.Linq;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    /// <summary>
    /// Extension interface for Queryable Target. <br/>
    /// Appends to <see cref="IQueryable{T}"/> instance of a <see cref="IQueryComponent"/>
    /// </summary>
    public interface IQueryBuilder : IQueryComponentExtension
    {
        /// <summary>
        /// Checks whether current QueryBuilder knows how to append passed <see cref="IQueryComponent"/>.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool CanAppend(IQueryComponent component);

        /// <summary>
        /// Append original <paramref name="source"/> with additional expression depending on <paramref name="component"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        IQueryable<TEntity> Append<TEntity>(IQueryable<TEntity> source, IQueryComponent component);
    }

}
