using NSubstitute.Analyzers.Benchmarks.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests
{
    public class BenchmarksConventionTests : IClassFixture<BenchmarksConventionFixture>
    {
        private readonly BenchmarksConventionFixture _fixture;

        public BenchmarksConventionTests(BenchmarksConventionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AssertBenchmarksPresenceForDiagnosticAnalyzers()
        {
            _fixture.AssertBenchmarksPresenceForDiagnosticAnalyzersFromAssemblyContaining(typeof(NonSubstitutableMemberAnalyzer).Assembly, typeof(DiagnosticAnalyzersBenchmarks).Assembly);
        }

        [Fact]
        public void AssertBenchmarksRunsAgainstCodeProducingAllDiagnostics()
        {
            _fixture.AssertBenchmarksProduceAllDiagnostics(typeof(DiagnosticAnalyzersBenchmarks).Assembly);
        }
    }
}