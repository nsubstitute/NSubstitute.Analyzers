using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberWhenAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TSyntaxKind : struct
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);

        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteWhenMethod,
            MetadataNames.NSubstituteWhenForAnyArgsMethod);

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractNonSubstitutableMemberWhenAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
        }

        protected abstract IEnumerable<SyntaxNode> GetExpressionsForAnalysys(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpressionSyntax);

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
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

            var expressionsForAnalysys = GetExpressionsForAnalysys(syntaxNodeContext, methodSymbol, invocationExpression);
            var typeSymbol = methodSymbol.TypeArguments.FirstOrDefault() ?? methodSymbol.ReceiverType;
            foreach (var analysedSyntax in expressionsForAnalysys)
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(analysedSyntax);
                if (symbolInfo.Symbol != null && symbolInfo.Symbol.ContainingType == typeSymbol)
                {
                    var canBeSetuped = symbolInfo.Symbol.CanBeSetuped();
                    if (canBeSetuped == false)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification,
                            analysedSyntax.GetLocation(),
                            symbolInfo.Symbol.Name);

                        syntaxNodeContext.ReportDiagnostic(diagnostic);
                    }

                    if (canBeSetuped && symbolInfo.Symbol.MemberVisibleToProxyGenerator() == false)
                    {
                        var diagnostic = Diagnostic.Create(
                            DiagnosticDescriptorsProvider.InternalSetupSpecification,
                            analysedSyntax.GetLocation(),
                            symbolInfo.Symbol.Name);

                        syntaxNodeContext.ReportDiagnostic(diagnostic);
                    }
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
    }
}