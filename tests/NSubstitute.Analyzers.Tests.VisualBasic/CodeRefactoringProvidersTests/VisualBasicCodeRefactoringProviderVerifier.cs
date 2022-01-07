using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeRefactoringProvidersTests;

public abstract class VisualBasicCodeRefactoringProviderVerifier : CodeRefactoringProviderVerifier
{
    protected VisualBasicCodeRefactoringProviderVerifier()
        : base(VisualBasicWorkspaceFactory.Default)
    {
    }
}