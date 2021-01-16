using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ExpressionsExtensions))]
    public class ExpressionsExtensionsTests
    {
        public static IEnumerable<object> Constants = new List<object>
        {
            1,
            1.1,
            1.1m,
            "test",
            'a',
            StringComparison.Ordinal
        };

        public static IEnumerable<TestCaseData> Expressions = new List<TestCaseData>
        {
            new TestCaseData(Enumerable.Repeat(5, 1).AsQueryable().OrderBy(a => a).Where(a => true)).Returns(true),
            new TestCaseData(Enumerable.Repeat(5, 1).AsQueryable().OrderBy(a => a)).Returns(true),
            new TestCaseData(Enumerable.Repeat(5, 1).AsQueryable()).Returns(false)
        };

        public static IEnumerable<TestCaseData> FromTestCases = new List<TestCaseData>
        {
            new TestCaseData("Value").Returns("1"),
            new TestCaseData("Child.Value").Returns("2"),
            new TestCaseData("Child.Child.Value").Returns("3")
        };

        [Test]
        [TestCaseSource(nameof(Constants))]
        public void AsConstantTest(object constant)
        {
            var value = constant.ToString();

            Expression? constantExpression = null;

            Assert.DoesNotThrow(() => constantExpression = value?.AsConstant(constant.GetType()));

            Assert.AreEqual(constantExpression?.Type, constant.GetType());
        }

        private static readonly Example example = new Example
        {
            Value = "1",
            Child = new Example
            {
                Value = "2",
                Child = new Example
                {
                    Value = "3"
                }
            }
        };

        [Test]
        [TestCaseSource(nameof(FromTestCases))]
        public string? FromTest(string path)
        {
            var lambda = path.From<Example>();

            var @delegate = lambda.Compile();

            var result = (string?)@delegate?.DynamicInvoke(example);

            return result;
        }

        [Test]
        [TestCaseSource(nameof(Expressions))]
        public bool IsOrderExpressionDefinedTest(IQueryable<int> source)
        {
            return source.IsOrderExpressionDefined();
        }

        public class Example
        {
            public string? Value { get; set; }

            public Example? Child { get; set; }
        }
    }
}