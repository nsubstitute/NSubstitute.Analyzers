using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer
{
    public ReEntrantSetupAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, ReEntrantCallFinder.Instance)
    {
    }
}