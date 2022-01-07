using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests;

public abstract class CSharpDiagnosticVerifier : DiagnosticVerifier
{
    protected CSharpDiagnosticVerifier()
        : base(CSharpWorkspaceFactory.Default)
    {
    }
}