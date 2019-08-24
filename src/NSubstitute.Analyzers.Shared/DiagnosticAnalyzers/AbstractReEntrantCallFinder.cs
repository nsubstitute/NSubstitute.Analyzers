using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantCallFinder : IReEntrantCallFinder
    {
        public ImmutableList<ISymbol> GetReEntrantCalls(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(rootNode);

            if (IsLocalSymbol(symbolInfo.Symbol) || semanticModel.GetTypeInfo(rootNode).IsCallInfoDelegate(semanticModel))
            {
                return ImmutableList<ISymbol>.Empty;
            }

            return GetReEntrantSymbols(compilation, semanticModel, originatingExpression, rootNode);
        }

        protected abstract ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode);

        protected IEnumerable<SyntaxNode> GetRelatedNodes(Compilation compilation, SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            if (compilation.ContainsSyntaxTree(syntaxNode.SyntaxTree) == false)
            {
                yield break;
            }

            var symbol = GetSemanticModel(compilation, semanticModel, syntaxNode).GetSymbolInfo(syntaxNode);
            if (symbol.Symbol != null && IsLocalSymbol(symbol.Symbol) == false && symbol.Symbol.Locations.Any())
            {
                foreach (var symbolLocation in symbol.Symbol.Locations.Where(location => location.SourceTree != null))
                {
                    var root = symbolLocation.SourceTree.GetRoot();
                    var relatedNode = root.FindNode(symbolLocation.SourceSpan);
                    if (relatedNode != null)
                    {
                        yield return relatedNode;
                    }
                }
            }
        }

        protected SemanticModel GetSemanticModel(Compilation compilation, SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            // perf - take original semantic model whenever possible
            if (semanticModel.SyntaxTree == syntaxNode.SyntaxTree)
            {
                return semanticModel;
            }

            // but keep in mind that we might traverse outside of the original one https://github.com/nsubstitute/NSubstitute.Analyzers/issues/56
            return compilation.GetSemanticModel(syntaxNode.SyntaxTree);
        }

        protected bool IsInnerReEntryLikeMethod(SemanticModel semanticModel, ISymbol symbol)
        {
            return symbol.IsInnerReEntryLikeMethod();
        }

        private bool IsLocalSymbol(ISymbol symbol)
        {
            return symbol != null && symbol.Kind == SymbolKind.Local;
        }
    }
}