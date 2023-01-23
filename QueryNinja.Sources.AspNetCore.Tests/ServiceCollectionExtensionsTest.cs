using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Factories;
using QueryNinja.Core.Filters;
using QueryNinja.Sources.AspNetCore.ModelBinding;

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

            static IFilter TestFilterFactory(TestOperation operation, string property, string value) =>
                new TestDefaultFilter(operation, property, value);

            extensionSettings.ConfigureFilterFactory(factory =>
            {
                factory.RegisterFilterFactory<TestOperation>(TestFilterFactory);
            });

            var defaultFilterFactory = QueryNinjaExtensions
                .Extensions<IQueryComponentSerializer>()
                .OfType<DefaultFilterSerializer>()
                .Single();

            var instance = defaultFilterFactory.Deserialize("filter.Property.DoSmth", "Value");

            Assert.IsInstanceOf<TestDefaultFilter>(instance);

            var provider = serviceCollection.BuildServiceProvider();

            var configureOptions = provider.GetRequiredService<IConfigureOptions<MvcOptions>>();

            var mvcOptions = new MvcOptions();

            configureOptions.Configure(mvcOptions);

            Assert.IsInstanceOf<QueryNinjaModelBinderProvider>(mvcOptions.ModelBinderProviders[index: 0]);
        }

        private class TestDefaultFilter : AbstractDefaultFilter<TestOperation>
        {
            public TestDefaultFilter(TestOperation operation, string property, string value)
                : base(operation, property, value)
            {
            }
        }

        private enum TestOperation
        {
            DoSmth
        }
    }
}