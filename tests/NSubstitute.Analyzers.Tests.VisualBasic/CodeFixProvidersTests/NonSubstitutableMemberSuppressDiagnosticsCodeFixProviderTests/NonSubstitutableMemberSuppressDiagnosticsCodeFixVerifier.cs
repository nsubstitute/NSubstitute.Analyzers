using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.NonSubstitutableMemberSuppressDiagnosticsCodeFixProviderTests;

public abstract class NonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier : VisualBasicSuppressDiagnosticSettingsVerifier, INonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new NonSubstitutableMemberSuppressDiagnosticsCodeFixProvider();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithStaticMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithExtensionMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithSealedOverrideMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualProperty();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualIndexer();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettingsForClass_WhenUsedWithNonVirtualMember_AndSelectingClassSuppression();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettingsForNamespace_WhenUsedWithNonVirtualMember_AndSelectingNamespaceSuppression();
}