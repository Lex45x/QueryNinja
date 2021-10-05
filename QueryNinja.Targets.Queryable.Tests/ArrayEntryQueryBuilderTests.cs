using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ArrayEntryFilterQueryBuilder))]
    public class ArrayEntryQueryBuilderTests
    {
        public static IEnumerable<TestCaseData> SuccessTests = new List<TestCaseData>
        {
            new TestCaseData(
                    new Example[] {"a", "b", "aa", "ab", "aaa", ""},
                    new ArrayEntryFilter(ArrayEntryOperations.In, "Value", "a|ab|abc"))
                .Returns(new[] {"a", "ab"})
        };

        [Test]
        [TestCaseSource(nameof(SuccessTests))]
        public IEnumerable<string> AppendTestOnStrings(IEnumerable<Example> source, ArrayEntryFilter defaultFilter)
        {
            var builder = new ArrayEntryFilterQueryBuilder();

            var queryable = source.AsQueryable();

            var result = builder.Append(queryable, defaultFilter);

            return result.AsEnumerable().Select(value => value.Value);
        }

        public class Example
        {
            public Example(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public static implicit operator Example(string value)
            {
                return new(value);
            }
        }
    }
}