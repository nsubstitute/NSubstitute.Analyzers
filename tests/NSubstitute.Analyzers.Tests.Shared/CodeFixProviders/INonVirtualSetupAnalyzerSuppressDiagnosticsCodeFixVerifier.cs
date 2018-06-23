using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface INonVirtualSetupAnalyzerSuppressDiagnosticsCodeFixVerifier
    {
        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty();

        Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer();
    }
}