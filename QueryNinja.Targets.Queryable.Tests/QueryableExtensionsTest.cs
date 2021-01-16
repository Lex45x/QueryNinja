using NUnit.Framework;
using QueryNinja.Core;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Integration", TestOf = typeof(QueryableExtensions))]
    public class QueryableExtensionsTest
    {
        private static readonly IQueryable<Example> sourceData = new[]
        {
            new Example(1, "First", 15),
            new Example(2, "Second", 21),
            new Example(3, "Third", 73),
            new Example(4, "Fourth", 90),
            new Example(5, "Fifth", 45),
            new Example(6, "Sixth", 22),
            new Example(7, "Seventh", 65),
            new Example(8, "Eighth", 34),
            new Example(9, "Ninth", 8)
        }.AsQueryable();

        public static IEnumerable<TestCaseData> Scenarios = new List<TestCaseData>
        {
            new TestCaseData(sourceData,
                new Query(
                    new []
                    {
                        new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "5"),
                        new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "IntValue", "10")
                    },
                    new []
                    {
                        new OrderingRule("Id", OrderDirection.Ascending)
                    }))
            .Returns(new []{ 1, 3, 5, 6 }),
            new TestCaseData(sourceData,
                new Query(
                    new []
                    {
                        new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "6"),
                        new ComparisonFilter(ComparisonOperation.Less, "IntValue", "40")
                    },
                    new []
                    {
                        new OrderingRule("StringValue", OrderDirection.Ascending)
                    }))
            .Returns(new []{ 8, 2 })
        };


        [OneTimeSetUp]
        public void SetupExtensions()
        {
            QueryNinjaExtensions.Configure.WithQueryableTarget();
        }

        [Test]
        [TestCaseSource(nameof(Scenarios))]
        public IEnumerable<int> ScenariosTest(IQueryable<Example> source, IQuery query)
        {
            return source.WithQuery(query).Select(example => example.Id);
        }

        public class Example
        {
            public Example(int id, string stringValue, int intValue)
            {
                Id = id;
                StringValue = stringValue;
                IntValue = intValue;
            }

            public int Id { get; }
            public string StringValue { get; }
            public int IntValue { get; }
        }
    }
}
