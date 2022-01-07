using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests;

public abstract class VisualBasicCodeFixVerifier : CodeFixVerifier
{
    protected VisualBasicCodeFixVerifier()
        : this(VisualBasicWorkspaceFactory.Default)
    {
    }

    protected VisualBasicCodeFixVerifier(WorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }
}