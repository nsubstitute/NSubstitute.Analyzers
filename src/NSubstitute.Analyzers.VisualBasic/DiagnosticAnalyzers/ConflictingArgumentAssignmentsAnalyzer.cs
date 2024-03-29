using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer
{
    public ConflictingArgumentAssignmentsAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, CallInfoFinder.Instance)
    {
    }
}