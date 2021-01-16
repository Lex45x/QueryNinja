using System;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Sources.AspNetCore.Factory
{
    public class OrderingRuleFactory : AbstractComponentExtension<OrderingRule>, IQueryComponentFactory
    {
        public bool CanApply(string name, string value)
        {
            if (!name.StartsWith("order", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return typeof(OrderDirection).IsEnumDefined(value);
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