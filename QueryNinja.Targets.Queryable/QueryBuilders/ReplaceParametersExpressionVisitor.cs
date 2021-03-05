using System.Linq.Expressions;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
    /// <summary>
    /// This Expression Visitor is used to replace parameters in expression tree with predefined property and constant expressions. <br/>
    /// This functinality is used by <see cref="DefaultFilterQueryBuilder{TFilter, TOperation}"/> to allow define expression in simplified way.
    /// </summary>
    internal class ReplaceParametersExpressionVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression propertyParameter;
        private readonly Expression property;
        private readonly ParameterExpression constantParameter;
        private readonly Expression constant;
        
        public ReplaceParametersExpressionVisitor(ParameterExpression propertyParameter, Expression property, ParameterExpression constantParameter, Expression constant)
        {
            this.propertyParameter = propertyParameter;
            this.property = property;
            this.constantParameter = constantParameter;
            this.constant = constant;
        }

        /// <inheritdoc />
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == propertyParameter)
            {
                return property;
            }

            if (node == constantParameter)
            {
                return constant;
            }

            return base.VisitParameter(node);
        }
    }
}