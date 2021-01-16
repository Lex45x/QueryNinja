using System;
using System.Linq;
using QueryNinja.Core;
using QueryNinja.Core.Exceptions;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    /// <summary>
    /// Provides generic implementation for <see cref="IQueryBuilder"/> using specific <typeparamref name="TComponent"/> type.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public abstract class AbstractQueryBuilder<TComponent> : AbstractComponentExtension<TComponent>, IQueryBuilder
        where TComponent : IQueryComponent
    {
        ///<inheritdoc/>
        public virtual bool CanAppend(IQueryComponent component)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            return component.GetType() == typeof(TComponent);
        }

        ///<inheritdoc/>
        public IQueryable<TEntity> Append<TEntity>(IQueryable<TEntity> source, IQueryComponent component)
        {
            return Append(source, (TComponent) component);
        }

        protected abstract IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            TComponent filter);

        /// <summary>
        /// Provides generic <see cref="Append{TEntity}(IQueryable{TEntity},IQueryComponent)"/> implementation for specific component type.<br/>
        /// Contains generic error handling.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <exception cref="QueryBuildingException">Thrown in case of any unexpected exception.</exception>
        /// <returns></returns>
        public IQueryable<TEntity> Append<TEntity>(IQueryable<TEntity> source, TComponent filter)
        {
            try
            {
                return AppendImplementation(source, filter);
            }
            catch (QueryNinjaException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new QueryBuildingException(GetType(), e);
            }
        }
    }
}