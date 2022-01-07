using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests;

public abstract class CSharpSuppressDiagnosticSettingsVerifier : SuppressDiagnosticSettingsVerifier
{
    protected CSharpSuppressDiagnosticSettingsVerifier()
        : base(CSharpWorkspaceFactory.Default)
    {
    }
}