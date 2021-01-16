using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace QueryNinja.Targets.Queryable
{
    internal static class ExpressionsExtensions
    {
        /// <summary>
        /// Creates a constant expression of desired type from string value. <br/>
        /// Includes built-in type conversion.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Expression AsConstant(this string value, Type type)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);

            var converted = typeConverter.ConvertFromString(value);

            if (converted == null)
            {
                throw new InvalidOperationException($"Cannot convert string '{value}' to {type}");
            }

            var constantExpression = Expression.Constant(converted, converted.GetType());

            return constantExpression;
        }

        /// <summary>
        /// Takes desired property with <paramref name="path"/> from <typeparamref name="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static LambdaExpression From<TEntity>(this string path)
        {
            var pathSegments = path.Split('.');

            var parameter = Expression.Parameter(typeof(TEntity));

            Expression currentProperty = pathSegments.Aggregate<string, Expression>(parameter, (current, property) => Expression.Property(current, current.Type, property));

            return Expression.Lambda(currentProperty, parameter);
        }

        /// <summary>
        /// Checks whether Queryable already contains order expressions defined.
        /// </summary>
        /// <param name="queryable">Source</param>
        /// <returns></returns>
        public static bool IsOrderExpressionDefined(this IQueryable queryable)
        {
            var methodCall = queryable.Expression as MethodCallExpression;

            while (methodCall != null)
            {
                if (methodCall.Method.Name.Contains("OrderBy"))
                {
                    return true;
                }

                methodCall = methodCall.Arguments.OfType<MethodCallExpression>().FirstOrDefault();
            }

            return false;
        }
    }

}
