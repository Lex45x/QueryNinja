using System.Linq.Expressions;

namespace QueryNinja.Targets.Queryable.QueryBuilders
{
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