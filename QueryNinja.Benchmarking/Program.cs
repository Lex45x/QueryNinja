using System;
using BenchmarkDotNet.Running;
using QueryNinja.Benchmarking.Sources.AspNetCore;

namespace QueryNinja.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ModelBinderBenchmarks>();
        }
    }
}
