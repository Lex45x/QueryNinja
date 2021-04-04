using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.EntityFrameworkCore.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.EntityFrameworkCore.Tests
{
    [TestFixture(TestOf = typeof(RegistrationExtensions))]
    public class RegistrationExtensionsTest
    {
        [Test]
        public void TestDatabaseFunctionRegistration()
        {
            QueryNinjaExtensions.Configure.WithEntityFrameworkTarget();

            var registeredQueryBuilders = QueryNinjaExtensions.Extensions<DefaultFilterQueryBuilder<DatabaseFunctionFilter, DatabaseFunction>>().ToList();
            
            Assert.AreEqual(expected: 1, registeredQueryBuilders.Count);

            var queryBuilder = registeredQueryBuilders.Single();

            var filter = new DatabaseFunctionFilter(DatabaseFunction.Like, "test", "test");
            var canAppend = queryBuilder.CanAppend(filter);

            Assert.True(canAppend);
        }
    }
}