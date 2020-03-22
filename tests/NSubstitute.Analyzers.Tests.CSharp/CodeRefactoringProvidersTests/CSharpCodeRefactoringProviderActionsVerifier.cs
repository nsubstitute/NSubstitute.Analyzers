using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeRefactoringProvidersTests
{
    public abstract class CSharpCodeRefactoringProviderActionsVerifier : CodeRefactoringProviderActionsVerifier
    {
        protected CSharpCodeRefactoringProviderActionsVerifier()
            : base(CSharpWorkspaceFactory.Default)
        {
        }
    }
}