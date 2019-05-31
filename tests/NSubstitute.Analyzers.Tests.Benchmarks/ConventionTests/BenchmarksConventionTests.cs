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

namespace NSubstitute.Analyzers.Tests.Benchmarks.ConventionTests
{
    public class BenchmarksConventionTests
    {
        private static readonly Assembly[] AnalyzersAssemblies;
        private static readonly BenchmarkDescriptor[] BenchmarkDescriptors;
        
        static BenchmarksConventionTests()
        {
            var benchmarksAssembly = typeof(Program).Assembly;
            AnalyzersAssemblies = new [] { typeof(CallInfoAnalyzer).Assembly, typeof(NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers.CallInfoAnalyzer).Assembly};
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
        public void BenchmarksShouldProduceAllDiagnostics()
        {
            var allDiagnosticIds = BenchmarkDescriptors
                .SelectMany(benchmark => benchmark.Benchmark.Analyzer.SupportedDiagnostics.Select(diag => diag.Id))
                .Distinct()
                .OrderBy(diag => diag)
                .ToList();

            var producedDiagnostics = BenchmarkDescriptors.Select(async benchmark =>
            {
                var propertyInfo = benchmark.Property.DeclaringType.GetProperty(
                    nameof(AbstractDiagnosticAnalyzersBenchmarks.Solution),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                
                var solution = propertyInfo.GetValue(benchmark.DeclaringTypeInstance) as Solution;
                return await GetDiagnostics(solution, benchmark.Benchmark.Analyzer);
            }).SelectMany(task => task.Result).ToList();
            
            
            var producedDiagnosticIds = producedDiagnostics
                .SelectMany(diag => diag.Select(x => x.Id))
                .Distinct()
                .OrderBy(diag => diag)
                .ToList();

            producedDiagnosticIds.Should().BeEquivalentTo(allDiagnosticIds);
        }

        private async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnostics(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var diagnostics = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var result = await project.GetCompilationAsync();
                var compilationWithAnalyzers = result.WithAnalyzers(ImmutableArray.Create(analyzer), project.AnalyzerOptions);
                diagnostics.Add(await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync());
            }

            return diagnostics;
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
                .ToDictionary(grouping => grouping.Key, grouping => (AbstractDiagnosticAnalyzersBenchmarks)Activator.CreateInstance(grouping.Key));

            var benchmarkAnalyzers = benchmarkFields
                .Select(benchmark => new BenchmarkDescriptor(benchmark, (AnalyzerBenchmark)benchmark.GetValue(declaringInstances[benchmark.DeclaringType]), declaringInstances[benchmark.DeclaringType]))
                .ToArray();

            return benchmarkAnalyzers;
        }
        
        private class BenchmarkDescriptor
        {
            public FieldInfo Property { get; }

            public AnalyzerBenchmark Benchmark { get; }

            public AbstractDiagnosticAnalyzersBenchmarks DeclaringTypeInstance { get; }
            
            public BenchmarkDescriptor(FieldInfo property, AnalyzerBenchmark benchmark, AbstractDiagnosticAnalyzersBenchmarks declaringTypeInstance)
            {
                Property = property;
                Benchmark = benchmark;
                DeclaringTypeInstance = declaringTypeInstance;
            }
        }
        
        
    }
}