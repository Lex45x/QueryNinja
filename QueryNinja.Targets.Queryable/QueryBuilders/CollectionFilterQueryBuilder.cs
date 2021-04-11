using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryNinja.Core;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class CollectionFilterQueryBuilder : AbstractQueryBuilder<CollectionFilter>
    {
        private static readonly MethodInfo Where = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "Where" && methodInfo.GetParameters()
                .Last()
                .ParameterType.GetGenericArguments()
                .Last()
                .GetGenericArguments()
                .Length == 2);

        private static readonly MethodInfo ContainsMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "Contains" && methodInfo.GetParameters().Length == 2);

        private static readonly MethodInfo AnyMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "Any" && methodInfo.GetParameters().Length == 1);

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

            var genericWhere = Where.MakeGenericMethod(typeof(TEntity));

            var queryBody = Expression.Call(genericWhere,
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }

        private static Expression IsEmpty(Expression property, Expression constant, Type elementType)
        {
            var any = AnyMethod.MakeGenericMethod(elementType);

            var anyCall = Expression.Call(any, property);

            return Expression.Equal(anyCall, Expression.Not(constant));
        }

        private static Expression Contains(Expression property, Expression constant)
        {
            var contains = ContainsMethod.MakeGenericMethod(constant.Type);
            return Expression.Call(contains, property, constant);
        }
    }
}