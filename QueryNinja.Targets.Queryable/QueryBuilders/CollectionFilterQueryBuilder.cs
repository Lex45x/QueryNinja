using System;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.Reflection;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class CollectionFilterQueryBuilder : AbstractQueryBuilder<CollectionFilter>
    {
        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            CollectionFilter component)
        {
            var propertyLambda = component.Property.From<TEntity>();
            var collectionInterface = propertyLambda.ReturnType.GetInterface("IEnumerable`1");

            if (collectionInterface == null)
            {
                throw new PropertyIsNotCollectionException(component.Property, propertyLambda.ReturnType,
                    typeof(TEntity));
            }

            var elementType = collectionInterface.GetGenericArguments().First();

            var body = component.Operation switch
            {
                CollectionOperation.Contains => Contains(propertyLambda.Body, component.Value.AsConstant(elementType)),
                CollectionOperation.IsEmpty => IsEmpty(propertyLambda.Body, component.Value.AsConstant(typeof(bool)),
                    elementType),
                _ => throw new ArgumentOutOfRangeException(nameof(CollectionFilter.Operation))
            };

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var genericWhere = FastReflection.ForQueryable<TEntity>.Where();

            var queryBody = Expression.Call(genericWhere,
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }

        private static Expression IsEmpty(Expression property, Expression constant, Type elementType)
        {
            var any = FastReflection.ForEnumerable.Any(elementType);

            var anyCall = Expression.Call(any, property);

            return Expression.Equal(anyCall, Expression.Not(constant));
        }

        private static Expression Contains(Expression property, Expression constant)
        {
            var contains = FastReflection.ForEnumerable.Contains(constant.Type);
            return Expression.Call(contains, property, constant);
        }
    }
}