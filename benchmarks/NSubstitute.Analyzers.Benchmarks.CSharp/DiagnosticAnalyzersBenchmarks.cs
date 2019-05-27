using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources;
using NSubstitute.Analyzers.Benchmarks.Shared;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    [CoreJob]
    [MemoryDiagnoser]
    public class DiagnosticAnalyzersBenchmarks
    {
        private static readonly Solution Solution;

        private readonly AnalyzerBenchmark _callInfoAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _conflictingArgumentAssignmentsAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _nonSubstitutableMemberAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _nonSubstitutableMemberReceivedAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _nonSubstitutableMemberWhenAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _reEntrantSetupAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _substituteAnalyzerAnalyzerBenchmark;

        private readonly AnalyzerBenchmark _unusedReceivedAnalyzerAnalyzerBenchmark;

        static DiagnosticAnalyzersBenchmarks()
        {
            var assembly = typeof(NonSubstitutableMemberWhenDiagnosticsSource).Assembly;
            var excludedMetaAssemblyName = $"{assembly.GetName().Name}.dll";
            var metadataReferences = MetadataReferences.Transitive(assembly)
                .Where(meta => meta.Display.EndsWith(excludedMetaAssemblyName) == false).ToArray();

            var benchmarkSourceProjectPath = GetBenchmarkSourceProjectDirectoryPath();

            var loader = new SolutionLoader();
            Solution = loader.CreateSolution(benchmarkSourceProjectPath, metadataReferences);

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
            _callInfoAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new CallInfoAnalyzer());
            _conflictingArgumentAssignmentsAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new ConflictingArgumentAssignmentsAnalyzer());
            _nonSubstitutableMemberAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberAnalyzer());
            _nonSubstitutableMemberReceivedAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberReceivedAnalyzer());
            _nonSubstitutableMemberWhenAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberWhenAnalyzer());
            _reEntrantSetupAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new ReEntrantSetupAnalyzer());
            _substituteAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new SubstituteAnalyzer());
            _unusedReceivedAnalyzerAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new UnusedReceivedAnalyzer());
        }

        [Benchmark]
        public void CallInfoAnalyzer()
        {
            _callInfoAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void ConflictingArgumentAssignmentsAnalyzer()
        {
            _conflictingArgumentAssignmentsAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void NonSubstitutableMemberAnalyzer()
        {
            _nonSubstitutableMemberAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void NonSubstitutableMemberReceivedAnalyzer()
        {
            _nonSubstitutableMemberReceivedAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void NonSubstitutableMemberWhenAnalyzer()
        {
            _nonSubstitutableMemberWhenAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void ReEntrantSetupAnalyzer()
        {
            _reEntrantSetupAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void SubstituteAnalyzer()
        {
            _substituteAnalyzerAnalyzerBenchmark.Run();
        }

        [Benchmark]
        public void UnusedReceivedAnalyzer()
        {
            _unusedReceivedAnalyzerAnalyzerBenchmark.Run();
        }

        private static string GetBenchmarkSourceProjectDirectoryPath()
        {
            var rootDirectory = FindRootDirectory();

            return Path.Combine(
                rootDirectory,
                "benchmarks",
                "NSubstitute.Analyzers.Benchmarks.CSharp.Source");
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
            }
            while (directoryInfo != null);

            throw new InvalidOperationException("Could not find root directory for NSubstitute.Analyzers");
        }

        private static Solution CreateSolution(string projrectDirectory, MetadataReference[] metadataReferences)
        {
            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var projectId = ProjectId.CreateNewId();
                var solution = adhocWorkspace
                    .CurrentSolution
                    .AddProject(projectId, "AnalyzerBenchmark", "AnalyzerBenchmark", LanguageNames.CSharp);

                foreach (var file in Directory.EnumerateFiles(projrectDirectory, "*.*", SearchOption.TopDirectoryOnly))
                {
                    switch (Path.GetExtension(file))
                    {
                        case ".json":
                            solution = solution.AddAdditionalDocument(DocumentId.CreateNewId(projectId),
                                Path.GetFileName(file), File.ReadAllText(file));
                            break;
                        case ".cs":
                            solution = solution.AddDocument(DocumentId.CreateNewId(projectId), Path.GetFileName(file), File.ReadAllText(file));
                            break;
                    }
                }
                
                /*
                var settings = GetSettings();
                if (!string.IsNullOrEmpty(settings))
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddAdditionalDocument(documentId, AnalyzersSettings.AnalyzerFileName, settings);
                }
                */

                return solution.AddMetadataReferences(projectId, metadataReferences)
                    .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            }
        }
    }
}