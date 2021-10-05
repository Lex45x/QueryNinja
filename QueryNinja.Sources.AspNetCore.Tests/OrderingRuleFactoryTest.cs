using System;
using System.Collections.Generic;
using NUnit.Framework;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Sources.AspNetCore.Factory;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(OrderingRuleFactory))]
    public class OrderingRuleFactoryTest
    {
        public static IEnumerable<TestCaseData> SuccessCases = new[]
        {
            new TestCaseData("order.Property", "Ascending", OrderDirection.Ascending),
            new TestCaseData("order.Property", "Descending", OrderDirection.Descending)
        };

        [Test]
        [TestCaseSource(nameof(SuccessCases))]
        public void TestApply(string name, string value, OrderDirection expectedEnum)
        {
            var factory = new OrderingRuleFactory();

            var canApply = factory.CanApply(name, value);

            Assert.True(canApply);

            var queryComponent = factory.Create(name, value);

            Assert.IsInstanceOf(typeof(OrderingRule), queryComponent);

            var orderingRule = (OrderingRule) queryComponent;

            var range = (name.IndexOf(value: '.') + 1)..;
            var property = name.AsSpan()[range].ToString();

            Assert.AreEqual(orderingRule.Direction, expectedEnum);
            Assert.AreEqual(orderingRule.Property, property);
        }
    }
}