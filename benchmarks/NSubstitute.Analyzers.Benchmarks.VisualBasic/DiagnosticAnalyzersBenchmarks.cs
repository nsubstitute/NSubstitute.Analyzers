using System.Reflection;
using BenchmarkDotNet.Attributes;
using NSubstitute.Analyzers.Benchmarks.Shared;
using NSubstitute.Analyzers.Benchmarks.VisualBasic.Source.DiagnosticsSources;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Benchmarks.VisualBasic
{
    [CoreJob]
    [MemoryDiagnoser]
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
            SourceProjectFolderName = "NSubstitute.Analyzers.Benchmarks.VisualBasic.Source";
            BenchmarkSourceAssembly = typeof(NonSubstitutableMemberDiagnosticSource).Assembly;
                
            CallInfoAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new CallInfoAnalyzer());
            ConflictingArgumentAssignmentsAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new ConflictingArgumentAssignmentsAnalyzer());
            NonSubstitutableMemberAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new NonSubstitutableMemberAnalyzer());
            NonSubstitutableMemberReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new NonSubstitutableMemberReceivedAnalyzer());
            NonSubstitutableMemberWhenAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new NonSubstitutableMemberWhenAnalyzer());
            ReEntrantSetupAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new ReEntrantSetupAnalyzer());
            SubstituteAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new SubstituteAnalyzer());
            UnusedReceivedAnalyzerBenchmark = AnalyzerBenchmark.CreateVisualBasicBenchmark(Solution, new UnusedReceivedAnalyzer());
        }
    }
}