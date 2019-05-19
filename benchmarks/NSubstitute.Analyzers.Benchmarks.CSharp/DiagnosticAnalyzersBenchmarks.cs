using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    [CoreJob]
    [MemoryDiagnoser]
    public class DiagnosticAnalyzersBenchmarks
    {
        private static readonly Solution Solution;
        
        private readonly Benchmark _callInfoAnalyzerBenchmark;

        private readonly Benchmark _conflictingArgumentAssignmentsAnalyzerBenchmark;

        private readonly Benchmark _nonSubstitutableMemberAnalyzerBenchmark;

        private readonly Benchmark _nonSubstitutableMemberReceivedAnalyzerBenchmark;
        
        private readonly Benchmark _nonSubstitutableMemberWhenAnalyzerBenchmark;

        private readonly Benchmark _reEntrantSetupAnalyzerBenchmark;

        private readonly Benchmark _substituteAnalyzerBenchmark;

        private readonly Benchmark _unusedReceivedAnalyzerBenchmark;

        static DiagnosticAnalyzersBenchmarks()
        {
            var assembly = typeof(NonSubstitutableMemberWhenDiagnosticsSource).Assembly;
            var excludedMetaAssemblyName = $"{assembly.GetName().Name}.dll";
            var metadataReferences = MetadataReferences.Transitive(assembly)
                .Where(meta => meta.Display.EndsWith(excludedMetaAssemblyName) == false).ToArray();

            var benchmarkSourceProjectPath = GetBenchmarkSourceProjectPath();
            Solution = CodeFactory.CreateSolution(
                new FileInfo(benchmarkSourceProjectPath),
                metadataReferences);

            var projectGraph = Solution.GetProjectDependencyGraph();
            foreach (var projectId in projectGraph.GetTopologicallySortedProjects())
            {
                var projectCompilation = Solution.GetProject(projectId).GetCompilationAsync().Result;
                using (var stream = new MemoryStream())
                {
                    var result = projectCompilation.Emit(stream);
                    if (result.Success == false)
                    {
                        throw new InvalidOperationException($"Compilation for benchmark source failed {Environment.NewLine} {string.Join(Environment.NewLine, result.Diagnostics.Select(diag => diag.ToString()))}");
                    }
                }
            }
        }
        
        public DiagnosticAnalyzersBenchmarks()
        {
            _callInfoAnalyzerBenchmark = Benchmark.Create(Solution, new CallInfoAnalyzer());
            _conflictingArgumentAssignmentsAnalyzerBenchmark = Benchmark.Create(Solution, new ConflictingArgumentAssignmentsAnalyzer());
            _nonSubstitutableMemberAnalyzerBenchmark = Benchmark.Create(Solution, new NonSubstitutableMemberAnalyzer());
            _nonSubstitutableMemberReceivedAnalyzerBenchmark = Benchmark.Create(Solution, new NonSubstitutableMemberReceivedAnalyzer());
            _nonSubstitutableMemberWhenAnalyzerBenchmark = Benchmark.Create(Solution, new NonSubstitutableMemberWhenAnalyzer());
            _reEntrantSetupAnalyzerBenchmark = Benchmark.Create(Solution, new ReEntrantSetupAnalyzer());
            _substituteAnalyzerBenchmark = Benchmark.Create(Solution, new SubstituteAnalyzer());
            _unusedReceivedAnalyzerBenchmark = Benchmark.Create(Solution, new UnusedReceivedAnalyzer());
        }

        [Benchmark]
        public void CallInfoAnalyzer()
        {
            _callInfoAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void ConflictingArgumentAssignmentsAnalyzer()
        {
            _conflictingArgumentAssignmentsAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void NonSubstitutableMemberAnalyzer()
        {
            _nonSubstitutableMemberAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void NonSubstitutableMemberReceivedAnalyzer()
        {
            _nonSubstitutableMemberReceivedAnalyzerBenchmark.Run();
        }
        
        [Benchmark]
        public void NonSubstitutableMemberWhenAnalyzer()
        {
            _nonSubstitutableMemberWhenAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void ReEntrantSetupAnalyzer()
        {
            _reEntrantSetupAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void SubstituteAnalyzer()
        {
            _substituteAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void UnusedReceivedAnalyzer()
        {
            _unusedReceivedAnalyzerBenchmark.Run();
        }
        
        private static string GetBenchmarkSourceProjectPath()
        {
            var rootDirectory = FindRootDirectory();

            return Path.Combine(rootDirectory,
                "benchmarks",
                "NSubstitute.Analyzers.Benchmarks.CSharp.Source",
                "NSubstitute.Analyzers.Benchmarks.CSharp.Source.csproj");
        }

        private static string FindRootDirectory()
        {
            const string rootDirFileName = "NSubstitute.Analyzers.sln";
            var location = Assembly.GetExecutingAssembly().Location;

            var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(location));
            do
            {
                var solutionFileInfo =
                    directoryInfo.EnumerateFiles().FirstOrDefault(file => file.Name == rootDirFileName);

                if (solutionFileInfo != null)
                {
                    return solutionFileInfo.DirectoryName;
                }

                directoryInfo = directoryInfo.Parent;
            } while (directoryInfo != null);

            throw new InvalidOperationException("Could not find root directory for NSubstitute.Analyzers");
        }
    }
}