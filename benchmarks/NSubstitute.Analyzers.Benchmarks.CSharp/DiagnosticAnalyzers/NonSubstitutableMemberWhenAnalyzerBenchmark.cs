using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.DiagnosticAnalyzers
{
    [ClrJob]
    [CoreJob]
    [MemoryDiagnoser]
    public class NonSubstitutableMemberWhenAnalyzerBenchmark
    {
        private static readonly Solution solution;
        static NonSubstitutableMemberWhenAnalyzerBenchmark()
        {
            var assembly = typeof(NonSubstitutableMemberWhen).Assembly;
            var excludedMetaAssemblyName = $"{assembly.GetName().Name}.dll";
            var metadataReferences = MetadataReferences.Transitive(assembly).Where(meta => meta.Display.EndsWith(excludedMetaAssemblyName) == false).ToArray();
            
            solution = CodeFactory.CreateSolution(
                new FileInfo(GetBenchmarkSourceProjectPath()),
                metadataReferences);
            
            var projectGraph = solution.GetProjectDependencyGraph();
            foreach (var projectId in projectGraph.GetTopologicallySortedProjects())
            {
                var projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;
                    using (var stream = new MemoryStream())
                    {
                        var result = projectCompilation.Emit(stream);
                        if (result.Success == false)
                        {
                            throw new InvalidOperationException("Compilation for benchmark source failed");
                        }
                    }
            }
            
            nonSubstitutableMemberWhenAnalyzerBenchmark =  Benchmark.Create(solution, new NonSubstitutableMemberWhenAnalyzer());
            
        }

        private static Benchmark nonSubstitutableMemberWhenAnalyzerBenchmark;

        [Benchmark]
        public void RunNonSubstitutableMemberWhenAnalyzerBenchmark()
        {
            nonSubstitutableMemberWhenAnalyzerBenchmark.Run();
        }
        
        private static string GetBenchmarkSourceProjectPath()
        {
            var locations = new[] { "..", "..", "..", "..", "NSubstitute.Analyzers.Benchmarks.CSharp.Source", "NSubstitute.Analyzers.Benchmarks.CSharp.Source.csproj" };
            var sourcePath = string.Join(Path.DirectorySeparatorChar, locations);
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), sourcePath));
        }
    }
}