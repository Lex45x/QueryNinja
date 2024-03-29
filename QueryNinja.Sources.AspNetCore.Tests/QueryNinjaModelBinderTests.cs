﻿using System;
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
using QueryNinja.Core.Projection;
using QueryNinja.Sources.AspNetCore.ModelBinding;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(QueryNinjaModelBinder))]
    public class QueryNinjaModelBinderTests
    {
        [Test]
        public async Task BindQueryModelTest()
        {
            var component = new TestComponent();

            var componentFactory = Mock.Of<IQueryComponentFactory>(factory =>
                // ReSharper disable once RedundantBoolCompare due to Moq specifications
                factory.CanApply(It.IsAny<string>(), It.IsAny<string>()) == true &&
                factory.Create(It.IsAny<string>(), It.IsAny<string>()) == component &&
                factory.QueryComponent == component.GetType());

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
            var bindingContext = Mock.Of<ModelBindingContext>(context =>
                context.HttpContext == httpContext && context.ModelType == modelType);
            return bindingContext;
        }

        private class TestComponent : IFilter
        {
        }
    }
}