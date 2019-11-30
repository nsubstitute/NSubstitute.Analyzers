using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface INonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
    {
        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForExtensionMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer();

        Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression();

        Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression();
    }
}