using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.ConventionTests
{
    public class AnalyzersConventionTests : IClassFixture<AnalyzersConventionFixture>
    {
        private readonly AnalyzersConventionFixture _fixture;

        public AnalyzersConventionTests(AnalyzersConventionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void DiagnosticAnalyzersConventionsShouldBeSatisfied()
        {
            _fixture.AssertDiagnosticAnalyzerAttributeUsageFormAssemblyContaining<NonVirtualSetupAnalyzer>(LanguageNames.VisualBasic);
        }
    }
}