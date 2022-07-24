using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class
    AbstractReEntrantCallFinder<TInvocationExpressionSyntax, TIdentifierExpressionSyntax> : IReEntrantCallFinder
    where TInvocationExpressionSyntax : SyntaxNode
    where TIdentifierExpressionSyntax : SyntaxNode
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    protected AbstractReEntrantCallFinder(ISubstitutionNodeFinder substitutionNodeFinder)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
    }

    public ImmutableList<IInvocationOperation> GetReEntrantCalls(Compilation compilation, IInvocationOperation invocationOperation, IOperation rootNode)
    {
        var rootNodeSymbol = rootNode.ExtractSymbol();

        if (rootNodeSymbol == null || rootNode.Kind == OperationKind.LocalReference)
        {
            return ImmutableList<IInvocationOperation>.Empty;
        }

        var reEntrantSymbols = GetReEntrantSymbols(compilation, invocationOperation, rootNode);
        var otherSubstitutions = GetOtherSubstitutionsForSymbol(compilation, rootNode, rootNodeSymbol);

        return reEntrantSymbols.AddRange(otherSubstitutions);
    }

    protected virtual IEnumerable<IInvocationOperation> GetPotentialOtherSubstituteInvocations(Compilation compilation, IEnumerable<IOperation> operations)
    {
        foreach (var operation in operations)
        {
            var visitor = new ReEntrantCallVisitor(compilation, operation);
            foreach (var visitorInvocationOperation in visitor.InvocationOperations)
            {
                yield return visitorInvocationOperation;
            }
        }
    }

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

    private IEnumerable<IInvocationOperation> GetOtherSubstitutionsForSymbol(Compilation compilation, IOperation rootOperation, ISymbol rootNodeSymbol)
    {
        if (rootNodeSymbol == null)
        {
            yield break;
        }

        var rootIdentifierNode = GetIdentifierOperation(rootOperation);

        var rootIdentifierSymbol = rootIdentifierNode?.ExtractSymbol();

        if (rootIdentifierSymbol == null)
        {
            yield break;
        }

        var ancestorChildNodes = rootOperation.Ancestors().SelectMany(ancestor => ancestor.Children);
        foreach (var operation in GetPotentialOtherSubstituteInvocations(compilation, ancestorChildNodes))
        {
            if (operation.TargetMethod.IsReturnLikeMethod() == false)
            {
                continue;
            }

            var substitutedNode = _substitutionNodeFinder.FindForStandardExpression(operation);

            var substituteNodeSymbol = substitutedNode?.ExtractSymbol();

            if (substituteNodeSymbol == null)
            {
               continue;
            }

            if (!rootNodeSymbol.Equals(substituteNodeSymbol))
            {
                continue;
            }

            // TODO from operation
            var substituteNodeIdentifier = GetIdentifierOperation(substitutedNode);

            if (rootIdentifierSymbol.Equals(substituteNodeIdentifier.ExtractSymbol()))
            {
                // TODO
                yield return substitutedNode as IInvocationOperation;
            }
        }
    }

    private ILocalReferenceOperation GetIdentifierOperation(IOperation node)
    {
        // TODO can it be done better?
        return node.Children.FirstOrDefault() as ILocalReferenceOperation;
    }

    private ImmutableList<IInvocationOperation> GetReEntrantSymbols(Compilation compilation, IInvocationOperation invocationOperation, IOperation rootNode)
    {
        var reentryVisitor = new ReEntrantCallVisitor(compilation, invocationOperation);
        reentryVisitor.Visit(rootNode);

        return reentryVisitor.InvocationOperations;
    }

    private class ReEntrantCallVisitor : OperationWalker
    {
        private readonly Compilation _compilation;
        private readonly HashSet<IOperation> _visitedOperations = new ();
        private readonly List<IInvocationOperation> _invocationOperation = new ();
        private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModelCache = new (1);

        public ImmutableList<IInvocationOperation> InvocationOperations => _invocationOperation.ToImmutableList();

        public ReEntrantCallVisitor(Compilation compilation, IOperation initialOperation)
        {
            _compilation = compilation;
            _visitedOperations.Add(initialOperation);
        }

        public override void VisitInvocation(IInvocationOperation operation)
        {
            if (_visitedOperations.Contains(operation) == false && operation.TargetMethod.IsInnerReEntryLikeMethod())
            {
               _invocationOperation.Add(operation);
            }

            VisitRelatedSymbols(operation);
            base.VisitInvocation(operation);
        }

        private void VisitRelatedSymbols(IInvocationOperation invocationOperation)
        {
            if (_visitedOperations.Contains(invocationOperation))
            {
                return;
            }

            _visitedOperations.Add(invocationOperation);

            foreach (var location in invocationOperation.TargetMethod.Locations.Where(location => location.IsInSource))
            {
                var root = location.SourceTree.GetRoot();
                var relatedNode = root.FindNode(location.SourceSpan);
                Visit(GetSemanticModel(relatedNode).GetOperation(relatedNode));
            }
        }

        private SemanticModel GetSemanticModel(SyntaxNode syntaxNode)
        {
            var syntaxTree = syntaxNode.SyntaxTree;
            if (_semanticModelCache.TryGetValue(syntaxTree, out var semanticModel))
            {
                return semanticModel;
            }

            semanticModel = _compilation.GetSemanticModel(syntaxTree);
            _semanticModelCache[syntaxTree] = semanticModel;

            return semanticModel;
        }
    }
}