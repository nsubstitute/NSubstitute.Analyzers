using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class CallInfoAnalyzer : AbstractCallInfoAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, InvocationExpressionSyntax>
    {
        public CallInfoAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override SyntaxNode GetSubstituteCall(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.Expression.DescendantNodes().First();
        }

        protected override IEnumerable<ExpressionSyntax> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.GetExpression());
        }

        protected override AbstractCallInfoFinder<InvocationExpressionSyntax, InvocationExpressionSyntax> GetCallInfoFinder()
        {
            return new CallInfoCallFinder();
        }

        protected override SyntaxNode GetCastTypeExpression(InvocationExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is CastExpressionSyntax castExpressionSyntax)
            {
                return castExpressionSyntax.Type;
            }

            return null;
        }

        protected override SyntaxNode GetAssignmentExpression(InvocationExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is AssignmentStatementSyntax assignmentStatementSyntax)
            {
                return assignmentStatementSyntax.Right;
            }

            return null;
        }

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax indexerExpressionSyntax)
        {
            return syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax).Symbol ??
                   syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax.Expression).Symbol;
        }

        protected override int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var argAtPosition = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(invocationExpressionSyntax.ArgumentList.Arguments.First().GetExpression());

            return (int?)(argAtPosition.HasValue ? argAtPosition.Value : null);
        }

        protected override int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax indexerExpressionSyntax)
        {
            var indexerPosition = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(indexerExpressionSyntax.ArgumentList.Arguments.First().GetExpression());

            return (int?)(indexerPosition.HasValue ? indexerPosition.Value : null);
        }
    }
}