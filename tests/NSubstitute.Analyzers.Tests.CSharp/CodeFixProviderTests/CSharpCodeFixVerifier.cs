using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests;

public abstract class CSharpCodeFixVerifier : CodeFixVerifier
{
    protected CSharpCodeFixVerifier()
        : this(CSharpWorkspaceFactory.Default)
    {
    }

    protected CSharpCodeFixVerifier(CSharpWorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }
}