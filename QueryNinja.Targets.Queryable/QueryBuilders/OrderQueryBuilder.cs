using System;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    internal class OrderQueryBuilder : AbstractQueryBuilder<OrderingRule>
    {
        public override IQueryable<TEntity> Append<TEntity>(IQueryable<TEntity> source, OrderingRule order)
        {
            var methodName = source.IsOrderExpressionDefined() ? "ThenBy" : "OrderBy";

            methodName = order.Direction switch
            {
                OrderDirection.Ascending => methodName,
                OrderDirection.Descending => $"{methodName}Descending",
                _ => throw new ArgumentOutOfRangeException(nameof(order), "Direction is outside Enum")
            };

            var lambda = order.Property.From<TEntity>();

            var queryBody = Expression.Call(typeof(System.Linq.Queryable),
                    methodName,
                    new[]
                    {
                        typeof(TEntity), lambda.ReturnType
                    },
                    source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }
    }

}
