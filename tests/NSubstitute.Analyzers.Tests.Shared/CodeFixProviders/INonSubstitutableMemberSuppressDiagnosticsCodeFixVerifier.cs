using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface INonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
{
    Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualMethod();

    Task SuppressesDiagnosticsInSettings_WhenUsedWithStaticMethod();

    Task SuppressesDiagnosticsInSettings_WhenUsedWithExtensionMethod();

    Task SuppressesDiagnosticsInSettings_WhenUsedWithSealedOverrideMethod();

    Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualProperty();

    Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualIndexer();

    Task SuppressesDiagnosticsInSettingsForClass_WhenUsedWithNonVirtualMember_AndSelectingClassSuppression();

    Task SuppressesDiagnosticsInSettingsForNamespace_WhenUsedWithNonVirtualMember_AndSelectingNamespaceSuppression();
}