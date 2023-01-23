using System;
using System.Collections.Generic;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core.Factories
{
    internal class OrderingRuleSerializer : AbstractComponentExtension<OrderingRule>, IQueryComponentSerializer
    {
        public bool CanDeserialize(ReadOnlySpan<char> path, string value)
        {
            return path.StartsWith("order", StringComparison.OrdinalIgnoreCase) &&
                   typeof(OrderDirection).IsEnumDefined(value);
        }

        public IQueryComponent Deserialize(ReadOnlySpan<char> path, string value)
        {
            var direction = Enum.Parse<OrderDirection>(value);

            var firstDot = path.IndexOf('.');

            var property = path[(firstDot + 1)..];

            return new OrderingRule(property.ToString(), direction);
        }

        public bool CanSerialize(IQueryComponent component)
        {
            return component is OrderingRule;
        }

        public KeyValuePair<string, string> Serialize(IQueryComponent component)
        {
            if (component is not OrderingRule rule)
            {
                throw new InvalidOperationException($"{nameof(component)} has to be of type OrderingRule");
            }

            return new KeyValuePair<string, string>($"order.{rule.Property}", rule.Direction.ToString());
        }
    }
}