using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantCallFinder
    {
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod);

        public abstract ImmutableList<ISymbol> GetReEntrantCalls(SemanticModel semanticModel, SyntaxNode rootNode);

        protected IEnumerable<SyntaxNode> GetRelatedNodes(SemanticModel semanticModel, SyntaxNode syntaxNode)
        {
            var symbol = semanticModel.GetSymbolInfo(syntaxNode);
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
            if (symbol == null || MethodNames.Contains(symbol.Name) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteExtensionsFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}