using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeRefactoringProvidersTests;

public abstract class CSharpCodeRefactoringProviderVerifier : CodeRefactoringProviderVerifier
{
    protected CSharpCodeRefactoringProviderVerifier()
        : base(CSharpWorkspaceFactory.Default)
    {
    }
}