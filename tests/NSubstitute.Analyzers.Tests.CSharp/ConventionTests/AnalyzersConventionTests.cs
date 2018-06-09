using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests
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
            _fixture.AssertDiagnosticAnalyzerAttributeUsageFormAssemblyContaining<NonVirtualSetupAnalyzer>(LanguageNames.CSharp);
        }
    }
}