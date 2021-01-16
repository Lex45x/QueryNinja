using System;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class CollectionFilterQueryBuilder : AbstractQueryBuilder<CollectionFilter>
    {
        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source, CollectionFilter defaultFilter)
        {
            var propertyLambda = defaultFilter.Property.From<TEntity>();

            var collectionInterface = propertyLambda.ReturnType.GetInterface("IEnumerable`1");

            if (collectionInterface == null)
            {
                throw new PropertyIsNotCollectionException(defaultFilter.Property, typeof(TEntity));
            }

            var elementType = collectionInterface.GetGenericArguments().First();

            var body = defaultFilter.Operation switch
            {
                CollectionOperation.Contains => Contains(propertyLambda.Body, defaultFilter.Value.AsConstant(elementType)),
                CollectionOperation.IsEmpty => IsEmpty(propertyLambda.Body, defaultFilter.Value.AsConstant(typeof(bool)), elementType),
                _ => throw new ArgumentOutOfRangeException(nameof(CollectionFilter.Operation))
            };

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var queryBody = Expression.Call(typeof(System.Linq.Queryable),
                "Where",
                new[]
                {
                    typeof(TEntity)
                },
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }

        private static Expression IsEmpty(Expression property, Expression constant, Type elementType)
        {
            var anyCall = Expression.Call(typeof(Enumerable),
                "Any",
                new[]
                {
                    elementType
                },
                property);

            return Expression.Equal(anyCall, Expression.Not(constant));
        }

        private static Expression Contains(Expression property, Expression constant)
        {
            return Expression.Call(typeof(Enumerable),
                "Contains",
                new[]
                {
                    constant.Type
                },
                property, constant);
        }
    }
}
