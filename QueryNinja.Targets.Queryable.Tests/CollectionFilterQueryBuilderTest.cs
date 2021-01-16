using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.Exceptions;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(CollectionFilterQueryBuilder))]
    public class CollectionFilterQueryBuilderTest
    {
        public static IEnumerable<TestCaseData> SuccessTests = new List<TestCaseData>
        {
            new TestCaseData(
                    new Example[]{"a","b","aa","ab","aaa",""},
                    new CollectionFilter(CollectionOperation.Contains, "Value", "a"))
                .Returns(new []{"a","aa", "ab", "aaa"}),
            new TestCaseData(
                    new Example[]{"a","b","aa","ab","aaa",""},
                    new CollectionFilter(CollectionOperation.IsEmpty, "Value", "true"))
                .Returns(new []{""})
        };

        public static IEnumerable<TestCaseData> FailedTests = new List<TestCaseData>
        {
            new TestCaseData(
                    new Example[]{"a","b","aa","ab","aaa",""},
                    new CollectionFilter(CollectionOperation.Contains, "Value.Length", "a"),
                    typeof(PropertyIsNotCollectionException)),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa",""},
                new CollectionFilter((CollectionOperation) 12, "Value", "a"),
                typeof(QueryBuildingException))
        };

        [Test]
        [TestCaseSource(nameof(SuccessTests))]
        public IEnumerable<string> AppendTestOnStrings(IEnumerable<Example> source, CollectionFilter defaultFilter)
        {
            var builder = new CollectionFilterQueryBuilder();

            var queryable = source.AsQueryable();

            var result = builder.Append(queryable, defaultFilter);

            return result.AsEnumerable().Select(value => value.Value);
        }

        [Test]
        [TestCaseSource(nameof(FailedTests))]
        public void FailedAppendTestOnStrings(IEnumerable<Example> source, CollectionFilter defaultFilter, Type exceptionType)
        {
            var builder = new CollectionFilterQueryBuilder();

            var queryable = source.AsQueryable();

            Assert.Throws(exceptionType, () => builder.Append(queryable, defaultFilter));
        }

        public class Example
        {
            public string Value { get; }

            public Example(string value)
            {
                Value = value;
            }

            public static implicit operator Example(string value)
            {
                return new (value);
            }
        }
    }
}