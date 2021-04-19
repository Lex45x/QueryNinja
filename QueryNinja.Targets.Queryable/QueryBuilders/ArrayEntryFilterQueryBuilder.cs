using System;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.Reflection;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class ArrayEntryFilterQueryBuilder : AbstractQueryBuilder<ArrayEntryFilter>
    {
        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            ArrayEntryFilter component)
        {
            var propertyLambda = component.Property.From<TEntity>();

            var arrayConstant = CreateArrayConstant(propertyLambda.ReturnType, component.Value);

            var body = component.Operation switch
            {
                ArrayEntryOperations.In => In(propertyLambda.Body, arrayConstant),
                _ => throw new ArgumentOutOfRangeException(nameof(CollectionFilter.Operation))
            };

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var genericWhere = FastReflection.ForQueryable<TEntity>.Where();

            var queryBody = Expression.Call(genericWhere,
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }

        private static Expression CreateArrayConstant(Type propertyType, string constant)
        {
            var arrayElements = constant.Split(separator: '|');
            var arrayElementExpressions = new Expression[arrayElements.Length];
            
            for (var arrayIndex = 0; arrayIndex < arrayElements.Length; arrayIndex++)
            {
                arrayElementExpressions[arrayIndex] = arrayElements[arrayIndex].AsConstant(propertyType);
            }
            
            var newArrayExpression = Expression.NewArrayInit(propertyType, arrayElementExpressions);
            
            return newArrayExpression;
        }

        private static Expression In(Expression property, Expression arrayConstant)
        {
            var methodInfo = FastReflection.ForEnumerable.Contains(property.Type);
            return Expression.Call(methodInfo, arrayConstant, property);
        }
    }
}