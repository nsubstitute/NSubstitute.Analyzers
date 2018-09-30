using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface ISubstituteForInternalMemberCodeFixActionsVerifier
    {
        Task CreatesCorrectCodeFixActions_WhenSourceForInternalType_IsAvailable();

        Task Does_Not_CreateCodeFixActions_WhenType_IsNotInternal();

        Task Does_Not_CreateCodeFixActions_WhenSourceForInternalType_IsNotAvailable();
    }
}