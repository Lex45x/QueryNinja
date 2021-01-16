using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Sources.AspNetCore.Factory
{
    internal class OrderingRuleFactory : AbstractComponentExtension<OrderingRule>, IQueryComponentFactory
    {
        public bool CanApply(string name, string value)
        {
            return name.StartsWith("order", StringComparison.OrdinalIgnoreCase) &&
                   typeof(OrderDirection).IsEnumDefined(value);
        }

        public IQueryComponent Create(string name, string value)
        {
            var segments = name.AsSpan();
            var firstDot = segments.IndexOf(value: '.');
            var property = segments.Slice(firstDot + 1).ToString();
            var direction = Enum.Parse<OrderDirection>(value);

            return new OrderingRule(property, direction);
        }
    }
}