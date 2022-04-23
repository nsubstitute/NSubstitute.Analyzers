using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer<SyntaxKind>
{
    public ConflictingArgumentAssignmentsAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, CallInfoCallFinder.Instance)
    {
    }

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}