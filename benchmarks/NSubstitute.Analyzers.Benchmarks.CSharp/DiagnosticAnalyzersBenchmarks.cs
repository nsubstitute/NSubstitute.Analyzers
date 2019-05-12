using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    [CoreJob]
    [MemoryDiagnoser]
    public class DiagnosticAnalyzersBenchmarks
    {
        private Solution _solution;
        
        private Benchmark _callInfoAnalyzerBenchmark;

        private Benchmark _conflictingArgumentAssignmentsAnalyzerBenchmark;

        private Benchmark _nonSubstitutableMemberAnalyzerBenchmark;

        private Benchmark _nonSubstitutableMemberReceivedAnalyzerBenchmark;
        
        private Benchmark _nonSubstitutableMemberWhenAnalyzerBenchmark;

        private Benchmark _reEntrantSetupAnalyzerBenchmark;

        private Benchmark _substituteAnalyzerBenchmark;

        private Benchmark _unusedReceivedAnalyzerBenchmark;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            var assembly = typeof(NonSubstitutableMemberWhenSource).Assembly;
            var excludedMetaAssemblyName = $"{assembly.GetName().Name}.dll";
            var metadataReferences = MetadataReferences.Transitive(assembly)
                .Where(meta => meta.Display.EndsWith(excludedMetaAssemblyName) == false).ToArray();

            var benchmarkSourceProjectPath = GetBenchmarkSourceProjectPath();
            _solution = CodeFactory.CreateSolution(
                new FileInfo(benchmarkSourceProjectPath),
                metadataReferences);

            var projectGraph = _solution.GetProjectDependencyGraph();
            foreach (var projectId in projectGraph.GetTopologicallySortedProjects())
            {
                var projectCompilation = _solution.GetProject(projectId).GetCompilationAsync().Result;
                using (var stream = new MemoryStream())
                {
                    var result = projectCompilation.Emit(stream);
                    if (result.Success == false)
                    {
                        throw new InvalidOperationException("Compilation for benchmark source failed");
                    }
                }
            }

            _callInfoAnalyzerBenchmark = Benchmark.Create(_solution, new CallInfoAnalyzer());
            _conflictingArgumentAssignmentsAnalyzerBenchmark = Benchmark.Create(_solution, new ConflictingArgumentAssignmentsAnalyzer());
            _nonSubstitutableMemberAnalyzerBenchmark = Benchmark.Create(_solution, new NonSubstitutableMemberAnalyzer());
            _nonSubstitutableMemberReceivedAnalyzerBenchmark = Benchmark.Create(_solution, new NonSubstitutableMemberReceivedAnalyzer());
            _nonSubstitutableMemberWhenAnalyzerBenchmark = Benchmark.Create(_solution, new NonSubstitutableMemberWhenAnalyzer());
            _reEntrantSetupAnalyzerBenchmark = Benchmark.Create(_solution, new ReEntrantSetupAnalyzer());
            _substituteAnalyzerBenchmark = Benchmark.Create(_solution, new SubstituteAnalyzer());
            _unusedReceivedAnalyzerBenchmark = Benchmark.Create(_solution, new UnusedReceivedAnalyzer());
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