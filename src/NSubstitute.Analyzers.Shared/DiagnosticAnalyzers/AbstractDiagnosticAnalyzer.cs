using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        protected IDiagnosticDescriptorsProvider DiagnosticDescriptorsProvider { get; }

        protected AbstractDiagnosticAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        {
            DiagnosticDescriptorsProvider = diagnosticDescriptorsProvider;
        }
    }
}