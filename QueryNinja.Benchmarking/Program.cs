using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Running;

namespace QueryNinja.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }

    
}