using System;
using System.Collections.Generic;
using NUnit.Framework;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Sources.AspNetCore.Factory;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(DefaultFilterFactory))]
    public class DefaultFilterFactoryTests
    {
        public static IEnumerable<TestCaseData> SuccessCases = new[]
        {
            new TestCaseData("filters.Property.Test", "0", typeof(TestFilter), TestOperations.Test),
            new TestCaseData("filters.Property.Equals", "0", typeof(ComparisonFilter), ComparisonOperation.Equals),
            new TestCaseData("filters.Property.Contains", "0", typeof(CollectionFilter), CollectionOperation.Contains)
        };
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            //enables dynamic factory building for this Filter
            QueryNinjaExtensions.Configure.RegisterComponent<TestFilter>();
        }

        [Test]
        [TestCaseSource(nameof(SuccessCases))]
        public void TestApply(string name, string value, Type expectedFilterType, Enum expectedEnum)
        {
            var factory = new DefaultFilterFactory();

            var canApply = factory.CanApply(name, value);

            Assert.True(canApply);

            var queryComponent = factory.Create(name, value);

            Assert.IsInstanceOf(expectedFilterType, queryComponent);

            var filter = (dynamic) queryComponent;

            var range = (name.IndexOf('.') + 1) .. name.LastIndexOf('.');
            var property = name.AsSpan()[range].ToString();

            Assert.AreEqual(filter.Operation, expectedEnum);
            Assert.AreEqual(filter.Property, property);
            Assert.AreEqual(filter.Value, value);
        }


        public class TestFilter : IDefaultFilter<TestOperations>
        {
            public TestFilter(TestOperations operation, string property, string value)
            {
                Operation = operation;
                Property = property;
                Value = value;
            }

            /// <inheritdoc />
            public TestOperations Operation { get; }

            /// <inheritdoc />
            public string Property { get; }

            /// <inheritdoc />
            public string Value { get; }
        }

        public enum TestOperations
        {
            Test = 1
        }
    }
}