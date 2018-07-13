using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantCallFinder
    {
        private static readonly ImmutableDictionary<string, string> MethodNames = new Dictionary<string, string>()
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDoMethod] = "NSubstitute.Core.WhenCalled<T>"
        }.ToImmutableDictionary();

        public ImmutableList<ISymbol> GetReEntrantCalls(SemanticModel semanticModel, SyntaxNode rootNode)
        {
            var typeInfo = semanticModel.GetTypeInfo(rootNode);
            if (IsCalledViaDelegate(semanticModel, typeInfo))
            {
                return ImmutableList<ISymbol>.Empty;
            }

            return GetReEntrantSymbols(semanticModel, rootNode);
        }

        protected abstract ImmutableList<ISymbol> GetReEntrantSymbols(SemanticModel semanticModel, SyntaxNode rootNode);

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
            if (symbol == null || MethodNames.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ConstructedFrom.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        private static bool IsCalledViaDelegate(SemanticModel semanticModel, TypeInfo typeInfo)
        {
            var typeSymbol = typeInfo.Type ?? typeInfo.ConvertedType;
            var isCalledViaDelegate = typeSymbol != null &&
                                      typeSymbol.TypeKind == TypeKind.Delegate &&
                                      typeSymbol is INamedTypeSymbol namedTypeSymbol &&
                                      namedTypeSymbol.ConstructedFrom.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Func`2")) &&
                                      IsCallInfoParameter(namedTypeSymbol.TypeArguments.First());

            return isCalledViaDelegate;
        }

        private static bool IsCallInfoParameter(ITypeSymbol symbol)
        {
            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}