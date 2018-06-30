using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class NonVirtualSetupWhenAnalyzer : AbstractDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteWhenMethod,
            MetadataNames.NSubstituteWhenForAnyArgsMethod);

        public NonVirtualSetupWhenAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (InvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;
            if (methodSymbol == null)
            {
                return;
            }

            if (IsWhenLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
            {
                return;
            }

            var argumentSyntax = invocationExpression.ArgumentList.Arguments.First();
            var typeSymbol = methodSymbol.TypeArguments.First();

            var expressionsForAnalysys = GetExpressionsForAnalysys(syntaxNodeContext, argumentSyntax.Expression);

            foreach (var analysedSyntax in expressionsForAnalysys)
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(analysedSyntax);
                if (symbolInfo.Symbol != null && symbolInfo.Symbol.ContainingType == typeSymbol && symbolInfo.Symbol.CanBeSetuped() == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification,
                        analysedSyntax.GetLocation(),
                        symbolInfo.Symbol.Name);

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        private bool IsWhenLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
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
                case LocalFunctionStatementSyntax statementSyntax:
                    body =
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