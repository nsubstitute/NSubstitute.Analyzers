using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests;

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
        _fixture.AssertDiagnosticAnalyzerAttributeUsageFromAssemblyContaining<NonSubstitutableMemberAnalyzer>(LanguageNames.CSharp);
    }

    [Fact]
    public void DiagnosticAnalyzersInheritanceHierarchyShouldBeSatisfied()
    {
        _fixture.AssertDiagnosticAnalyzerInheritanceFromAssemblyContaining<NonSubstitutableMemberAnalyzer>();
    }

    [Fact]
    public void CodeFixProvidersAttributeConventionsShouldBeSatisfied()
    {
        _fixture.AssertExportCodeFixProviderAttributeUsageFromAssemblyContaining<NonSubstitutableMemberAnalyzer>(LanguageNames.CSharp);
    }

    [Fact]
    public void CodeFixProvidersInheritanceHierarchyShouldBeSatisfied()
    {
        _fixture.AssertCodeFixProviderInheritanceFromAssemblyContaining<NonSubstitutableMemberAnalyzer>();
    }

    [Fact]
    public void CodeRefactoringProvidersAttributeConventionsShouldBeSatisfied()
    {
        _fixture.AssertExportCodeRefactoringProviderAttributeUsageFromAssemblyContaining<NonSubstitutableMemberAnalyzer>(LanguageNames.CSharp);
    }

    [Fact]
    public void CodeRefactoringProvidersInheritanceHierarchyShouldBeSatisfied()
    {
        _fixture.AssertCodeRefactoringProviderInheritanceFromAssemblyContaining<NonSubstitutableMemberAnalyzer>();
    }
}