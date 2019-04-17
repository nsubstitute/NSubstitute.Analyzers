using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    /// <summary>
    /// Finds nodes which are considered to be a part of substitution call. For instance substitute.Bar().Returns(1) will return substitute.Bar()
    /// </summary>
    internal class SubstitutionNodeFinder : AbstractSubstitutionNodeFinder<InvocationExpressionSyntax>
    {
        public override SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol)
        {
            var parentInvocationExpression = invocationExpression?.GetParentInvocationExpression();
            if (parentInvocationExpression == null)
            {
                return null;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(parentInvocationExpression);

            return symbol.Symbol is IMethodSymbol methodSymbol && methodSymbol.ReducedFrom == null
                ? parentInvocationExpression.ArgumentList.Arguments.First().Expression
                : parentInvocationExpression.Expression.DescendantNodes().First();
        }

        public override SyntaxNode FindForStandardExpression(InvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol invocationExpressionSymbol)
        {
            switch (invocationExpressionSymbol.MethodKind)
            {
                case MethodKind.ReducedExtension:
                    return invocationExpressionSyntax.Expression.DescendantNodes().First();
                case MethodKind.Ordinary:
                    return invocationExpressionSyntax.ArgumentList.Arguments.First().Expression;
                default:
                    return null;
            }
        }

        protected override InvocationExpressionSyntax GetParentInvocationExpression(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.GetParentInvocationExpression();
        }

        protected override IEnumerable<SyntaxNode> FindForWhenExpressionInternal(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax whenInvocationExpression, IMethodSymbol whenInvocationSymbol)
        {
            var argumentExpression = whenInvocationSymbol.MethodKind == MethodKind.ReducedExtension
                ? whenInvocationExpression.ArgumentList.Arguments.First().Expression
                : whenInvocationExpression.ArgumentList.Arguments.Skip(1).First().Expression;

            return FindForWhenExpression(syntaxNodeContext, argumentExpression);
        }

        private IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode argumentSyntax)
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

                        foreach (var expressionForAnalysis in FindForWhenExpression(syntaxNodeContext, syntaxNode))
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