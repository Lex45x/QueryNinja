using BenchmarkDotNet.Running;

namespace QueryNinja.Benchmarking
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}