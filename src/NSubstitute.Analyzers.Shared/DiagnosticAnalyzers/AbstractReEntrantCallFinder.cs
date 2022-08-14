using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReEntrantCallFinder : IReEntrantCallFinder
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    protected AbstractReEntrantCallFinder(ISubstitutionNodeFinder substitutionNodeFinder)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
    }

    public IReadOnlyList<IOperation> GetReEntrantCalls(
        Compilation compilation,
        IInvocationOperation invocationOperation,
        IOperation rootNode)
    {
        var rootNodeSymbol = rootNode.ExtractSymbol();

        if (rootNodeSymbol == null || rootNode.Kind == OperationKind.LocalReference)
        {
            return Array.Empty<IOperation>();
        }

        var reEntrantSymbols = GetReEntrantSymbols(compilation, invocationOperation, rootNode);
        var otherSubstitutions = GetOtherSubstitutionsForSymbol(compilation, rootNode, rootNodeSymbol);

        return reEntrantSymbols.Concat(otherSubstitutions).ToList().AsReadOnly();
    }

    protected virtual IEnumerable<IInvocationOperation> GetPotentialOtherSubstituteInvocations(Compilation compilation, IEnumerable<IOperation> operations)
    {
        foreach (var operation in operations)
        {
            var visitor = new ReEntrantCallVisitor(compilation, operation);
            visitor.Visit(operation);
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
        if (symbol.Symbol == null || symbol.Symbol.IsLocal() || symbol.Symbol.Locations.Length == 0)
        {
            yield break;
        }

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

    private IEnumerable<IOperation> GetOtherSubstitutionsForSymbol(Compilation compilation, IOperation rootOperation, ISymbol rootNodeSymbol)
    {
        if (rootNodeSymbol == null)
        {
            yield break;
        }

        var rootIdentifierNode = GetLocalReferenceOperation(rootOperation);

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

            var substituteNodeIdentifier = GetLocalReferenceOperation(substitutedNode);

            if (rootIdentifierSymbol.Equals(substituteNodeIdentifier.ExtractSymbol()))
            {
                yield return substitutedNode;
            }
        }
    }

    private IOperation GetLocalReferenceOperation(IOperation node)
    {
        // TODO can it be done better?
        var child = node.Children.FirstOrDefault();
        return child switch
        {
            ILocalReferenceOperation _ => child,
            IFieldReferenceOperation _ => child,
            _ => null
        };
    }

    private IEnumerable<IOperation> GetReEntrantSymbols(Compilation compilation, IInvocationOperation invocationOperation, IOperation rootNode)
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