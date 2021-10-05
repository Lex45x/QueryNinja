using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Reflection;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    /// <summary>
    ///   Allows to configure <see cref="IDefaultFilter{TOperation}" /> in simplified way.
    /// </summary>
    /// <typeparam name="TOperation"></typeparam>
    /// <typeparam name="TFilter"></typeparam>
    public class DefaultFilterQueryBuilder<TFilter, TOperation> : AbstractQueryBuilder<TFilter>
        where TFilter : IDefaultFilter<TOperation>
        where TOperation : Enum
    {
        private readonly Dictionary<TOperation, OperationBuilder> operations =
            new Dictionary<TOperation, OperationBuilder>();

        /// <summary>
        ///   Allows to define <paramref name="operation" /> on <typeparamref name="TTarget" /> property type. <br />
        /// </summary>
        /// <typeparam name="TTarget">Supported type of property.</typeparam>
        /// <param name="operation">Desired operation to define.</param>
        /// <param name="expression">
        ///   Expression that defines <paramref name="operation" /> on the property of type
        ///   <typeparamref name="TTarget" />
        /// </param>
        /// <returns>Configured instance of <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}" /></returns>
        public DefaultFilterQueryBuilder<TFilter, TOperation> Define<TTarget>(TOperation operation,
            Expression<Func<TTarget, TTarget, bool>> expression)
        {
            return Define<TTarget, TTarget>(operation, expression);
        }

        /// <summary>
        ///   Allows to define <paramref name="operation" /> on <typeparamref name="TTarget" /> property type with different
        ///   constant type of <typeparamref name="TValue" /><br />
        /// </summary>
        /// <typeparam name="TTarget">Supported type of property.</typeparam>
        /// <typeparam name="TValue">Supported type of the value.</typeparam>
        /// <param name="operation">Desired operation to define.</param>
        /// <param name="expression">
        ///   Expression that defines <paramref name="operation" /> on the property of type
        ///   <typeparamref name="TTarget" />
        /// </param>
        /// <returns>Configured instance of <see cref="DefaultFilterQueryBuilder{TFilter,TOperation}" /></returns>
        public DefaultFilterQueryBuilder<TFilter, TOperation> Define<TTarget, TValue>(TOperation operation,
            Expression<Func<TTarget, TValue, bool>> expression)
        {
            var parameters = expression.Parameters;

            var expressionBody = expression.Body;

            Expression CreateBody(Expression property, Expression constant)
            {
                var expressionVisitor =
                    new ReplaceParametersExpressionVisitor(parameters[index: 0], property, parameters[index: 1],
                        constant);

                return expressionVisitor.Visit(expressionBody) ??
                       throw new InvalidOperationException("Visit returned null");
            }

            var operationBuilder = new OperationBuilder(typeof(TTarget), typeof(TValue), CreateBody);
            operations[operation] = operationBuilder;

            return this;
        }

        /// <inheritdoc />
        protected override IQueryable<TEntity> AppendImplementation<TEntity>(IQueryable<TEntity> source,
            TFilter component)
        {
            if (!operations.TryGetValue(component.Operation, out var operationBuilder))
            {
                throw new NotSupportedException("Operation is not defined!");
            }

            var propertyLambda = component.Property.From<TEntity>();

            if (!operationBuilder.TargetType.IsAssignableFrom(propertyLambda.ReturnType))
            {
                throw new InvalidOperationException(
                    $"Type of property {component.Property} should be assignable to {operationBuilder.TargetType}");
            }

            var constant = component.Value.AsConstant(operationBuilder.ConstantType);

            var body = operationBuilder.CreateBody(propertyLambda.Body, constant);

            var filterExpression = Expression.Lambda(body, propertyLambda.Parameters);

            var genericWhere = FastReflection.ForQueryable<TEntity>.Where();

            var queryBody = Expression.Call(genericWhere,
                source.Expression, Expression.Quote(filterExpression));

            return source.Provider.CreateQuery<TEntity>(queryBody);
        }

        private class OperationBuilder
        {
            private readonly Func<Expression, Expression, Expression> bodyFactory;

            public OperationBuilder(Type targetType, Type constantType,
                Func<Expression, Expression, Expression> bodyFactory)
            {
                this.bodyFactory = bodyFactory;
                TargetType = targetType;
                ConstantType = constantType;
            }

            public Type TargetType { get; }
            public Type ConstantType { get; }

            public Expression CreateBody(Expression property, Expression constant)
            {
                return bodyFactory(property, constant);
            }
        }
    }
}