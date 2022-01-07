using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReEntrantCallFinder<TInvocationExpressionSyntax, TIdentifierExpressionSyntax> : IReEntrantCallFinder
    where TInvocationExpressionSyntax : SyntaxNode
    where TIdentifierExpressionSyntax : SyntaxNode
{
    private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

    protected AbstractReEntrantCallFinder(ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
    }

    public ImmutableList<ISymbol> GetReEntrantCalls(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(rootNode);

        if (symbolInfo.Symbol.IsLocal() || semanticModel.GetTypeInfo(rootNode).IsCallInfoDelegate(semanticModel))
        {
            return ImmutableList<ISymbol>.Empty;
        }

        var reEntrantSymbols = GetReEntrantSymbols(compilation, semanticModel, originatingExpression, rootNode);
        var otherSubstitutions = GetOtherSubstitutionsForSymbol(semanticModel, rootNode, symbolInfo.Symbol);

        return reEntrantSymbols.AddRange(otherSubstitutions);
    }

    protected abstract ImmutableList<ISymbol> GetReEntrantSymbols(Compilation compilation, SemanticModel semanticModel, SyntaxNode originatingExpression, SyntaxNode rootNode);

    protected abstract IEnumerable<TInvocationExpressionSyntax> GetPotentialOtherSubstituteInvocations(IEnumerable<SyntaxNode> nodes);

    protected IEnumerable<SyntaxNode> GetRelatedNodes(Compilation compilation, SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (compilation.ContainsSyntaxTree(syntaxNode.SyntaxTree) == false)
        {
            yield break;
        }

        var symbol = GetSemanticModel(compilation, semanticModel, syntaxNode).GetSymbolInfo(syntaxNode);
        if (symbol.Symbol != null && symbol.Symbol.IsLocal() == false && symbol.Symbol.Locations.Any())
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

    private IEnumerable<ISymbol> GetOtherSubstitutionsForSymbol(SemanticModel semanticModel, SyntaxNode rootNode, ISymbol rootNodeSymbol)
    {
        if (rootNodeSymbol == null)
        {
            yield break;
        }

        var rootIdentifierNode = GetIdentifierExpressionSyntax(rootNode);
        if (rootIdentifierNode == null)
        {
            yield break;
        }

        var rootIdentifierSymbol = semanticModel.GetSymbolInfo(rootIdentifierNode);

        if (rootIdentifierSymbol.Symbol == null)
        {
            yield break;
        }

        var ancestorChildNodes = rootNode.Ancestors().SelectMany(ancestor => ancestor.ChildNodes());
        foreach (var syntaxNode in GetPotentialOtherSubstituteInvocations(ancestorChildNodes))
        {
            var symbol = semanticModel.GetSymbolInfo(syntaxNode).Symbol;
            if (symbol.IsReturnLikeMethod() == false)
            {
                continue;
            }

            var substitutedNode = _substitutionNodeFinder.FindForStandardExpression(syntaxNode, symbol as IMethodSymbol);

            var substituteNodeSymbol = semanticModel.GetSymbolInfo(substitutedNode).Symbol;
            if (substituteNodeSymbol != rootNodeSymbol)
            {
                continue;
            }

            var substituteNodeIdentifier = GetIdentifierExpressionSyntax(substitutedNode);

            var substituteIdentifierSymbol = semanticModel.GetSymbolInfo(substituteNodeIdentifier);
            if (rootIdentifierSymbol.Symbol == substituteIdentifierSymbol.Symbol)
            {
                yield return substituteNodeSymbol;
            }
        }
    }

    private TIdentifierExpressionSyntax GetIdentifierExpressionSyntax(SyntaxNode node)
    {
        return node.ChildNodes().FirstOrDefault() as TIdentifierExpressionSyntax;
    }
}