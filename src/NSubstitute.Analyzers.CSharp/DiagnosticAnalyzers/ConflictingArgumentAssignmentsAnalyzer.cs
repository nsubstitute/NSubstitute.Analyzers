using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax, ElementAccessExpressionSyntax>
    {
        public ConflictingArgumentAssignmentsAnalyzer()
            : base(new DiagnosticDescriptorsProvider(), new CallInfoCallFinder())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<ExpressionSyntax> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.Expression);
        }

        protected override SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.GetParentInvocationExpression();
        }

        protected override int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            var position = syntaxNodeAnalysisContext.SemanticModel.GetConstantValue(indexerExpressionSyntax.ArgumentList.Arguments.First().Expression);
            return (int?)(position.HasValue ? position.Value : null);
        }

        protected override ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            return syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax).Symbol ??
                   syntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(indexerExpressionSyntax.Expression).Symbol;
        }

        protected override SyntaxNode GetAssignmentExpression(ElementAccessExpressionSyntax indexerExpressionSyntax)
        {
            if (indexerExpressionSyntax.Parent is AssignmentExpressionSyntax assignmentExpressionSyntax)
            {
                return assignmentExpressionSyntax.Right;
            }

            return null;
        }
    }
}