using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Sources.AspNetCore.Factory
{
    internal class OrderingRuleFactory : AbstractComponentExtension<OrderingRule>, IQueryComponentFactory
    {
        public bool CanApply(ReadOnlySpan<char> name, string value)
        {
            return name.StartsWith("order", StringComparison.OrdinalIgnoreCase) &&
                   typeof(OrderDirection).IsEnumDefined(value);
        }

        public IQueryComponent Create(ReadOnlySpan<char> name, string value)
        {
            var firstDot = name.IndexOf(value: '.');
            var property = name[(firstDot + 1)..].ToString();
            var direction = Enum.Parse<OrderDirection>(value);

            return new OrderingRule(property, direction);
        }
    }
}