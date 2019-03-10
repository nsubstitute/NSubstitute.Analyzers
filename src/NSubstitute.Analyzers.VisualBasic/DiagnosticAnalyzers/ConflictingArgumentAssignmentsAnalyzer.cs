using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, InvocationExpressionSyntax>
    {
        public ConflictingArgumentAssignmentsAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<ExpressionSyntax> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.GetExpression());
        }

        protected override SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.GetParentInvocationExpression();
        }

        protected override AbstractCallInfoFinder<InvocationExpressionSyntax, InvocationExpressionSyntax> GetCallInfoFinder()
        {
            return new CallInfoCallFinder();
        }

        protected override int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax indexerExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(indexerExpressionSyntax.ArgumentList.Arguments.First().GetExpression());
            return (int?)(position.HasValue ? position.Value : null);
        }

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax indexerExpressionSyntax)
        {
            return ModelExtensions.GetSymbolInfo(syntaxNodeAnalysisContext.SemanticModel, indexerExpressionSyntax).Symbol ??
                   ModelExtensions.GetSymbolInfo(syntaxNodeAnalysisContext.SemanticModel, indexerExpressionSyntax.Expression).Symbol;
        }

        protected override SyntaxNode GetAssignmentExpression(InvocationExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is AssignmentStatementSyntax assignmentExpressionSyntax)
            {
                return assignmentExpressionSyntax.Right;
            }

            return null;
        }
    }
}