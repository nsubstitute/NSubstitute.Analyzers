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
        protected abstract ImmutableArray<ImmutableArray<int>> PossibleAncestorPathsForArgument { get; }

        private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

        private readonly IDiagnosticDescriptorsProvider _diagnosticDescriptorsProvider;

        private HashSet<SyntaxNode> ReceivedInOrderNodes { get; } = new HashSet<SyntaxNode>();

        private HashSet<SyntaxNode> WhenNodes { get; } = new HashSet<SyntaxNode>();

        private Dictionary<SyntaxNode, List<SyntaxNode>> PotentialMisusedNodes { get; } = new Dictionary<SyntaxNode, List<SyntaxNode>>();

        protected AbstractArgumentMatcherCompilationAnalyzer(ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder, IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
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
            foreach (var potential in PotentialMisusedNodes)
            {
                if (WhenNodes.Contains(potential.Key) == false && ReceivedInOrderNodes.Contains(potential.Key) == false)
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
        }

        protected abstract SyntaxNode GetOperationSyntax(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TArgumentSyntax argumentExpression);

        protected abstract List<SyntaxNode> TryGetArgumentExpressions(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode syntaxNode);

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
                var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
                var actualNode = syntaxNode is TMemberAccessExpressionSyntax && symbol is IMethodSymbol _
                    ? syntaxNode.Parent
                    : syntaxNode;

                WhenNodes.Add(actualNode);
            }
        }

        private void BeginAnalyzeReceivedInOrderMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression)
        {
            foreach (var syntaxNode in _substitutionNodeFinder
                .FindForReceivedInOrderExpression(syntaxNodeContext, invocationExpression).ToList())
            {
                var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
                var actualNode = syntaxNode is TMemberAccessExpressionSyntax && symbol is IMethodSymbol _
                    ? syntaxNode.Parent
                    : syntaxNode;

                ReceivedInOrderNodes.Add(actualNode);
            }
        }

        private void BeginAnalyzeArgLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression)
        {
            // find enclosing type
            var enclosingExpression = FindEnclosingExpression(invocationExpression);

            if (enclosingExpression != null &&
                IsFollowedBySetupInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsPrecededByReceivedInvocation(syntaxNodeContext, enclosingExpression) == false &&
                IsUsedAlongWithArgInvokers(syntaxNodeContext, enclosingExpression) == false)
            {
                if (PotentialMisusedNodes.TryGetValue(enclosingExpression, out var nodes))
                {
                    nodes.Add(invocationExpression);
                }
                else
                {
                    PotentialMisusedNodes.Add(enclosingExpression, new List<SyntaxNode> { invocationExpression });
                }
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

        private SyntaxNode FindEnclosingExpression(TInvocationExpressionSyntax invocationExpression)
        {
            // finding usage of Arg like method in element access expressions and method invocation
            // deliberately skipping odd usages like var x = Arg.Any<int>() in order not to report false positives
            foreach (var possibleAncestorPath in PossibleAncestorPathsForArgument)
            {
                var node = invocationExpression.GetAncestorNode(possibleAncestorPath);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
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