using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Sources.AspNetCore.Factory;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ServiceCollectionExtensions))]
    public class ServiceCollectionExtensionsTest
    {
        [Test]
        public void ConfigureDefaultFilterFactoryTest()
        {
            var serviceCollection = new ServiceCollection();

            var extensionSettings = serviceCollection.AddQueryNinja();

            static IFilter TestFilterFactory(TestOperation operation, string property, string value) => new TestDefaultFilter(operation, property, value);

            extensionSettings.ConfigureFilterFactory(factory =>
            {
                factory.RegisterFilterFactory<TestOperation>(TestFilterFactory);
            });

            var defaultFilterFactory = QueryNinjaExtensions.Extensions<DefaultFilterFactory>().Single();

            var instance = defaultFilterFactory.Create("filter.Property.DoSmth", "Value");

            Assert.IsInstanceOf<TestDefaultFilter>(instance);
        }

        public class TestDefaultFilter : IDefaultFilter<TestOperation>
        {
            public TestDefaultFilter(TestOperation operation, string property, string value)
            {
                Operation = operation;
                Property = property;
                Value = value;
            }

            /// <inheritdoc />
            public TestOperation Operation { get; }

            /// <inheritdoc />
            public string Property { get; }

            /// <inheritdoc />
            public string Value { get; }
        }

        public enum TestOperation
        {
            DoSmth
        }
    }
}