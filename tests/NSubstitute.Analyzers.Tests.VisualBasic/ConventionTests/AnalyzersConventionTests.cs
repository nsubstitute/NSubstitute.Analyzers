using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Tests.Shared.Fixtures;
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
        public void DiagnosticAnalyzersAttributeConventionsShouldBeSatisfied()
        {
            _fixture.AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining<NonSubstitutableMemberAnalyzer>(LanguageNames.VisualBasic);
        }

        [Fact]
        public void DiagnosticAnalyzersInheritanceHierarchyShouldBeSatisfied()
        {
            _fixture.AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining<NonSubstitutableMemberAnalyzer>();
        }

        [Fact]
        public void CodeFixProvidersAttributeConventionsShouldBeSatisfied()
        {
            _fixture.AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining<NonSubstitutableMemberAnalyzer>(LanguageNames.VisualBasic);
        }
    }
}