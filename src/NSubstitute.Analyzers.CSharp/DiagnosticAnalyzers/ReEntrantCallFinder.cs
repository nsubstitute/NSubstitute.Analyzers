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
        protected override ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SyntaxNode rootNode)
        {
            var visitor = new ReEntrantCallVisitor(this, compilation);
            visitor.Visit(rootNode);
            return visitor.InvocationSymbols;
        }

        private class ReEntrantCallVisitor : CSharpSyntaxWalker
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

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
            }

            public override void VisitStructDeclaration(StructDeclarationSyntax node)
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
                    syntaxNode.IsKind(SyntaxKind.ElementAccessExpression) ||
                    syntaxNode.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
                {
                    _visitedNodes.Add(syntaxNode);
                    foreach (var relatedNode in _reEntrantCallFinder.GetRelatedNodes(_compilation, syntaxNode))
                    {
                        Visit(relatedNode);
                    }
                }
            }
        }
    }
}