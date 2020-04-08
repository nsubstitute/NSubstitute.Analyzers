using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReceivedInReceivedInOrderAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
    {
        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;
        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected AbstractReceivedInReceivedInOrderAnalyzer(
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder,
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _substitutionNodeFinder = substitutionNodeFinder;
            _analyzeInvocationAction = AnalyzeInvocation;
            SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder);
        }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            if (methodSymbolInfo.Symbol.IsReceivedInOrderMethod() == false)
            {
                return;
            }

            foreach (var syntaxNode in _substitutionNodeFinder.FindForReceivedInOrderExpression(
                syntaxNodeContext,
                invocationExpression,
                (IMethodSymbol)methodSymbolInfo.Symbol))
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode);

                if (symbolInfo.Symbol.IsReceivedLikeMethod() == false)
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ReceivedUsedInReceivedInOrder,
                    syntaxNode.GetLocation(),
                    symbolInfo.Symbol.Name);

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }
    }
}