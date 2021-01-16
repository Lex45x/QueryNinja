using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;
// ReSharper disable StringLiteralTypo

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ComparisonFilterQueryBuilder))]
    public class ComparisonFilterQueryBuilderTest
    {
        public static IEnumerable<TestCaseData> CollectionsAndFilters = new List<TestCaseData>
        {
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.Equals, "Value.Length", "1"))
            .Returns(new []{"a","b"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.NotEquals, "Value.Length", "2"))
            .Returns(new []{"a","b","aaa","aabb"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.Greater, "Value.Length", "3"))
            .Returns(new []{"aabb"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.GreaterOrEquals, "Value.Length", "3"))
            .Returns(new []{"aaa","aabb"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.Less, "Value.Length", "2"))
            .Returns(new []{"a","b"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.LessOrEquals, "Value.Length", "2"))
            .Returns(new []{"a","b","aa","ab"}),
            new TestCaseData(
                new Example[]{"a","b","aa","ab","aaa","aabb"},
                new ComparisonFilter(ComparisonOperation.Equals, "Value", "a"))
            .Returns(new []{"a"})
        };

        [Test]
        [TestCaseSource(nameof(CollectionsAndFilters))]
        public IEnumerable<string> AppendTestOnStrings(IEnumerable<Example> source, ComparisonFilter defaultFilter)
        {
            var builder = new ComparisonFilterQueryBuilder();

            var queryable = source.AsQueryable();

            var result = builder.Append(queryable, defaultFilter);

            return result.AsEnumerable().Select(value => value.Value);
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
