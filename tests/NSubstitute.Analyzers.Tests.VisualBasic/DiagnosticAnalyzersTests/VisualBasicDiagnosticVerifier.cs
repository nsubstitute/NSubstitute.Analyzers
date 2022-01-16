using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests;

public abstract class VisualBasicDiagnosticVerifier : DiagnosticVerifier
{
    protected VisualBasicDiagnosticVerifier()
        : base(VisualBasicWorkspaceFactory.Default)
    {
    }
}