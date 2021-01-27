using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using QueryNinja.Core;
using QueryNinja.Sources.AspNetCore.ModelBinding;

namespace QueryNinja.Sources.AspNetCore.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(QueryNinjaModelBinderProvider))]
    public class QueryNinjaModelBinderProviderTests
    {
        private static ModelMetadata ModelMetadataForType(Type modelType)
        {
            var compositeMetadataDetailsProvider =
                new DefaultCompositeMetadataDetailsProvider(Enumerable.Empty<IMetadataDetailsProvider>());

            var modelMetadataIdentity = ModelMetadataIdentity.ForType(modelType);
            var attributesForType = ModelAttributes.GetAttributesForType(modelType);
            var defaultMetadataDetails = new DefaultMetadataDetails(modelMetadataIdentity, attributesForType);
            var defaultModelMetadataProvider = new DefaultModelMetadataProvider(compositeMetadataDetailsProvider);

            return new DefaultModelMetadata(defaultModelMetadataProvider, compositeMetadataDetailsProvider,
                defaultMetadataDetails);
        }

        public static IEnumerable<TestCaseData> Contexts()
        {
            var mockedContext = Mock.Of<ModelBinderProviderContext>(context =>
                context.BindingInfo.BindingSource == BindingSource.Query &&
                context.Metadata == ModelMetadataForType(typeof(IQuery)));

            yield return new TestCaseData(mockedContext).Returns(typeof(QueryNinjaModelBinder));

            mockedContext = Mock.Of<ModelBinderProviderContext>(context =>
                context.BindingInfo.BindingSource == BindingSource.Query &&
                context.Metadata == ModelMetadataForType(typeof(string)));

            yield return new TestCaseData(mockedContext).Returns(null);

            mockedContext = Mock.Of<ModelBinderProviderContext>(context =>
                context.BindingInfo.BindingSource == BindingSource.Body &&
                context.Metadata == ModelMetadataForType(typeof(string)));

            yield return new TestCaseData(mockedContext).Returns(null);
        }

        [Test]
        [TestCaseSource(nameof(Contexts))]
        public Type GetBinderTests(ModelBinderProviderContext context)
        {
            var provider = new QueryNinjaModelBinderProvider();

            return provider.GetBinder(context)?.GetType();
        }
    }
}