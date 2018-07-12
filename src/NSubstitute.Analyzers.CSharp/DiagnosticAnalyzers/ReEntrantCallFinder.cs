using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class ReEntrantCallFinder : AbstractReEntrantCallFinder
    {
        protected override ImmutableList<ISymbol> GetReEntrantSymbols(SemanticModel semanticModel, SyntaxNode rootNode)
        {
            var visitor = new ReEntrantCallVisitor(this, semanticModel);
            visitor.Visit(rootNode);
            return visitor.InvocationSymbols;
        }

        private class ReEntrantCallVisitor : CSharpSyntaxWalker
        {
            private readonly ReEntrantCallFinder _reEntrantCallFinder;
            private readonly SemanticModel _semanticModel;
            private readonly HashSet<SyntaxNode> _visitedNodes = new HashSet<SyntaxNode>();
            private readonly List<ISymbol> _invocationSymbols = new List<ISymbol>();

            public ImmutableList<ISymbol> InvocationSymbols => _invocationSymbols.ToImmutableList();

            public ReEntrantCallVisitor(ReEntrantCallFinder reEntrantCallFinder, SemanticModel semanticModel)
            {
                _reEntrantCallFinder = reEntrantCallFinder;
                _semanticModel = semanticModel;
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                var symbolInfo = _semanticModel.GetSymbolInfo(node);
                if (_reEntrantCallFinder.IsReturnsLikeMethod(_semanticModel, symbolInfo.Symbol))
                {
                    _invocationSymbols.Add(symbolInfo.Symbol);
                }

                base.VisitInvocationExpression(node);
            }

            public override void DefaultVisit(SyntaxNode node)
            {
                VisitRelatedSymbols(node);
                base.DefaultVisit(node);
            }

            private void VisitRelatedSymbols(SyntaxNode syntaxNode)
            {
                if (_visitedNodes.Contains(syntaxNode) == false &&
                    (syntaxNode.IsKind(SyntaxKind.IdentifierName) ||
                    syntaxNode.IsKind(SyntaxKind.ElementAccessExpression) ||
                    syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
                {
                    _visitedNodes.Add(syntaxNode);
                    foreach (var relatedNode in _reEntrantCallFinder.GetRelatedNodes(_semanticModel, syntaxNode))
                    {
                        Visit(relatedNode);
                    }
                }
            }
        }
    }
}