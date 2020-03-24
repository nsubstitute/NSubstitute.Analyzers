using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeRefactoringProvidersTests
{
    public abstract class VisualBasicCodeRefactoringProviderActionsVerifier : CodeRefactoringProviderActionsVerifier
    {
        protected VisualBasicCodeRefactoringProviderActionsVerifier()
            : base(VisualBasicWorkspaceFactory.Default)
        {
        }
    }
}