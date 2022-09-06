using System.Collections.Generic;
using System.Linq;
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
    public static IEnumerable<object[]> DiagnosticIndicesTestCases =>
        Enumerable.Range(0, 3).Select(item => new object[] { item });

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new SubstituteForInternalMemberCodeFixProvider();

    [Theory]
    [MemberData(nameof(DiagnosticIndicesTestCases))]
    public abstract Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass(int diagnosticIndex);

    [Theory]
    [MemberData(nameof(DiagnosticIndicesTestCases))]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass(int diagnosticIndex);

    [Theory]
    [MemberData(nameof(DiagnosticIndicesTestCases))]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty(int diagnosticIndex);

    [Theory]
    [MemberData(nameof(DiagnosticIndicesTestCases))]
    public abstract Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass(int diagnosticIndex);

    [Fact]
    public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass();

    [Fact]
    public abstract Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2();
}