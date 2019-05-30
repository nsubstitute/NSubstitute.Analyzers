using System.Reflection;
using BenchmarkDotNet.Attributes;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources;
using NSubstitute.Analyzers.Benchmarks.Shared;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    [CoreJob]
    [BenchmarkCategory("VisualBasic")]
    public class DiagnosticAnalyzersBenchmarks : AbstractDiagnosticAnalyzersBenchmarks
    {
        protected override AnalyzerBenchmark CallInfoAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark ConflictingArgumentAssignmentsAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark NonSubstitutableMemberAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark NonSubstitutableMemberReceivedAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark NonSubstitutableMemberWhenAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark ReEntrantSetupAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark SubstituteAnalyzerBenchmark { get; }
        protected override AnalyzerBenchmark UnusedReceivedAnalyzerBenchmark { get; }
        protected override AbstractSolutionLoader SolutionLoader { get; }
        protected override string SourceProjectFolderName { get; }
        protected override Assembly BenchmarkSourceAssembly { get; }

        public DiagnosticAnalyzersBenchmarks()
        {
            SolutionLoader = new SolutionLoader();
            SourceProjectFolderName = "NSubstitute.Analyzers.Benchmarks.CSharp.Source";
            BenchmarkSourceAssembly = typeof(NonSubstitutableMemberDiagnosticSource).Assembly;
                
            CallInfoAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new CallInfoAnalyzer());
            ConflictingArgumentAssignmentsAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new ConflictingArgumentAssignmentsAnalyzer());
            NonSubstitutableMemberAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberAnalyzer());
            NonSubstitutableMemberReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberReceivedAnalyzer());
            NonSubstitutableMemberWhenAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new NonSubstitutableMemberWhenAnalyzer());
            ReEntrantSetupAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new ReEntrantSetupAnalyzer());
            SubstituteAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new SubstituteAnalyzer());
            UnusedReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateCSharpBenchmark(Solution, new UnusedReceivedAnalyzer());
        }
        
        
    }
}