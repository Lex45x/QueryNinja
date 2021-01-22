using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Examples.AspNetCore.Extensions
{
    public class DatabaseFunctionFilter : IDefaultFilter<DatabaseFunction>
    {
        public DatabaseFunctionFilter(DatabaseFunction operation, string property, string value)
        {
            Operation = operation;
            Property = property;
            Value = value;
        }

        /// <inheritdoc />
        public DatabaseFunction Operation { get; }

        /// <inheritdoc />
        public string Property { get; }

        /// <inheritdoc />
        public string Value { get; }
    }

    public enum DatabaseFunction
    {
        Like = 1
    }

    public class DatabaseFunctionQueryBuilder : AbstractQueryBuilder<DatabaseFunctionFilter>
    {
        /// <inheritdoc />
        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            DatabaseFunctionFilter component)
        {
            if (component.Operation != DatabaseFunction.Like)
            {
                throw new NotSupportedException("Only Like operation is supported");
            }

            var propertyLambda = component.Property.From<TEntity>();

            var constant = component.Value.AsConstant(propertyLambda.ReturnType);

            var efFunctions = Expression.Property(expression: null, typeof(EF), "Functions");

            var body = Expression.Call(
                typeof(DbFunctionsExtensions), 
                "Like", 
                Type.EmptyTypes, 
                efFunctions,
                propertyLambda, 
                constant);

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var queryBody = Expression.Call(typeof(Queryable),
                "Where",
                new[]
                {
                    typeof(TEntity)
                },
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }
    }
}