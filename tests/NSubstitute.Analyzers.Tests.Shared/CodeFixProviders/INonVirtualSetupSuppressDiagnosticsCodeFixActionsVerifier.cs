using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface INonVirtualSetupSuppressDiagnosticsCodeFixActionsVerifier
    {
        Task CreatesCorrectCodeFixActions_ForIndexer();

        Task CreatesCorrectCodeFixActions_ForProperty();

        Task CreatesCorrectCodeFixActions_ForMethod();
    }
}