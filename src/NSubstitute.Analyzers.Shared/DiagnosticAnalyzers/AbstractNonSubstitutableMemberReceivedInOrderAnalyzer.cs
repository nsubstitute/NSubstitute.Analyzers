using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractNonSubstitutableMemberReceivedInOrderAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TMemberAccessExpressionSyntax, TBlockStatementSyntax> : AbstractNonSubstitutableSetupAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpressionSyntax : SyntaxNode
        where TBlockStatementSyntax : SyntaxNode
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected abstract ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; }

        private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;
        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

        protected AbstractNonSubstitutableMemberReceivedInOrderAnalyzer(
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder,
            INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
        {
            _substitutionNodeFinder = substitutionNodeFinder;
            SupportedDiagnostics = ImmutableArray.Create(
                DiagnosticDescriptorsProvider.InternalSetupSpecification,
                DiagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification);
            _analyzeInvocationAction = AnalyzeInvocation;
            NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification;
        }

        protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

        protected sealed override void InitializeAnalyzer(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
        }

        protected abstract ISymbol GetDeclarationSymbol(SemanticModel semanticModel, SyntaxNode node);

        protected override Location GetSubstitutionNodeActualLocation(in NonSubstitutableMemberAnalysisResult analysisResult)
        {
            return analysisResult.Member.GetSubstitutionNodeActualLocation<TMemberAccessExpressionSyntax>(analysisResult.Symbol);
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
                (IMethodSymbol)methodSymbolInfo.Symbol).Where(node => ShouldAnalyzeNode(syntaxNodeContext.SemanticModel, node)))
            {
                var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode);

                if (symbolInfo.Symbol == null)
                {
                    return;
                }

                Analyze(syntaxNodeContext, syntaxNode, symbolInfo.Symbol);
            }
        }

        private bool ShouldAnalyzeNode(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var maybeIgnoredExpression = FindIgnoredEnclosingExpression(syntaxNode);
            if (maybeIgnoredExpression == null)
            {
                return true;
            }

            var symbol = GetDeclarationSymbol(semanticModel, maybeIgnoredExpression);

            if (symbol == null)
            {
                return false;
            }

            var blockStatementSyntax =
                maybeIgnoredExpression.Ancestors().OfType<TBlockStatementSyntax>().FirstOrDefault();

            if (blockStatementSyntax == null)
            {
                return false;
            }

            var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(blockStatementSyntax);
            return !dataFlowAnalysis.ReadInside.Contains(symbol);
        }

        private SyntaxNode FindIgnoredEnclosingExpression(SyntaxNode syntaxNode)
        {
            return syntaxNode.GetAncestorNode(IgnoredAncestorPaths);
        }
    }
}