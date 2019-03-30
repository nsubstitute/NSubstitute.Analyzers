using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    /// <summary>
    /// Finds calls considered to be substitute calls in expressions
    /// </summary>
    internal class WhenSubstituteCallFinder
    {
        public IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode argumentSyntax)
        {
            SyntaxNode body = null;
            switch (argumentSyntax)
            {
                case SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax:
                    body = simpleLambdaExpressionSyntax.Body;
                    break;
                case AnonymousFunctionExpressionSyntax anonymousFunctionExpressionSyntax:
                    body = anonymousFunctionExpressionSyntax.Body;
                    break;
                case LocalFunctionStatementSyntax localFunctionStatementSyntax:
                    body = (SyntaxNode)localFunctionStatementSyntax.Body ?? localFunctionStatementSyntax.ExpressionBody;
                    break;
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    body = (SyntaxNode)methodDeclarationSyntax.Body ?? methodDeclarationSyntax.ExpressionBody;
                    break;
                case IdentifierNameSyntax identifierNameSyntax:
                    var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(identifierNameSyntax);
                    if (symbol.Symbol != null && symbol.Symbol.Locations.Any())
                    {
                        var location = symbol.Symbol.Locations.First();
                        var syntaxNode = location.SourceTree.GetRoot().FindNode(location.SourceSpan);

                        foreach (var expressionForAnalysis in Find(syntaxNodeContext, syntaxNode))
                        {
                            yield return expressionForAnalysis;
                        }
                    }

                    break;
            }

            if (body == null)
            {
                yield break;
            }

            foreach (var invocationExpressionSyntax in body.DescendantNodes().Where(node => node.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                                                            node.IsKind(SyntaxKind.ElementAccessExpression)))
            {
                yield return invocationExpressionSyntax;
            }
        }
    }
}