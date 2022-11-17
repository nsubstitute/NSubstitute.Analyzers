using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests;

public abstract class SubstituteForInternalMemberCodeFixVerifier : CSharpCodeFixVerifier, ISubstituteForInternalMemberCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new SubstituteForInternalMemberCodeFixProvider();

    [Fact]
    public abstract Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass();

    [Fact]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass();

    [Fact]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty();

    [Fact]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass();

    [Fact]
    public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass();

    [Fact]
    public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2();
}