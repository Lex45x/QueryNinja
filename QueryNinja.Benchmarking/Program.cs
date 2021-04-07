using System;
using System.Reflection;
using BenchmarkDotNet.Running;
using QueryNinja.Benchmarking.Sources.AspNetCore;
using QueryNinja.Benchmarking.Targets.Queryable;

namespace QueryNinja.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
        }
    }
}
