using System;
using System.Collections.Generic;
using NUnit.Framework;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Factories;
using QueryNinja.Core.Filters;

namespace QueryNinja.Core.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(DefaultFilterSerializer))]
    public class DefaultFilterFactoryTests
    {
        public static IEnumerable<TestCaseData> SuccessCases = new[]
        {
            new TestCaseData("filter.Property.Test", "0", typeof(TestFilter), TestOperations.Test),
            new TestCaseData("filter.Property.Equals", "0", typeof(ComparisonFilter), ComparisonOperation.Equals),
            new TestCaseData("filter.Property.Contains", "0", typeof(CollectionFilter), CollectionOperation.Contains)
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            //enables dynamic factory building for this Filter
            QueryNinjaExtensions.Configure.RegisterComponent<TestFilter>();
        }

        [Test]
        [TestCaseSource(nameof(SuccessCases))]
        public void TestApply(string path, string value, Type expectedFilterType, Enum expectedEnum)
        {
            var factory = new DefaultFilterSerializer();

            var canApply = factory.CanDeserialize(path, value);

            Assert.True(canApply);

            var queryComponent = factory.Deserialize(path, value);

            Assert.IsInstanceOf(expectedFilterType, queryComponent);

            var filter = (dynamic)queryComponent;

            var range = (path.IndexOf(value: '.') + 1)..path.LastIndexOf(value: '.');
            var property = path.AsSpan()[range].ToString();

            Assert.AreEqual(filter.Operation, expectedEnum);
            Assert.AreEqual(filter.Property, property);
            Assert.AreEqual(filter.Value, value);
        }


        private class TestFilter : AbstractDefaultFilter<TestOperations>
        {
            public TestFilter(TestOperations operation, string property, string value)
                : base(operation, property, value)
            {
            }
        }

        public enum TestOperations
        {
            Test = 1
        }
    }
}