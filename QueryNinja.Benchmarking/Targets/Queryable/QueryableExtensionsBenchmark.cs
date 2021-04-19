using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using QueryNinja.Benchmarking.ExampleDomain;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Core.Projection;
using QueryNinja.Sources.AspNetCore;
using QueryNinja.Targets.EntityFrameworkCore;
using QueryNinja.Targets.EntityFrameworkCore.Filters;
using QueryNinja.Targets.Queryable;

namespace QueryNinja.Benchmarking.Targets.Queryable
{
    [SimpleJob]
    [MemoryDiagnoser]
    public class QueryableExtensionsBenchmark
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
            QueryNinjaExtensions.Configure
                .WithAspNetCoreSource()
                .WithEntityFrameworkTarget();
        }


        public static IEnumerable<QueryScenario> UsageScenarios()
        {
            yield return new QueryScenario
            {
                Query = new DynamicQuery(new List<IQueryComponent>(), new List<ISelector>()),
                Description = "Empty Query"
            };

            var fiveFilters = new List<IQueryComponent>
            {
                new DatabaseFunctionFilter(DatabaseFunction.Like, "FullName", "Oleks%"),
                new ComparisonFilter(ComparisonOperation.NotEquals, "Id", "10"),
                new ArrayEntryFilter(ArrayEntryOperations.In, "Id", "1|2|3"),
                new CollectionFilter(CollectionOperation.IsEmpty, "Orders", "false"),
                new ComparisonFilter(ComparisonOperation.Greater, "Orders.Count", "5")
            };

            yield return new QueryScenario
            {
                Query = new DynamicQuery(fiveFilters, new List<ISelector>()),
                Description = "5 Filters"
            };

            var fiveOrderRules  = new List<IQueryComponent>
            {
                new OrderingRule("Orders.Count", OrderDirection.Descending),
                new OrderingRule("Country", OrderDirection.Ascending),
                new OrderingRule("FullName", OrderDirection.Descending),
                new OrderingRule("Email", OrderDirection.Ascending),
                new OrderingRule("PhoneNumber", OrderDirection.Ascending)
            };

            yield return new QueryScenario
            {
                Query = new DynamicQuery(fiveOrderRules, new List<ISelector>()),
                Description = "5 Order Rules"
            };

            var fiveSelects = new List<ISelector>
            {
                new Selector("Id"),
                new Selector("FullName"),
                new RenameSelector("Orders.Count", "OrdersCount"),
                new RenameSelector("Email", "Contacts.Email"),
                new RenameSelector("PhoneNumber", "Contacts.PhoneNumber")
            };

            yield return new QueryScenario
            {
                Query = new DynamicQuery(new List<IQueryComponent>(), fiveSelects),
                Description = "5 Selects"
            };

            yield return new QueryScenario
            {
                Query = new DynamicQuery(fiveFilters.Concat(fiveOrderRules).ToList(), fiveSelects),
                Description = "5 of Each"
            };
        }

        [ParamsSource(nameof(UsageScenarios))]
        public QueryScenario Scenario { get; set; }

        public IQueryable<Customer> Source { get; } = Enumerable.Empty<Customer>().AsQueryable();

        [Benchmark]
        public object Query()
        {
            return Source.WithQuery(Scenario.Query);
        }

        public class QueryScenario
        {
            public IDynamicQuery Query { get; init; }
            public string Description { get; init; }

            /// <inheritdoc />
            public override string ToString()
            {
                return Description;
            }
        }
    }
}