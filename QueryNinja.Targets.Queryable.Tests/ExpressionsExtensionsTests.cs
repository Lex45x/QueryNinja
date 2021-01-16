using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QueryNinja.Targets.Queryable.Exceptions;

namespace QueryNinja.Targets.Queryable.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ExpressionsExtensions))]
    public class ExpressionsExtensionsTests
    {
        public static IEnumerable<TestCaseData> Constants = new List<TestCaseData>
        {
            new TestCaseData(arg1: 1, arg2: null),
            new TestCaseData(arg1: 1.1, arg2: null),
            new TestCaseData(arg1: 1.1m, arg2: null),
            new TestCaseData("test", arg2: null),
            new TestCaseData(arg1: 'a', arg2: null),
            new TestCaseData(StringComparison.Ordinal, arg2: null),
            new TestCaseData(new List<string>(), typeof(TypeConversionException))
        };

        public static IEnumerable<TestCaseData> Expressions = new List<TestCaseData>
        {
            new TestCaseData(Enumerable.Repeat(element: 5, count: 1).AsQueryable().OrderBy(a => a).Where(a => true))
                .Returns(result: true),
            new TestCaseData(Enumerable.Repeat(element: 5, count: 1).AsQueryable().OrderBy(a => a)).Returns(
                result: true),
            new TestCaseData(Enumerable.Repeat(element: 5, count: 1).AsQueryable()).Returns(result: false)
        };

        public static IEnumerable<TestCaseData> FromSuccessTestCases = new List<TestCaseData>
        {
            new TestCaseData("Value").Returns("1"),
            new TestCaseData("Child.Value").Returns("2"),
            new TestCaseData("Child.Child.Value").Returns("3")
        };

        public static IEnumerable<TestCaseData> FromFailedTestCases = new List<TestCaseData>
        {
            new TestCaseData("Values", typeof(InvalidPropertyException)),
            new TestCaseData("Child.Values", typeof(InvalidPropertyException))
        };

        [Test]
        [TestCaseSource(nameof(Constants))]
        public void AsConstantTest(object constant, Type exceptionType)
        {
            var value = constant.ToString();

            Expression? constantExpression = null;

            if (exceptionType == null)
            {
                Assert.DoesNotThrow(() => constantExpression = value?.AsConstant(constant.GetType()));

                Assert.AreEqual(constantExpression?.Type, constant.GetType());
            }
            else
            {
                Assert.Throws(exceptionType, () => constantExpression = value?.AsConstant(constant.GetType()));
            }
        }

        private static readonly Example ExampleInstance = new()
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
        [TestCaseSource(nameof(FromSuccessTestCases))]
        public string? FromTest(string path)
        {
            var lambda = path.From<Example>();

            var @delegate = lambda.Compile();

            var result = (string?) @delegate.DynamicInvoke(ExampleInstance);

            return result;
        }

        [Test]
        [TestCaseSource(nameof(FromFailedTestCases))]
        public void FromFailedTest(string path, Type exceptionType)
        {
            Assert.Throws(exceptionType, () => path.From<Example>());
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