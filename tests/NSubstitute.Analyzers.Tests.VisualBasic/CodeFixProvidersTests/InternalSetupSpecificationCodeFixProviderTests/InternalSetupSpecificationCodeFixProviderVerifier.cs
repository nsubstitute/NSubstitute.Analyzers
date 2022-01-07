using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.InternalSetupSpecificationCodeFixProviderTests;

public abstract class InternalSetupSpecificationCodeFixProviderVerifier : VisualBasicCodeFixVerifier, IInternalSetupSpecificationCodeFixProviderVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new InternalSetupSpecificationCodeFixProvider();

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ChangesInternalToPublic_ForIndexer_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ChangesInternalToPublic_ForProperty_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ChangesInternalToPublic_ForMethod_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task AppendsProtectedInternal_ToIndexer_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task AppendsProtectedInternal_ToProperty_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task AppendsProtectedInternal_ToMethod_WhenUsedWithInternalMember(string method);

    [CombinatoryTheory]
    [InlineData(".Bar")]
    [InlineData(".FooBar()")]
    [InlineData("(0)")]
    public abstract Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalMember(string method, string call);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task AppendsInternalsVisibleToWithFullyQualifiedName_WhenUsedWithInternalMemberAndCompilerServicesNotImported(string method);
}