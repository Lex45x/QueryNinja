using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(ServiceCollectionExtensions), Ignore = "Is not completed")]
    public class ServiceCollectionExtensionsTest
    {
        [Test]
        public void AddQueryNinjaTest()
        {
            var serviceCollection = Mock.Of<IServiceCollection>();

            var extensionSettings = serviceCollection.AddQueryNinja();
        }
    }
}