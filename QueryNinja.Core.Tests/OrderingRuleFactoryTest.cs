using System;
using System.Collections.Generic;
using NUnit.Framework;
using QueryNinja.Core.Factories;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Core.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(OrderingRuleSerializer))]
    public class OrderingRuleFactoryTest
    {
        public static IEnumerable<TestCaseData> SuccessCases = new[]
        {
            new TestCaseData("order.Property", "Ascending", OrderDirection.Ascending),
            new TestCaseData("order.Property", "Descending", OrderDirection.Descending)
        };

        [Test]
        [TestCaseSource(nameof(SuccessCases))]
        public void TestApply(string path, string value, OrderDirection expectedEnum)
        {
            var factory = new OrderingRuleSerializer();

            var canApply = factory.CanDeserialize(path, value);

            Assert.True(canApply);

            var queryComponent = factory.Deserialize(path, value);

            Assert.IsInstanceOf(typeof(OrderingRule), queryComponent);

            var orderingRule = (OrderingRule)queryComponent;
            var property = path.Split('.')[1];

            Assert.AreEqual(expectedEnum, orderingRule.Direction);
            Assert.AreEqual(property, orderingRule.Property);
        }
    }
}