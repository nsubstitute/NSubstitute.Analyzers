using System.Reflection;
using BenchmarkDotNet.Running;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).GetTypeInfo().Assembly)
                .Run(args);
        }
    }
}