using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(CollectionFilterQueryBuilder))]
    public class CollectionFilterQueryBuilderTest
    {
        public static IEnumerable<TestCaseData> CollectionsAndFilters = new List<TestCaseData>
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

        [Test]
        [TestCaseSource(nameof(CollectionsAndFilters))]
        public IEnumerable<string> AppendTestOnStrings(IEnumerable<Example> source, CollectionFilter defaultFilter)
        {
            var builder = new CollectionFilterQueryBuilder();

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