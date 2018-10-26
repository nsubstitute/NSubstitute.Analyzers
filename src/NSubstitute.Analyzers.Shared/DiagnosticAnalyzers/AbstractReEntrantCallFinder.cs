using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantCallFinder
    {
        private static readonly ImmutableDictionary<string, string> MethodNames = new Dictionary<string, string>()
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDoMethod] = MetadataNames.NSubstituteWhenCalledType
        }.ToImmutableDictionary();

        public ImmutableList<ISymbol> GetReEntrantCalls(Compilation compilation, SyntaxNode rootNode)
        {
            var semanticModel = compilation.GetSemanticModel(rootNode.SyntaxTree);
            var symbolInfo = semanticModel.GetSymbolInfo(rootNode);

            if (IsLocalSymbol(symbolInfo.Symbol) || semanticModel.GetTypeInfo(rootNode).IsCallInfoDelegate(semanticModel))
            {
                return ImmutableList<ISymbol>.Empty;
            }

            return GetReEntrantSymbols(compilation, rootNode);
        }

        protected abstract ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SyntaxNode rootNode);

        protected IEnumerable<SyntaxNode> GetRelatedNodes(Compilation compilation, SyntaxNode syntaxNode)
        {
            var symbol = compilation.GetSemanticModel(syntaxNode.SyntaxTree).GetSymbolInfo(syntaxNode);
            if (symbol.Symbol != null && symbol.Symbol.Locations.Any())
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

        protected bool IsReturnsLikeMethod(SemanticModel semanticModel, ISymbol symbol)
        {
            if (symbol == null || MethodNames.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   (symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true ||
                    (symbol.ContainingType?.ConstructedFrom.Name)?.Equals(containingType, StringComparison.OrdinalIgnoreCase) == true);
        }

        private bool IsLocalSymbol(ISymbol symbol)
        {
            return symbol != null && symbol.Kind == SymbolKind.Local;
        }
    }
}