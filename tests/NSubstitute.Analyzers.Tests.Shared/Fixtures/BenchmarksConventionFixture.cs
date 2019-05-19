using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures
{
    public class BenchmarksConventionFixture
    {
        public void AssertBenchmarksPresenceForDiagnosticAnalyzersFromAssemblyContaining(Assembly analyzersAssembly, Assembly benchmarksAssembly)
        {
            var allAnalyzers = analyzersAssembly.GetTypesAssignableTo<DiagnosticAnalyzer>();
            var benchmarkAnalyzersTypes = GetAnalyzerBenchmarks(benchmarksAssembly).Select(benchmark => benchmark.Benchmark.Analyzer.GetType());

            benchmarkAnalyzersTypes.Should().BeEquivalentTo(allAnalyzers, "because every analyzer should have a benchmark");
        }

        public void AssertBenchmarksProduceAllDiagnostics(Assembly benchmarksAssembly)
        {
            var benchmarkAnalyzers = GetAnalyzerBenchmarks(benchmarksAssembly);
            var allDiagnostics = benchmarkAnalyzers
                .SelectMany(benchmark => benchmark.Benchmark.Analyzer.SupportedDiagnostics.Select(diag => diag.Id))
                .Distinct()
                .OrderBy(diag => diag)
                .ToList();


            // TODO introduce sort of interface
            var producedDiagnostics = benchmarkAnalyzers.SelectMany(benchmark =>
                    Analyze.GetDiagnostics(benchmark.Field.DeclaringType
                        .GetField("Solution", BindingFlags.NonPublic | BindingFlags.Static)
                        .GetValue(benchmark.Field.DeclaringType) as Solution, benchmark.Benchmark.Analyzer))
                .SelectMany(diag => diag.Select(x => x.Id))
                .Distinct()
                .OrderBy(diag => diag)
                .ToList();

            producedDiagnostics.Should().BeEquivalentTo(allDiagnostics);
        }
        
        private static List<BenchmarkDescriptor> GetAnalyzerBenchmarks(Assembly benchmarksAssembly)
        {
            var benchmarkFields = benchmarksAssembly.GetTypes()
                .SelectMany(type => type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                .Where(fieldInfo => fieldInfo.FieldType == typeof(Benchmark))
                .ToList();

            var declaringInstances = benchmarkFields.GroupBy(fieldInfo => fieldInfo.DeclaringType)
                .ToDictionary(grouping => grouping.Key, grouping => Activator.CreateInstance(grouping.Key));

            var benchmarkAnalyzers = benchmarkFields
                .Select(benchmark => new BenchmarkDescriptor(benchmark, (Benchmark)benchmark.GetValue(declaringInstances[benchmark.DeclaringType])))
                .ToList();
            
            return benchmarkAnalyzers;
        }
        
        private class BenchmarkDescriptor
        {
            public FieldInfo Field { get; }
            
            public Benchmark Benchmark { get; }

            public BenchmarkDescriptor(FieldInfo field, Benchmark benchmark)
            {
                Field = field;
                Benchmark = benchmark;
            }
        }
    }
}