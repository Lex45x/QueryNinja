using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Reflection;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class ComparisonFilterQueryBuilder : AbstractQueryBuilder<ComparisonFilter>
    {
        private static readonly Dictionary<ComparisonOperation, Func<Expression, Expression, Expression>> Operations =
            new Dictionary<ComparisonOperation, Func<Expression, Expression, Expression>>
            {
                [ComparisonOperation.Equals] = Expression.Equal,
                [ComparisonOperation.NotEquals] = Expression.NotEqual,
                [ComparisonOperation.Greater] = Expression.GreaterThan,
                [ComparisonOperation.GreaterOrEquals] = Expression.GreaterThanOrEqual,
                [ComparisonOperation.Less] = Expression.LessThan,
                [ComparisonOperation.LessOrEquals] = Expression.LessThanOrEqual
            };


        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            ComparisonFilter component)
        {
            var propertyLambda = component.Property.From<TEntity>();

            var constant = component.Value.AsConstant(propertyLambda.ReturnType);

            var body = Operations[component.Operation](propertyLambda.Body, constant);

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var genericWhere = FastReflection.ForQueryable.Where<TEntity>();

            var queryBody = Expression.Call(genericWhere,
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }
    }
}