using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class OrderQueryBuilder : AbstractQueryBuilder<OrderingRule>
    {
        private static readonly MethodInfo OrderBy = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "OrderBy" && methodInfo.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescending = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "OrderByDescending" && methodInfo.GetParameters().Length == 2);

        private static readonly MethodInfo ThenBy = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "ThenBy" && methodInfo.GetParameters().Length == 2);

        private static readonly MethodInfo ThenByDescending = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(methodInfo => methodInfo.Name == "ThenByDescending" && methodInfo.GetParameters().Length == 2);

        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            OrderingRule order)
        {
            var method = order.Direction switch
            {
                OrderDirection.Ascending => source.IsOrderExpressionDefined() ? ThenBy : OrderBy,
                OrderDirection.Descending => source.IsOrderExpressionDefined() ? ThenByDescending : OrderByDescending,
                _ => throw new ArgumentOutOfRangeException(nameof(order), "Direction is outside Enum")
            };

            var lambda = order.Property.From<TEntity>();

            //can't introduce FastReflection as lambda.Return type can contain lots of values
            //this will multiply cache dictionary in many times.
            var genericOrderMethod = method.MakeGenericMethod(typeof(TEntity), lambda.ReturnType);

            var queryBody = Expression.Call(genericOrderMethod, source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }
    }
}