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
                ? parentInvocationExpression.ArgumentList.Arguments.First().GetExpression()
                : parentInvocationExpression.Expression.DescendantNodes().First();
        }

        public override SyntaxNode FindForStandardExpression(InvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol invocationExpressionSymbol)
        {
            switch (invocationExpressionSymbol.MethodKind)
            {
                case MethodKind.ReducedExtension:
                    return invocationExpressionSyntax.Expression.DescendantNodes().First();
                case MethodKind.Ordinary:
                    return invocationExpressionSyntax.ArgumentList.Arguments.First().GetExpression();
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
                ? whenInvocationExpression.ArgumentList.Arguments.First().GetExpression()
                : whenInvocationExpression.ArgumentList.Arguments.Skip(1).First().GetExpression();

            return FindForWhenExpression(syntaxNodeContext, argumentExpression);
        }

        private IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode argumentSyntax)
        {
            SyntaxNode body = null;
            switch (argumentSyntax)
            {
                case SingleLineLambdaExpressionSyntax _:
                case ExpressionStatementSyntax _:
                case LocalDeclarationStatementSyntax _:
                case AssignmentStatementSyntax _:
                    body = argumentSyntax;
                    break;
                case MultiLineLambdaExpressionSyntax simpleLambdaExpressionSyntax:
                    foreach (var syntaxNode in IterateStatements(simpleLambdaExpressionSyntax.Statements))
                    {
                        yield return syntaxNode;
                    }

                    break;
                case MethodBlockSyntax methodBlockSyntax:
                    foreach (var syntaxNode in IterateStatements(methodBlockSyntax.Statements))
                    {
                        yield return syntaxNode;
                    }

                    break;
                case UnaryExpressionSyntax unaryExpressionSyntax:
                    foreach (var syntaxNode in FindForWhenExpression(syntaxNodeContext, unaryExpressionSyntax.Operand))
                    {
                        yield return syntaxNode;
                    }

                    break;
                case IdentifierNameSyntax identifierNameSyntax:
                    var symbol = ModelExtensions.GetSymbolInfo(syntaxNodeContext.SemanticModel, identifierNameSyntax);
                    if (symbol.Symbol != null && symbol.Symbol.Locations.Any())
                    {
                        var location = symbol.Symbol.Locations.First();
                        var syntaxNode = location.SourceTree.GetRoot().FindNode(location.SourceSpan);

                        SyntaxNode innerNode = null;
                        if (syntaxNode is MethodStatementSyntax methodStatementSyntax)
                        {
                            innerNode = methodStatementSyntax.Parent;
                        }

                        innerNode = innerNode ?? syntaxNode;
                        foreach (var expressionsForAnalysy in FindForWhenExpression(syntaxNodeContext, innerNode))
                        {
                            yield return expressionsForAnalysy;
                        }
                    }

                    break;
            }

            if (body == null)
            {
                yield break;
            }

            var memberAccessExpressions = body.DescendantNodes().Where(node => node.IsKind(SyntaxKind.SimpleMemberAccessExpression)).ToList();
            var invocationExpressions = body.DescendantNodes().Where(node => node.IsKind(SyntaxKind.InvocationExpression)).ToList();

            // rather ugly but prevents reporting two times the same thing
            // as VB syntax is based on statements, you can't access body of method directly
            if (invocationExpressions.Any())
            {
                foreach (var invocationExpression in invocationExpressions)
                {
                    yield return invocationExpression;
                }
            }
            else if (memberAccessExpressions.Any())
            {
                foreach (var memberAccessExpression in memberAccessExpressions)
                {
                    yield return memberAccessExpression;
                }
            }

            IEnumerable<SyntaxNode> IterateStatements(IEnumerable<StatementSyntax> statements)
            {
                return statements.SelectMany(statement => FindForWhenExpression(syntaxNodeContext, statement));
            }
        }
    }
}