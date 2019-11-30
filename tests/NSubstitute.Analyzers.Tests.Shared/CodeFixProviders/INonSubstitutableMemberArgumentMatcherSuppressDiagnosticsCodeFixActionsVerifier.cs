using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface INonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixActionsVerifier
    {
        Task CreatesCorrectCodeFixActions_ForIndexer();

        Task CreatesCorrectCodeFixActions_ForMethod();

        Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInStandaloneExpression();

        Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInConstructor();
    }
}