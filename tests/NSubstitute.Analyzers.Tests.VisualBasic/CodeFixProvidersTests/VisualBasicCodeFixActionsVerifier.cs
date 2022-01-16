using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests;

public abstract class VisualBasicCodeFixActionsVerifier : CodeFixCodeActionsVerifier
{
    protected VisualBasicCodeFixActionsVerifier()
        : this(VisualBasicWorkspaceFactory.Default)
    {
    }

    protected VisualBasicCodeFixActionsVerifier(VisualBasicWorkspaceFactory factory)
        : base(factory)
    {
    }
}