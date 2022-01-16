using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests;

public abstract class CSharpCodeFixActionsVerifier : CodeFixCodeActionsVerifier
{
    protected CSharpCodeFixActionsVerifier()
        : this(CSharpWorkspaceFactory.Default)
    {
    }

    protected CSharpCodeFixActionsVerifier(CSharpWorkspaceFactory factory)
        : base(factory)
    {
    }
}