using System;
using NUnit.Framework;
using QueryNinja.Core;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Core.Projection;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Integration", TestOf = typeof(QueryableExtensions))]
    public class QueryableExtensionsTest
    {
        private static readonly IQueryable<Example> SourceData = new[]
        {
            new Example(id: 1, "First", intValue: 15, nullableIntValue: 15),
            new Example(id: 2, "Second", intValue: 21, nullableIntValue: null),
            new Example(id: 3, "Third", intValue: 73, nullableIntValue: 6),
            new Example(id: 4, "Fourth", intValue: 90, nullableIntValue: 12),
            new Example(id: 5, "Fifth", intValue: 45, nullableIntValue: null),
            new Example(id: 6, "Sixth", intValue: 22, nullableIntValue: 15),
            new Example(id: 7, "Seventh", intValue: 65, nullableIntValue: 33),
            new Example(id: 8, "Eighth", intValue: 34, nullableIntValue: null),
            new Example(id: 9, "Ninth", intValue: 8, nullableIntValue: null)
        }.AsQueryable();

        public static IEnumerable<TestCaseData> SuccessScenarios = new List<TestCaseData>
        {
            new TestCaseData(SourceData,
                    new Query(
                        new IQueryComponent[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "5"),
                            new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "IntValue", "10"),
                            new ComparisonFilter(ComparisonOperation.NotEquals, "NullableIntValue", ""),
                            new OrderingRule("Id", OrderDirection.Ascending)
                        }))
                .Returns(new[] {1, 3, 6}),
            new TestCaseData(SourceData,
                    new Query(
                        new IQueryComponent[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "5"),
                            new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "IntValue", "10"),
                            new ComparisonFilter(ComparisonOperation.Equals, "NullableIntValue", "15"),
                            new OrderingRule("Id", OrderDirection.Ascending)
                        }))
                .Returns(new[] {1, 6}),
            new TestCaseData(SourceData,
                    new Query(
                        new IQueryComponent[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "6"),
                            new ComparisonFilter(ComparisonOperation.Less, "IntValue", "40"),
                            new OrderingRule("StringValue", OrderDirection.Ascending),
                            new ArrayEntryFilter(ArrayEntryOperations.In, "Id", "8|2|15")
                        }))
                .Returns(new[] {8, 2}),
            new TestCaseData(SourceData,
                    new Query(
                        new IQueryComponent[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "6"),
                            new ArrayEntryFilter(ArrayEntryOperations.In, "Id", "1|2|3"),
                            new OrderingRule("StringValue", OrderDirection.Ascending)
                        }))
                .Returns(new[] {2})
        };

        public static IEnumerable<TestCaseData> SuccessDynamicScenarios = new List<TestCaseData>
        {
            new TestCaseData(SourceData,
                    new DynamicQuery(
                        new IQueryComponent[]
                        {
                            new ComparisonFilter(ComparisonOperation.Equals, "StringValue.Length", "5"),
                            new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "IntValue", "10"),
                            new OrderingRule("Id", OrderDirection.Ascending)
                        },
                        new ISelector[]
                        {
                            new Selector("Id"),
                            new RenameSelector("StringValue", "Values.String"),
                            new RenameSelector("IntValue", "Values.Numbers.Int")
                        }))
                .Returns(new[]
                {
                    new Dictionary<string, object>
                    {
                        ["Id"] = 1,
                        ["Values"] = new Dictionary<string, object>
                        {
                            ["String"] = "First",
                            ["Numbers"] = new Dictionary<string, object>
                            {
                                ["Int"] = 15
                            }
                        }
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 3,
                        ["Values"] = new Dictionary<string, object>
                        {
                            ["String"] = "Third",
                            ["Numbers"] = new Dictionary<string, object>
                            {
                                ["Int"] = 73
                            }
                        }
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 5,
                        ["Values"] = new Dictionary<string, object>
                        {
                            ["String"] = "Fifth",
                            ["Numbers"] = new Dictionary<string, object>
                            {
                                ["Int"] = 45
                            }
                        }
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 6,
                        ["Values"] = new Dictionary<string, object>
                        {
                            ["String"] = "Sixth",
                            ["Numbers"] = new Dictionary<string, object>
                            {
                                ["Int"] = 22
                            }
                        }
                    }
                })
        };

        public static IEnumerable<TestCaseData> FailedScenarios = new List<TestCaseData>
        {
            new(SourceData, new UnsupportedQuery(), typeof(NoMatchingExtensionsException))
        };


        [OneTimeSetUp]
        public void SetupExtensions()
        {
            QueryNinjaExtensions.Configure.WithQueryableTarget();
        }

        [Test]
        [TestCaseSource(nameof(SuccessScenarios))]
        public IEnumerable<int> ScenariosTestOnQuery(IQueryable<Example> source, IQuery query)
        {
            return source.WithQuery(query).Select(example => example.Id);
        }

        [Test]
        [TestCaseSource(nameof(SuccessDynamicScenarios))]
        public IEnumerable<dynamic> ScenariosTestOnDynamicQuery(IQueryable<Example> source, IDynamicQuery query)
        {
            return source.WithQuery(query).ToList();
        }

        [Test]
        [TestCaseSource(nameof(FailedScenarios))]
        public void FailedScenariosTestOnQuery(IQueryable<Example> source, IQuery query, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => source.WithQuery(query));
        }

        private class UnsupportedQuery : IQuery
        {
            public IReadOnlyList<IQueryComponent> GetComponents()
            {
                return new[] {new UnknownComponent()};
            }

            public bool Equals(IQuery? other)
            {
                return other?.GetType() == GetType();
            }
        }

        private class UnknownComponent : IQueryComponent
        {
            public bool Equals(IQueryComponent? other)
            {
                return other?.GetType() == GetType();
            }

            public string Kind => "unknown";
        }

        public class Example
        {
            public Example(int id, string stringValue, int intValue, int? nullableIntValue)
            {
                Id = id;
                StringValue = stringValue;
                IntValue = intValue;
                NullableIntValue = nullableIntValue;
            }

            public int Id { get; }
            public string StringValue { get; }
            public int IntValue { get; }
            public int? NullableIntValue { get; }
        }
    }
}