using System;
using System.Linq;
using System.Linq.Expressions;

namespace QueryNinja.Sources.GraphQL.Middleware
{
    /// <summary>
    /// Rebuilds given expression with null-handling for Linq calls and Dictionary construction.
    /// </summary>
    internal class NullableFriendlyVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object == null)
            {
                var arguments = node.Arguments.Select(expression => Visit((Expression?) expression)).ToList();
                var expression = arguments[index: 0];
                var nullBaseExpression = Expression.Default(expression.Type);
                var test = Expression.Equal(expression, nullBaseExpression);
                var memberAccess = Expression.Call(node.Method, arguments);
                var nullMemberExpression = Expression.Default(MakeNullable(node.Type));
                return Expression.Condition(test, nullMemberExpression, memberAccess);
            }
            else
            {
                if (!IsNullable(node.Object.Type))
                {
                    return base.VisitMethodCall(node);
                }

                var expression = base.Visit(node.Object);
                var nullBaseExpression = Expression.Default(expression.Type);
                var test = Expression.Equal(expression, nullBaseExpression);
                var memberAccess = Expression.Call(expression, node.Method, node.Arguments);
                var nullMemberExpression = Expression.Default(MakeNullable(node.Type));
                return Expression.Condition(test, nullMemberExpression, memberAccess);
            }
        }

        /// <inheritdoc />
        protected override Expression VisitListInit(ListInitExpression node)
        {
            //bug: dictionary must contain at least one direct property access.
            if (!(node.Initializers[index: 0].Arguments[index: 1] is UnaryExpression unaryExpression))
            {
                return base.VisitListInit(node);
            }

            if (unaryExpression.Operand is MemberExpression expression)
            {
                var nullResult = Expression.Constant(value: null, node.Type);
                var nullParent = Expression.Default(expression.Expression.Type);
                var test = Expression.Equal(nullParent, expression.Expression);
                return Expression.Condition(test, nullResult, base.VisitListInit(node));
            }

            return base.VisitListInit(node);
        }

        private static Type MakeNullable(Type type)
        {
            if (IsNullable(type))
                return type;

            return typeof(Nullable<>).MakeGenericType(type);
        }

        private static bool IsNullable(Type type)
        {
            if (type.IsClass || type.IsInterface)
                return true;
            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}