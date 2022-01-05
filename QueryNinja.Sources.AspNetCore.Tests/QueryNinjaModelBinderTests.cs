using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.Projection;
using QueryNinja.Sources.AspNetCore.ModelBinding;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(QueryNinjaModelBinder))]
    public class QueryNinjaModelBinderTests
    {
        /// <summary>
        /// Moq is unable to mock ReadOnlySpan
        /// </summary>
        private class FactoryMock:IQueryComponentFactory
        {
            private readonly IQueryComponent instance;

            public FactoryMock(IQueryComponent instance)
            {
                this.instance = instance;
            }

            /// <inheritdoc />
            public Type QueryComponent => instance.GetType();

            /// <inheritdoc />
            public bool CanApply(ReadOnlySpan<char> name, string value)
            {
                return true;
            }

            /// <inheritdoc />
            public IQueryComponent Create(ReadOnlySpan<char> name, string value)
            {
                return instance;
            }
        }

        [Test]
        public async Task BindQueryModelTest()
        {
            var component = new TestComponent();

            var componentFactory = new FactoryMock(component);

            QueryNinjaExtensions.Configure
                .ForType<IQueryComponentFactory>()
                .Register(componentFactory);

            var queryNinjaModelBinder = new QueryNinjaModelBinder();

            var queryString = new Dictionary<string, StringValues>
            {
                ["test.component"] = "value"
            };

            var bindingContext = CreateModelBindingContext(typeof(IQuery), queryString);

            await queryNinjaModelBinder.BindModelAsync(bindingContext);

            Assert.True(bindingContext.Result.IsModelSet);
            Assert.IsInstanceOf<IQuery>(bindingContext.Result.Model);

            var query = bindingContext.Result.Model as IQuery;

            Assert.AreSame(query?.GetComponents().First(), component);
        }

        [Test]
        public async Task BindDynamicQueryTest()
        {
            var queryNinjaModelBinder = new QueryNinjaModelBinder();

            var queryString = new Dictionary<string, StringValues>
            {
                ["select"] = "Property",
                ["select.AnotherProperty"] = "Another Property"
            };

            var bindingContext = CreateModelBindingContext(typeof(IDynamicQuery), queryString);

            await queryNinjaModelBinder.BindModelAsync(bindingContext);

            Assert.True(bindingContext.Result.IsModelSet);
            Assert.IsInstanceOf<IQuery>(bindingContext.Result.Model);

            var query = bindingContext.Result.Model as IDynamicQuery;

            var selectors = query?.GetSelectors();

            Assert.NotNull(selectors);
            Assert.AreEqual(expected: 2, selectors.Count);

            var selector = selectors.OfType<Selector>().First();
            Assert.AreEqual("Property", selector.Target);

            var renameSelector = selectors.OfType<RenameSelector>().First();
            Assert.AreEqual("AnotherProperty", renameSelector.Source);
            Assert.AreEqual("Another Property", renameSelector.Target);
        }

        private static ModelBindingContext CreateModelBindingContext(Type modelType,
            Dictionary<string, StringValues> query)
        {
            var queryCollection = new QueryCollection(query);
            var httpRequest = Mock.Of<HttpRequest>(request => request.Query == queryCollection);
            var httpContext = Mock.Of<HttpContext>(context => context.Request == httpRequest);
            IList<ParameterDescriptor> parameters = new List<ParameterDescriptor>
            {
                Mock.Of<ParameterDescriptor>(descriptor => descriptor.ParameterType == modelType)
            };
            var actionDescriptor = Mock.Of<ActionDescriptor>(descriptor => descriptor.Parameters == parameters);
            var actionContext = Mock.Of<ActionContext>(context => context.ActionDescriptor == actionDescriptor);
            var bindingContext = Mock.Of<ModelBindingContext>(context =>
                context.HttpContext == httpContext && context.ModelType == modelType && context.ActionContext == actionContext);
            return bindingContext;
        }

        private class TestComponent : IFilter
        {
        }
    }
}