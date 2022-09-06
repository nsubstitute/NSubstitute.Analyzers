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
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForExtensionMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression();

    [Fact]
    public abstract Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression();
}