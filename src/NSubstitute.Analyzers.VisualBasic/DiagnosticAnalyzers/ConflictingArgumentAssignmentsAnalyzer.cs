using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer<SyntaxKind, InvocationExpressionSyntax, InvocationExpressionSyntax>
    {
        public ConflictingArgumentAssignmentsAnalyzer()
            : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, CallInfoCallFinder.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}