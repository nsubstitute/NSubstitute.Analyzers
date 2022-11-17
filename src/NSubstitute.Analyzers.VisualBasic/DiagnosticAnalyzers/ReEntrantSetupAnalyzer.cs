using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer
{
    public ReEntrantSetupAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, ReEntrantCallFinder.Instance)
    {
    }
}