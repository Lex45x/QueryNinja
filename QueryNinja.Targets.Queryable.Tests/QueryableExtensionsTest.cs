using System;
using NUnit.Framework;
using QueryNinja.Core;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Integration", TestOf = typeof(QueryableExtensions))]
    public class QueryableExtensionsTest
    {
        private static readonly IQueryable<Example> SourceData = new[]
        {
            new Example(id: 1, "First", intValue: 15),
            new Example(id: 2, "Second", intValue: 21),
            new Example(id: 3, "Third", intValue: 73),
            new Example(id: 4, "Fourth", intValue: 90),
            new Example(id: 5, "Fifth", intValue: 45),
            new Example(id: 6, "Sixth", intValue: 22),
            new Example(id: 7, "Seventh", intValue: 65),
            new Example(id: 8, "Eighth", intValue: 34),
            new Example(id: 9, "Ninth", intValue: 8)
        }.AsQueryable();

        public static IEnumerable<TestCaseData> SuccessScenarios = new List<TestCaseData>
        {
            new TestCaseData(SourceData,
                    new Query(
                        new[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "5"),
                            new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "IntValue", "10")
                        },
                        new[]
                        {
                            new OrderingRule("Id", OrderDirection.Ascending)
                        }))
                .Returns(new[] {1, 3, 5, 6}),
            new TestCaseData(SourceData,
                    new Query(
                        new[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "6"),
                            new ComparisonFilter(ComparisonOperation.Less, "IntValue", "40")
                        },
                        new[]
                        {
                            new OrderingRule("StringValue", OrderDirection.Ascending)
                        }))
                .Returns(new[] {8, 2})
        };

        public static IEnumerable<TestCaseData> FailedScenarios = new List<TestCaseData>
        {
            new TestCaseData(SourceData, new UnsupportedQuery(), typeof(NoMatchingExtensionsException))
        };


        [OneTimeSetUp]
        public void SetupExtensions()
        {
            QueryNinjaExtensions.Configure.WithQueryableTarget();
        }

        [Test]
        [TestCaseSource(nameof(SuccessScenarios))]
        public IEnumerable<int> ScenariosTest(IQueryable<Example> source, IQuery query)
        {
            return source.WithQuery(query).Select(example => example.Id);
        }

        [Test]
        [TestCaseSource(nameof(FailedScenarios))]
        public void FailedScenariosTest(IQueryable<Example> source, IQuery query, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => source.WithQuery(query));
        }

        public class UnsupportedQuery : IQuery
        {
            public IEnumerable<IQueryComponent> GetComponents()
            {
                return new[] {new UnknownComponent()};
            }
        }

        public class UnknownComponent : IQueryComponent
        {
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