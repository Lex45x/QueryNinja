using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QueryNinja.Core.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(DefaultFilterQueryBuilder<,>))]
    public class DefaultFilterQueryBuilderTests
    {
        [Test]
        public void DefineEqualsOnInTest()
        {
            var queryBuilder = new DefaultFilterQueryBuilder<TestFilter, TestOperations>();

            queryBuilder.Define<int>(TestOperations.Equals, (property, constant) => property == constant);
            
            var testTarget = new List<TestClass>
                {
                    new TestClass(1, "property", 10),
                    new TestClass(2, "name", 15)
                }
                .AsQueryable();

            var filter = new TestFilter(TestOperations.Equals, "Id", "1");

            Assert.True(queryBuilder.CanAppend(filter));

            var result = queryBuilder.Append(testTarget, filter);

            var testClass = result.Single();

            Assert.AreEqual(testClass.Id, actual: 1);
        }

        [Test]
        public void DefineContainsOnStringTest()
        {
            var queryBuilder = new DefaultFilterQueryBuilder<TestFilter, TestOperations>();

            queryBuilder.Define<string, char>(TestOperations.Contains, (property, constant) => property.Contains(constant));

            var testTarget = new List<TestClass>
                {
                    new TestClass(1, "property", 10),
                    new TestClass(2, "name", 15)
                }
                .AsQueryable();

            var filter = new TestFilter(TestOperations.Contains, "Property", "n");

            Assert.True(queryBuilder.CanAppend(filter));

            var result = queryBuilder.Append(testTarget, filter);

            var testClass = result.Single();

            Assert.AreEqual(testClass.Id, actual: 2);
        }

        public enum TestOperations
        {
            Equals,
            Contains
        }

        public class TestFilter : IDefaultFilter<TestOperations>
        {
            public TestFilter(TestOperations operation, string property, string value)
            {
                Operation = operation;
                Property = property;
                Value = value;
            }

            /// <inheritdoc />
            public TestOperations Operation { get; }

            /// <inheritdoc />
            public string Property { get; }

            /// <inheritdoc />
            public string Value { get; }
        }

        public class TestClass
        {
            public TestClass(int id, string property, decimal amount)
            {
                Id = id;
                Property = property;
                Amount = amount;
            }

            public int Id { get; }
            public string Property { get; }
            public decimal Amount { get; }
        }
    }
}