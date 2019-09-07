using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractArgumentMatcherCompilationAnalyzer<TInvocationExpressionSyntax, TMemberAccessExpressionSyntax, TArgumentSyntax>
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpressionSyntax : SyntaxNode
        where TArgumentSyntax : SyntaxNode
    {
        protected abstract ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; }

        protected abstract ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; }

        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

        private readonly IDiagnosticDescriptorsProvider _diagnosticDescriptorsProvider;

        private readonly ConcurrentBag<SyntaxNode> _receivedInOrderNodes = new ConcurrentBag<SyntaxNode>();

        private readonly ConcurrentBag<SyntaxNode> _whenNodes = new ConcurrentBag<SyntaxNode>();

        private readonly ConcurrentDictionary<SyntaxNode, List<SyntaxNode>> _potentialMisusedNodes = new ConcurrentDictionary<SyntaxNode, List<SyntaxNode>>();

        private readonly ConcurrentBag<SyntaxNode> _misusedNodes = new ConcurrentBag<SyntaxNode>();

        protected AbstractArgumentMatcherCompilationAnalyzer(
            ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder,
            IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        {
            _substitutionNodeFinder = substitutionNodeFinder;
            _diagnosticDescriptorsProvider = diagnosticDescriptorsProvider;
        }

        public void BeginAnalyzeArgMatchers(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
            var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

            if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
            {
                return;
            }

            var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

            switch (methodSymbol)
            {
                case ISymbol symbol when symbol.IsArgMatcherLikeMethod():
                    BeginAnalyzeArgLikeMethod(syntaxNodeContext, invocationExpression);
                    break;
                case ISymbol symbol when symbol.IsWhenLikeMethod():
                    BeginAnalyzeWhenLikeMethod(syntaxNodeContext, invocationExpression);
                    break;
                case ISymbol symbol when symbol.IsReceivedInOrderMethod():
                    BeginAnalyzeReceivedInOrderMethod(syntaxNodeContext, invocationExpression);
                    break;
            }
        }

        public void FinishAnalyzeArgMatchers(CompilationAnalysisContext compilationAnalysisContext)
        {
            // we dont need thread safety anymore - changing for hashset for faster lookup
            var whenNodes = _whenNodes.ToImmutableHashSet();
            var receivedInOrderNodes = _receivedInOrderNodes.ToImmutableHashSet();

            foreach (var potential in _potentialMisusedNodes)
            {
                if (whenNodes.Contains(potential.Key) == false && receivedInOrderNodes.Contains(potential.Key) == false)
                {
                    foreach (var arg in potential.Value)
                    {
                        var diagnostic = Diagnostic.Create(
                            _diagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall,
                            arg.GetLocation());

                        compilationAnalysisContext.ReportDiagnostic(diagnostic);
                    }
                }
            }

            foreach (var misusedNode in _misusedNodes)
            {
                var diagnostic = Diagnostic.Create(
                    _diagnosticDescriptorsProvider.ArgumentMatcherUsedWithoutSpecifyingCall,
                    misusedNode.GetLocation());

                compilationAnalysisContext.ReportDiagnostic(diagnostic);
            }
        }

        protected abstract SyntaxNode GetOperationSyntax(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TArgumentSyntax argumentExpression);

        protected abstract IEnumerable<SyntaxNode> TryGetArgumentExpressions(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode syntaxNode);

        private bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var parentNote = invocationExpressionSyntax.Parent;

            if (parentNote is TMemberAccessExpressionSyntax)
            {
                var child = parentNote.ChildNodes().Except(new[] { invocationExpressionSyntax }).FirstOrDefault();

                return child != null && IsSetupLikeMethod(syntaxNodeContext.SemanticModel.GetSymbolInfo(child).Symbol);
            }

            if (parentNote is TArgumentSyntax argumentExpression)
            {
                var operationSyntax = GetOperationSyntax(syntaxNodeContext, argumentExpression);
                return operationSyntax != null && IsSetupLikeMethod(syntaxNodeContext.SemanticModel.GetSymbolInfo(operationSyntax).Symbol);
            }

            return false;
        }

        private void BeginAnalyzeWhenLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression)
        {
            foreach (var syntaxNode in _substitutionNodeFinder.FindForWhenExpression(syntaxNodeContext, invocationExpression))
            {
                _whenNodes.Add(syntaxNode);
            }
        }

        private void BeginAnalyzeReceivedInOrderMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression)
        {
            foreach (var syntaxNode in _substitutionNodeFinder
                .FindForReceivedInOrderExpression(syntaxNodeContext, invocationExpression))
            {
                _receivedInOrderNodes.Add(syntaxNode);
            }
        }

        private void BeginAnalyzeArgLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression)
        {
            // find allowed enclosing expression
            var enclosingExpression = FindAllowedEnclosingExpression(invocationExpression);

            // if Arg is used with not allowed expression, find if it is used in ignored ones eg. var x = Arg.Any
            // as variable might be used later on
            if (enclosingExpression == null && FindIgnoredEnclosingExpression(invocationExpression) == null)
            {
                _misusedNodes.Add(invocationExpression);
            }

            if (enclosingExpression != null &&
                IsFollowedBySetupInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsPrecededByReceivedInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsUsedAlongWithArgInvokers(syntaxNodeContext, enclosingExpression) == false)
            {
                _potentialMisusedNodes.AddOrUpdate(
                    enclosingExpression,
                    node => new List<SyntaxNode> { invocationExpression },
                    (node, list) =>
                    {
                        list.Add(invocationExpression);
                        return list;
                    });
            }
        }

        private bool IsPrecededByReceivedInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var syntaxNodes = invocationExpressionSyntax.Parent.DescendantNodes().ToList();
            var index = syntaxNodes.IndexOf(invocationExpressionSyntax.DescendantNodes().First());

            if (index >= 0 && index + 1 < syntaxNodes.Count - 1)
            {
                var syntaxNode = syntaxNodes[index + 1];
                return syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol.IsReceivedLikeMethod();
            }

            return false;
        }

        private SyntaxNode FindAllowedEnclosingExpression(TInvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.GetAncestorNode(AllowedAncestorPaths);
        }

        private SyntaxNode FindIgnoredEnclosingExpression(TInvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.GetAncestorNode(IgnoredAncestorPaths);
        }

        private bool IsUsedAlongWithArgInvokers(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            return TryGetArgumentExpressions(syntaxNodeContext, invocationExpressionSyntax)
                .OfType<TInvocationExpressionSyntax>()
                .Select(syntaxNode => syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol).Any(symbol => symbol.IsArgInvokerLikeMethod());
        }

        private bool IsSetupLikeMethod(ISymbol symbol)
        {
            return symbol.IsReturnLikeMethod() || symbol.IsThrowLikeMethod();
        }
    }
}