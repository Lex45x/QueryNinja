using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraphQLParser.AST;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using QueryNinja.Core;
using QueryNinja.Sources.GraphQL.Introspection;
using QueryNinja.Sources.GraphQL.Serializers;
using QueryNinja.Targets.Queryable;

namespace QueryNinja.Sources.GraphQL.Middleware
{

    internal class NullableFriendlyVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object == null)
            {
                var arguments = node.Arguments.Select(expression => base.Visit(expression)).ToList();
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

    internal class IntrospectionQueryHandler : IGraphQLQueryHandler
    {
        private readonly IQuerySerializer<IDynamicQuery> serializer;
        private readonly IQueryable<IntrospectionModel> introspectionSource;

        public IntrospectionQueryHandler(__Schema schema, IQuerySerializer<IDynamicQuery> serializer)
        {
            this.serializer = serializer;
            introspectionSource = Enumerable.Repeat(new IntrospectionModel(schema), count: 1).AsQueryable();
        }

        /// <inheritdoc />
        public Task<object> Handle(HttpContext context, GraphQLDocument document, JObject variables)
        {
            var query = serializer.Deserialize(document);

            //source should tolerate selects from null
            var queryable = introspectionSource.WithQuery(query);

            var visitor = new NullableFriendlyVisitor();

            var resultQueryable = queryable.Provider.CreateQuery<object>(visitor.Visit(queryable.Expression));
            
            var result = resultQueryable.FirstOrDefault();

            return Task.FromResult(result);
        }
    }
}