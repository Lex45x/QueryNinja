using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Moq;
using QueryNinja.Core;
using QueryNinja.Core.Extensibility;
using QueryNinja.Sources.AspNetCore;
using QueryNinja.Sources.AspNetCore.ModelBinding;
using QueryNinja.Targets.EntityFrameworkCore;

namespace QueryNinja.Benchmarking.Sources.AspNetCore
{
    [SimpleJob(launchCount: 1, warmupCount: 5, targetCount: 30)]
    public class ModelBinderBenchmarks
    {
        private static readonly QueryNinjaModelBinder QueryNinjaModelBinder = new();

        [GlobalSetup]
        public void GlobalSetup()
        {
            QueryNinjaExtensions.Configure
                .WithAspNetCoreSource()
                .WithEntityFrameworkTarget();
        }

        private static ModelBindingScenario CreateModelBindingScenario(Dictionary<string, StringValues> query,
            string description)
        {
            var queryCollection = new QueryCollection(query);
            var httpRequest = Mock.Of<HttpRequest>(request => request.Query == queryCollection);
            var httpContext = Mock.Of<HttpContext>(context => context.Request == httpRequest);
            var bindingContext = Mock.Of<ModelBindingContext>(context =>
                context.HttpContext == httpContext && context.ModelType == typeof(IDynamicQuery));

            return new ModelBindingScenario
            {
                Context = bindingContext,
                Description = description
            };
        }

        public static IEnumerable<ModelBindingScenario> UsageScenarios()
        {
            var emptyQuery = new Dictionary<string, StringValues>();

            yield return CreateModelBindingScenario(emptyQuery, "Empty Query");

            var tenFilters = new Dictionary<string, StringValues>
            {
                ["filter.A.Equals"] = "1",
                ["filter.B.IsEmpty"] = "true",
                ["filter.C.Like"] = "pattern",
                ["filter.D.Less"] = "2",
                ["filter.E.Contains"] = "item",
                ["filter.F.Greater"] = "3",
                ["filter.G.Like"] = "another pattern",
                ["filter.H.NotEquals"] = "4",
                ["filter.I.GreaterOrEquals"] = "5",
                ["filter.J.Equals"] = "string"
            };

            yield return CreateModelBindingScenario(tenFilters, "10 Filters");

            var tenOrderingRules = new Dictionary<string, StringValues>
            {
                ["order.A"] = "Ascending",
                ["order.B"] = "Descending",
                ["order.C"] = "Ascending",
                ["order.D"] = "Descending",
                ["order.E"] = "Ascending",
                ["order.F"] = "Ascending",
                ["order.G"] = "Descending",
                ["order.H"] = "Descending",
                ["order.I"] = "Ascending",
                ["order.J"] = "Descending"
            };

            yield return CreateModelBindingScenario(tenOrderingRules, "10 OrderingRules");

            var tenSelects = new Dictionary<string, StringValues>
            {
                ["select.A"] = "A Value",
                ["select.B"] = "B.Value",
                ["select.C"] = "C",
                ["select.D"] = "D",
                ["select.E"] = "Some E Value",
                ["select.F"] = "F",
                ["select.G"] = "Values.G",
                ["select"] = new[] {"H", "I", "J"}
            };

            yield return CreateModelBindingScenario(tenSelects, "10 Selects");

            var tenOfEach = new Dictionary<string, StringValues>(tenFilters.Concat(tenSelects).Concat(tenOrderingRules));

            yield return CreateModelBindingScenario(tenOfEach, "10 of Each");
        }

        [ParamsSource(nameof(UsageScenarios))]
        public ModelBindingScenario Scenario { get; set; }

        [Benchmark]
        public async Task<ModelBindingContext> ModelBindingBenchmark()
        {
            await QueryNinjaModelBinder.BindModelAsync(Scenario.Context);

            return Scenario.Context;
        }

        public class ModelBindingScenario
        {
            public ModelBindingContext Context { get; init; }
            public string Description { get; init; }

            /// <inheritdoc />
            public override string ToString()
            {
                return Description;
            }
        }
    }
}