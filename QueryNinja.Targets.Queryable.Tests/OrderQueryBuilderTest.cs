using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(OrderQueryBuilder))]
    public class OrderQueryBuilderTest
    {
        public static IEnumerable<TestCaseData> SuccessCases = new List<TestCaseData>
        {
            new TestCaseData(
                    new Example[] {1, 3, 5, 9, 7, 2, 8, 6, 4}.AsQueryable().OrderBy(a => true),
                    new OrderingRule("Value", OrderDirection.Ascending))
                .Returns(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}),
            new TestCaseData(
                    new Example[] {1, 3, 5, 9, 7, 2, 8, 6, 4}.AsQueryable().OrderBy(a => true),
                    new OrderingRule("Value", OrderDirection.Descending))
                .Returns(new[] {9, 8, 7, 6, 5, 4, 3, 2, 1}),
            new TestCaseData(
                    new Example[] {1, 3, 5, 9, 7, 2, 8, 6, 4}.AsQueryable(),
                    new OrderingRule("Value", OrderDirection.Ascending))
                .Returns(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}),
            new TestCaseData(
                    new Example[] {1, 3, 5, 9, 7, 2, 8, 6, 4}.AsQueryable(),
                    new OrderingRule("Value", OrderDirection.Descending))
                .Returns(new[] {9, 8, 7, 6, 5, 4, 3, 2, 1})
        };

        public static IEnumerable<TestCaseData> FailedCases = new List<TestCaseData>
        {
            new TestCaseData(
                new Example[] {1, 3, 5, 9, 7, 2, 8, 6, 4}.AsQueryable().OrderBy(a => true),
                new OrderingRule("Value", (OrderDirection) 12),
                typeof(QueryBuildingException))
        };


        [Test]
        [TestCaseSource(nameof(SuccessCases))]
        public IEnumerable<int> AppendTestOnIntegers(IQueryable<Example> source, OrderingRule orderRule)
        {
            var builder = new OrderQueryBuilder();

            var result = builder.Append(source, orderRule);

            return result.AsEnumerable().Select(value => value.Value);
        }

        [Test]
        [TestCaseSource(nameof(FailedCases))]
        public void FailedAppendTestOnIntegers(IQueryable<Example> source, OrderingRule orderRule, Type exceptionType)
        {
            var builder = new OrderQueryBuilder();

            Assert.Throws(exceptionType, () => builder.Append(source, orderRule));
        }

        public class Example
        {
            public int Value { get; }

            public Example(int value)
            {
                Value = value;
            }

            public static implicit operator Example(int value)
            {
                return new(value);
            }
        }
    }
}