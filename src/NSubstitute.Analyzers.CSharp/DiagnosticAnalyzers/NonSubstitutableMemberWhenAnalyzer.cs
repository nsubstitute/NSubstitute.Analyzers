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
    internal class NonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableMemberWhenAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax>
    {
        public NonSubstitutableMemberWhenAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<SyntaxNode> GetExpressionsForAnalysys(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var argumentListArguments = invocationExpressionSyntax.ArgumentList.Arguments;
            var argumentSyntax = methodSymbol.MethodKind == MethodKind.ReducedExtension ? argumentListArguments.First() : argumentListArguments.Skip(1).First();
            return GetExpressionsForAnalysys(syntaxNodeAnalysisContext, argumentSyntax.Expression);
        }

        private IEnumerable<SyntaxNode> GetExpressionsForAnalysys(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode argumentSyntax)
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

                        foreach (var expressionsForAnalysy in GetExpressionsForAnalysys(syntaxNodeContext, syntaxNode))
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

            foreach (var invocationExpressionSyntax in body.DescendantNodes().Where(node => node.IsKind(SyntaxKind.SimpleMemberAccessExpression) ||
                                                                                            node.IsKind(SyntaxKind.ElementAccessExpression)))
            {
                yield return invocationExpressionSyntax;
            }
        }
    }
}