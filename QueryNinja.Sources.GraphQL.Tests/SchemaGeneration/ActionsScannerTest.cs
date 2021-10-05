using NUnit.Framework;
using QueryNinja.Sources.GraphQL.SchemaGeneration;

namespace QueryNinja.Sources.GraphQL.Tests.SchemaGeneration
{
    [TestFixture(TestOf = typeof(ActionsScanner))]
    public class ActionsScannerTest
    {
        [Test]
        public void ScanActions()
        {
            //todo: mock of provider without mocking whole middleware.

            var actionsScanner = new ActionsScanner(provider: null);
        }
    }
}