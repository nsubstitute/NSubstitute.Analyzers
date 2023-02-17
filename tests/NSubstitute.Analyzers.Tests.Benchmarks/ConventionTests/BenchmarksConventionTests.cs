using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Benchmarks;
using NSubstitute.Analyzers.Benchmarks.Shared;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Benchmarks.ConventionTests;

public class BenchmarksConventionTests
{
    private static readonly Assembly[] AnalyzersAssemblies;
    private static readonly BenchmarkDescriptor[] BenchmarkDescriptors;

    private static readonly IReadOnlyList<string> SupportedLanguages =
        new[] { LanguageNames.CSharp, LanguageNames.VisualBasic };

    static BenchmarksConventionTests()
    {
        var benchmarksAssembly = typeof(Program).Assembly;
        AnalyzersAssemblies = new[] { typeof(CallInfoAnalyzer).Assembly, typeof(NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers.CallInfoAnalyzer).Assembly };
        BenchmarkDescriptors = GetAnalyzerBenchmarks(benchmarksAssembly).ToArray();
    }

    [Fact]
    public void BenchmarksShouldBeDefinedForAllAnalyzers()
    {
        var allAnalyzers = AnalyzersAssemblies.SelectMany(assembly => assembly.GetTypesAssignableTo<DiagnosticAnalyzer>());
        var benchmarkAnalyzersTypes = BenchmarkDescriptors.Select(benchmark => benchmark.Benchmark.Analyzer.GetType());

        benchmarkAnalyzersTypes.Should().BeEquivalentTo(allAnalyzers, "because every analyzer should have a benchmark");
    }

    [Fact]
    public async Task BenchmarksShouldProduceAllDiagnostics()
    {
        var allDiagnosticIds = BenchmarkDescriptors
            .SelectMany(benchmark => benchmark.Benchmark.Analyzer.SupportedDiagnostics.Select(diag => diag.Id))
            .Distinct()
            .OrderBy(diag => diag)
            .ToList();
        var expectedDiagnosticIds = SupportedLanguages
            .Select(language => new DiagnosticIdsWithLanguage(language, allDiagnosticIds)).ToList();

        var producedDiagnosticIds = await GetProducedDiagnosticIds();

        producedDiagnosticIds.Should().BeEquivalentTo(expectedDiagnosticIds, opts => opts.WithStrictOrdering());
    }

    private async Task<IReadOnlyList<DiagnosticIdsWithLanguage>> GetProducedDiagnosticIds()
    {
        var producedDiagnostics = await BenchmarkDescriptors.ToAsyncEnumerable().SelectAwait(async benchmark =>
        {
            var propertyInfo = benchmark.Property.DeclaringType!.GetProperty(
                nameof(AbstractDiagnosticAnalyzersBenchmarks.Solution),
                BindingFlags.Instance | BindingFlags.NonPublic);

            var solution = (Solution)propertyInfo!.GetValue(benchmark.DeclaringTypeInstance)!;
            return await GetDiagnostics(solution!, benchmark.Benchmark.Analyzer);
        }).ToListAsync();

        return producedDiagnostics.GroupBy(diagnostic => diagnostic.Language).Select(grouping =>
                new DiagnosticIdsWithLanguage(
                    grouping.Key,
                    grouping.SelectMany(diagWithAnalyzer => diagWithAnalyzer.DiagnosticIds).Distinct().OrderBy(diagId => diagId).ToList()))
            .OrderBy(diag => diag.Language)
            .ToList();
    }

    private async Task<DiagnosticsWithAnalyzer> GetDiagnostics(Solution solution, DiagnosticAnalyzer analyzer)
    {
        var diagnostics = new List<ImmutableArray<Diagnostic>>();
        foreach (var project in solution.Projects)
        {
            var result = await project.GetCompilationAsync();
            var compilationWithAnalyzers = result.WithAnalyzers(ImmutableArray.Create(analyzer), project.AnalyzerOptions);
            diagnostics.Add(await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync());
        }

        return new DiagnosticsWithAnalyzer(analyzer, diagnostics.SelectMany(diag => diag).ToList());
    }

    private static BenchmarkDescriptor[] GetAnalyzerBenchmarks(Assembly benchmarksAssembly)
    {
        var benchmarksBaseType = typeof(AbstractDiagnosticAnalyzersBenchmarks);
        var benchmarkFields = benchmarksAssembly.GetTypes()
            .Where(type => benchmarksBaseType.IsAssignableFrom(type))
            .SelectMany(type => type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            .Where(fieldInfo => fieldInfo.FieldType == typeof(AnalyzerBenchmark))
            .ToList();

        var declaringInstances = benchmarkFields.GroupBy(fieldInfo => fieldInfo.DeclaringType)
            .ToDictionary(grouping => grouping.Key!, grouping => (AbstractDiagnosticAnalyzersBenchmarks)Activator.CreateInstance(grouping.Key!)!);

        var benchmarkAnalyzers = benchmarkFields
            .Select(benchmark => new BenchmarkDescriptor(benchmark, (AnalyzerBenchmark)benchmark.GetValue(declaringInstances[benchmark.DeclaringType!])!, declaringInstances[benchmark.DeclaringType!]))
            .ToArray();

        return benchmarkAnalyzers;
    }

    private record BenchmarkDescriptor(FieldInfo Property, AnalyzerBenchmark Benchmark, AbstractDiagnosticAnalyzersBenchmarks DeclaringTypeInstance);

    private record DiagnosticsWithAnalyzer(DiagnosticAnalyzer Analyzer, IReadOnlyList<Diagnostic> Diagnostics)
    {
        public string Language { get; } =
            Analyzer.GetType().GetCustomAttribute<DiagnosticAnalyzerAttribute>()!.Languages.Single();

        public IReadOnlyList<string> DiagnosticIds => Diagnostics.Select(diag => diag.Id).ToList();
    }

    private record DiagnosticIdsWithLanguage(string Language, IReadOnlyList<string> DiagnosticIds);
}