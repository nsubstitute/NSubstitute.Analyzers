using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Benchmarks.Shared;

[CoreJob]
[MemoryDiagnoser]
public abstract class AbstractDiagnosticAnalyzersBenchmarks
{
    internal Solution Solution => _solutionProxy.Value;

    protected abstract AnalyzerBenchmark CallInfoAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark ConflictingArgumentAssignmentsAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark NonSubstitutableMemberAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark NonSubstitutableMemberReceivedAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark NonSubstitutableMemberWhenAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark NonSubstitutableMemberReceivedInOrderAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark ReEntrantSetupAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark SubstituteAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark UnusedReceivedAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark ArgumentMatcherAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark ReceivedInReceivedInOrderAnalyzerBenchmark { get; }

    protected abstract AnalyzerBenchmark AsyncReceivedInOrderCallbackAnalyzerBenchmark { get; }

    protected abstract AbstractSolutionLoader SolutionLoader { get; }

    protected abstract string SourceProjectFolderName { get; }

    protected abstract Assembly BenchmarkSourceAssembly { get; }

    private readonly Lazy<Solution> _solutionProxy;

    protected AbstractDiagnosticAnalyzersBenchmarks()
    {
        _solutionProxy = new Lazy<Solution>(CreateSolution);
    }

    [Benchmark]
    public void CallInfoAnalyzer()
    {
        CallInfoAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void ConflictingArgumentAssignmentsAnalyzer()
    {
        ConflictingArgumentAssignmentsAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void NonSubstitutableMemberAnalyzer()
    {
        NonSubstitutableMemberAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void NonSubstitutableMemberReceivedAnalyzer()
    {
        NonSubstitutableMemberReceivedAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void NonSubstitutableMemberWhenAnalyzer()
    {
        NonSubstitutableMemberWhenAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void NonSubstitutableMemberReceivedInOrderAnalyzer()
    {
        NonSubstitutableMemberReceivedInOrderAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void ReEntrantSetupAnalyzer()
    {
        ReEntrantSetupAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void SubstituteAnalyzer()
    {
        SubstituteAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void UnusedReceivedAnalyzer()
    {
        UnusedReceivedAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void ArgumentMatcherAnalyzer()
    {
        ArgumentMatcherAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void ReceivedInReceivedInOrderAnalyzer()
    {
        ReceivedInReceivedInOrderAnalyzerBenchmark.Run();
    }

    [Benchmark]
    public void AsyncReceivedInOrderCallbackAnalyzer()
    {
        AsyncReceivedInOrderCallbackAnalyzerBenchmark.Run();
    }

    [IterationCleanup(Target = nameof(ArgumentMatcherAnalyzer))]
    public void CleanUp()
    {
        // clearing previously captured actions, as solution-wide analyzer keeps state
        ArgumentMatcherAnalyzerBenchmark.RefreshActions();
    }

    private string GetBenchmarkSourceProjectPath()
    {
        var rootDirectory = FindRootDirectory();

        return Path.Combine(
            rootDirectory,
            "benchmarks",
            SourceProjectFolderName);
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

    private MetadataReference[] GetMetadataReferences(Assembly assembly)
    {
        var portableExecutableReferences = RecursiveReferencedAssemblies(assembly).Select(referencedAssembly => MetadataReference.CreateFromFile(referencedAssembly.Location))
            .ToArray();

        return portableExecutableReferences;
    }

    private static HashSet<Assembly> RecursiveReferencedAssemblies(
        Assembly assembly,
        HashSet<Assembly> recursiveAssemblies = null)
    {
        recursiveAssemblies = recursiveAssemblies ?? new HashSet<Assembly>();
        if (recursiveAssemblies.Add(assembly))
        {
            foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
            {
                Assembly result;
                if (TryGetOrLoad(referencedAssembly, out result))
                    RecursiveReferencedAssemblies(result, recursiveAssemblies);
            }
        }

        return recursiveAssemblies;
    }

    private Solution CreateSolution()
    {
        var excludedMetaAssemblyName = $"{BenchmarkSourceAssembly.GetName().Name}.dll";
        var metadataReferences = GetMetadataReferences(BenchmarkSourceAssembly)
            .Where(meta => meta.Display.EndsWith(excludedMetaAssemblyName) == false).ToArray();

        var benchmarkSourceProjectPath = GetBenchmarkSourceProjectPath();
        var solution = SolutionLoader.CreateSolution(benchmarkSourceProjectPath, metadataReferences);

        var projectGraph = solution.GetProjectDependencyGraph();
        foreach (var projectId in projectGraph.GetTopologicallySortedProjects())
        {
            var projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;
            using (var stream = new MemoryStream())
            {
                var result = projectCompilation.Emit(stream);
                if (result.Success == false)
                {
                    throw new InvalidOperationException($"Compilation for benchmark source failed {Environment.NewLine} {string.Join(Environment.NewLine, result.Diagnostics.Select(diag => diag.ToString()))}");
                }
            }
        }

        return solution;
    }

    private static bool TryGetOrLoad(AssemblyName name, out Assembly result)
    {
        try
        {
            result = Assembly.Load(name);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }
}