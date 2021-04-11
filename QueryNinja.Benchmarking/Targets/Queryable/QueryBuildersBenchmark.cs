using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Microsoft.EntityFrameworkCore;
using QueryNinja.Benchmarking.ExampleDomain;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Core.Filters;
using QueryNinja.Core.OrderingRules;
using QueryNinja.Sources.AspNetCore;
using QueryNinja.Targets.EntityFrameworkCore;
using QueryNinja.Targets.EntityFrameworkCore.Filters;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Benchmarking.Targets.Queryable
{
    
    [SimpleJob]
    [MemoryDiagnoser]
    [EventPipeProfiler(EventPipeProfile.CpuSampling)]
    [RPlotExporter]
    public class QueryBuildersBenchmark
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
            QueryNinjaExtensions.Configure
                .WithAspNetCoreSource()
                .WithEntityFrameworkTarget();
        }

        public IEnumerable<QueryBuildingScenario> Scenarios()
        {
            yield return new QueryBuildingScenario
            {
                QueryBuilder = new ComparisonFilterQueryBuilder(),
                Component = new ComparisonFilter(ComparisonOperation.Equals, "Id", "1"),
                Description = "Comparison Filter"
            };

            yield return new QueryBuildingScenario
            {
                QueryBuilder = new CollectionFilterQueryBuilder(),
                Component = new CollectionFilter(CollectionOperation.IsEmpty, "Orders", "true"),
                Description = "Collection Filter"
            };

            var defaultFilterQueryBuilder = new DefaultFilterQueryBuilder<DatabaseFunctionFilter, DatabaseFunction>();
            defaultFilterQueryBuilder.Define<string>(DatabaseFunction.Like,
                (property, value) => EF.Functions.Like(property, value)
            );

            yield return new QueryBuildingScenario
            {
                QueryBuilder = defaultFilterQueryBuilder,
                Component = new DatabaseFunctionFilter(DatabaseFunction.Like, "FullName", "1"),
                Description = "Default Filter"
            };

            yield return new QueryBuildingScenario
            {
                QueryBuilder = new OrderQueryBuilder(),
                Component = new OrderingRule("Orders.Count", OrderDirection.Descending),
                Description = "Ordering Rule"
            };
        }

        [ParamsSource(nameof(Scenarios))]
        public QueryBuildingScenario Scenario { get; set; }

        public IQueryable<Customer> Source { get; } = Enumerable.Empty<Customer>().AsQueryable();


        [Benchmark]
        public object QueryBuilding()
        {
            return Scenario.QueryBuilder.Append(Source, Scenario.Component);
        }

        public class QueryBuildingScenario
        {
            public IQueryComponent Component { get; init; }
            public IQueryBuilder QueryBuilder { get; init; }

            public string Description { get; init; }

            /// <inheritdoc />
            public override string ToString()
            {
                return Description;
            }
        }
    }
}