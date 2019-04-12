using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal class ReEntrantCallFinder : AbstractReEntrantCallFinder
    {
        protected override ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SyntaxNode rootNode)
        {
            var visitor = new ReEntrantCallVisitor(this, compilation);
            visitor.Visit(rootNode);
            return visitor.InvocationSymbols;
        }

        private class ReEntrantCallVisitor : VisualBasicSyntaxWalker
        {
            private readonly ReEntrantCallFinder _reEntrantCallFinder;
            private readonly Compilation _compilation;
            private readonly HashSet<SyntaxNode> _visitedNodes = new HashSet<SyntaxNode>();
            private readonly List<ISymbol> _invocationSymbols = new List<ISymbol>();

            public ImmutableList<ISymbol> InvocationSymbols => _invocationSymbols.ToImmutableList();

            public ReEntrantCallVisitor(ReEntrantCallFinder reEntrantCallFinder, Compilation compilation)
            {
                _reEntrantCallFinder = reEntrantCallFinder;
                _compilation = compilation;
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (_compilation.ContainsSyntaxTree(node.SyntaxTree))
                {
                    var semanticModel = _compilation.GetSemanticModel(node.SyntaxTree);
                    var symbolInfo = semanticModel.GetSymbolInfo(node);
                    if (_reEntrantCallFinder.IsReturnsLikeMethod(semanticModel, symbolInfo.Symbol))
                    {
                        _invocationSymbols.Add(symbolInfo.Symbol);
                    }
                }

                base.VisitInvocationExpression(node);
            }

            public override void VisitClassBlock(ClassBlockSyntax node)
            {
            }

            public override void VisitStructureBlock(StructureBlockSyntax node)
            {
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
                    syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
                {
                    _visitedNodes.Add(syntaxNode);
                    foreach (var relatedNode in _reEntrantCallFinder.GetRelatedNodes(_compilation, syntaxNode))
                    {
                        var currentNode = relatedNode;

                        // hack getting related symbols for method identifier doesnt return method block but method statement syntax
                        if (syntaxNode.IsKind(SyntaxKind.IdentifierName) && currentNode is MethodStatementSyntax && _visitedNodes.Contains(relatedNode.Parent) == false)
                        {
                            currentNode = relatedNode.Parent;
                        }

                        Visit(currentNode);
                    }
                }
            }
        }
    }
}