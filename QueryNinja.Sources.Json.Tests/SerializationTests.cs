using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;

namespace QueryNinja.Sources.Json.Tests
{
    public class SerializationTests
    {
        [SetUp]
        public void Setup()
        {
            QueryNinjaExtensions.Configure.WithJsonSource();
        }

        [Test]
        [TestCaseSource(nameof(QuerySamples))]
        public void ReverseEqualityTest(IQuery source)
        {
            var jObject = JObject.FromObject(source);
            var query = jObject.ToObject<IQuery>();

            Assert.AreEqual(source, query);
        }

        public static IEnumerable<IQuery> QuerySamples
        {

            get
            {
                yield return new Query(new[]
                {
                    new OrderingRule("A", OrderDirection.Ascending)
                });

                yield return new Query(new IQueryComponent[]
                {
                    new OrderingRule("A", OrderDirection.Ascending),
                    new ComparisonFilter(ComparisonOperation.Equals,"A", "Value 1"),
                    new ComparisonFilter(ComparisonOperation.Less,"A", "Value 2"),
                    new ComparisonFilter(ComparisonOperation.Less,"B", "Value 2"),
                    new OrderingRule("B", OrderDirection.Descending)
                });
            }
        }
    }
}