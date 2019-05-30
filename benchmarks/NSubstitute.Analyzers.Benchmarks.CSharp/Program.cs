using System.Reflection;
using BenchmarkDotNet.Running;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssemblies(new [] { typeof(Program).GetTypeInfo().Assembly, typeof(NSubstitute.Analyzers.Benchmarks.VisualBasic.Program).Assembly })
                .Run(args);
        }
    }
}