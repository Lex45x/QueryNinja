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
            new Example(id: 1, "First", intValue: 15, nullableIntValue: 15,
                new[]
                {
                    new Example(id: 10, "Tenth", intValue: 10, nullableIntValue: null, new List<Example>())
                }),
            new Example(id: 2, "Second", intValue: 21, nullableIntValue: null, new List<Example>()),
            new Example(id: 3, "Third", intValue: 73, nullableIntValue: 6, new List<Example>()),
            new Example(id: 4, "Fourth", intValue: 90, nullableIntValue: 12, new List<Example>()),
            new Example(id: 5, "Fifth", intValue: 45, nullableIntValue: null, new List<Example>()),
            new Example(id: 6, "Sixth", intValue: 22, nullableIntValue: 15,
                new[]
                {
                    new Example(id: 11, "Eleventh", intValue: 53, nullableIntValue: 22, new List<Example>())
                }),
            new Example(id: 7, "Seventh", intValue: 65, nullableIntValue: 33, new List<Example>()),
            new Example(id: 8, "Eighth", intValue: 34, nullableIntValue: null, new List<Example>()),
            new Example(id: 9, "Ninth", intValue: 8, nullableIntValue: null, new List<Example>())
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
                            new Selector("StringValue"),
                            new Selector("IntValue"),
                            new Selector("ChildValues.Id")
                        }))
                .Returns(new[]
                {
                    new Dictionary<string, object>
                    {
                        ["Id"] = 1,
                        ["StringValue"] = "First",
                        ["IntValue"] = 15,
                        ["ChildValues"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                ["Id"] = 10
                            }
                        } 
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 3,
                        ["StringValue"] = "Third",
                        ["IntValue"] = 73,
                        ["ChildValues"] = new List<Dictionary<string, object>>()
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 5,
                        ["StringValue"] = "Fifth",
                        ["IntValue"] = 45,
                        ["ChildValues"] = new List<Dictionary<string, object>>()
                    },
                    new Dictionary<string, object>
                    {
                        ["Id"] = 6,
                        ["StringValue"] = "Sixth",
                        ["IntValue"] = 22,
                        ["ChildValues"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                ["Id"] = 11
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
        }

        private class UnknownComponent : IQueryComponent
        {
        }

        public class Example
        {
            public Example(int id, string stringValue, int intValue, int? nullableIntValue,
                IReadOnlyList<Example> childValues)
            {
                Id = id;
                StringValue = stringValue;
                IntValue = intValue;
                NullableIntValue = nullableIntValue;
                ChildValues = childValues;
            }

            public int Id { get; }
            public string StringValue { get; }
            public int IntValue { get; }
            public int? NullableIntValue { get; }
            public IReadOnlyList<Example> ChildValues { get; }
        }
    }
}