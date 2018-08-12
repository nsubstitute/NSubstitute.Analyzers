using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class CallInfoAnalyzer : AbstractCallInfoAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ElementAccessExpressionSyntax>
    {
        public CallInfoAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override SyntaxNode GetParentMethodCall(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.Expression.DescendantNodes().First();
        }

        protected override IEnumerable<ExpressionSyntax> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.Expression);
        }

        protected override AbstractCallInfoFinder<InvocationExpressionSyntax, ElementAccessExpressionSyntax> GetCallInfoFinder()
        {
            return new CallInfoCallFinder();
        }

        protected override SyntaxNode GetCastTypeExpression(ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            switch (indexerExpressionSyntax.Parent)
            {
                case BinaryExpressionSyntax binaryExpressionSyntax when binaryExpressionSyntax.OperatorToken.Kind() == SyntaxKind.AsKeyword:
                    return binaryExpressionSyntax.Right;
                case CastExpressionSyntax castExpressionSyntax:
                    return castExpressionSyntax.Type;
                default:
                    return null;
            }
        }

        protected override SyntaxNode GetAssignmentExpression(ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                return assignmentExpressionSyntax.Right;
            }

            return null;
        }

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            return syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax).Symbol ??
                   syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax.Expression).Symbol;
        }

        protected override int? GetArgAtPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(invocationExpressionSyntax.ArgumentList.Arguments.First().Expression);
            return (int?)(position.HasValue ? position.Value : null);
        }

        protected override int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(indexerExpressionSyntax.ArgumentList.Arguments.First().Expression);
            return (int?)(position.HasValue ? position.Value : null);
        }
    }
}