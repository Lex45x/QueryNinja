using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Sources.AspNetCore.ModelBinding;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(QueryNinjaModelBinder))]
    public class QueryNinjaModelBinderTests
    {
        [Test]
        public async Task BindModelTest()
        {
            var component = new TestComponent();

            var componentFactory = Mock.Of<IQueryComponentFactory>(factory =>
                // ReSharper disable once RedundantBoolCompare due to Moq specifications
                factory.CanApply(It.IsAny<string>(), It.IsAny<string>()) == true &&
                factory.Create(It.IsAny<string>(), It.IsAny<string>()) == component);

            QueryNinjaExtensions.Configure.Register(componentFactory);

            var queryNinjaModelBinder = new QueryNinjaModelBinder();

            var bindingContext = CreateModelBindingContext();

            await queryNinjaModelBinder.BindModelAsync(bindingContext);

            Assert.True(bindingContext.Result.IsModelSet);
            Assert.IsInstanceOf<IQuery>(bindingContext.Result.Model);

            var query = bindingContext.Result.Model as IQuery;

            Assert.AreSame(query?.GetComponents().First(), component);
        }

        private static ModelBindingContext CreateModelBindingContext()
        {
            var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["test.component"] = "value"
            });
            var httpRequest = Mock.Of<HttpRequest>(request => request.Query == queryCollection);
            var httpContext = Mock.Of<HttpContext>(context => context.Request == httpRequest);
            var bindingContext = Mock.Of<ModelBindingContext>(context => context.HttpContext == httpContext);
            return bindingContext;
        }

        public class TestComponent : IFilter
        {
        }
    }
}