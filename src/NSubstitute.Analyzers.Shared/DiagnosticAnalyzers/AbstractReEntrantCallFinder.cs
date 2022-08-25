using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    private IEnumerable<IInvocationOperation> GetPotentialOtherSubstituteInvocations(Compilation compilation, IEnumerable<IOperation> operations)
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

        // TODO make it nicer
        var constructorOperations = GetConstructorOperations(compilation, rootIdentifierSymbol);
        var ancestorOperations = rootOperation.Ancestors().SelectMany(ancestor => ancestor.Children).Concat(constructorOperations);
        foreach (var operation in GetPotentialOtherSubstituteInvocations(compilation, ancestorOperations))
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

    private static IEnumerable<IOperation> GetConstructorOperations(Compilation compilation, ISymbol fieldReferenceOperation)
    {
        // TODO naming
        foreach (var location in fieldReferenceOperation.ContainingType.GetMembers().OfType<IMethodSymbol>()
                     .Where(methodSymbol => methodSymbol.MethodKind is MethodKind.Constructor or MethodKind.StaticConstructor && methodSymbol.Locations.Length > 0)
                     .SelectMany(x => x.Locations)
                     .Where(location => location.IsInSource))
        {
            var root = location.SourceTree.GetRoot();
            var relatedNode = root.FindNode(location.SourceSpan);

            // TODO reuse semantic model
            var semanticModel = compilation.GetSemanticModel(location.SourceTree);
            var operation = semanticModel.GetOperation(relatedNode) ?? semanticModel.GetOperation(relatedNode.Parent);

            if (operation is not null)
            {
                yield return operation;
            }
        }
    }

    private IOperation GetLocalReferenceOperation(IOperation node)
    {
        var child = node.Children.FirstOrDefault();
        return child is ILocalReferenceOperation or IFieldReferenceOperation ? child : null;
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
                var semanticModel = GetSemanticModel(relatedNode);
                var operation = semanticModel.GetOperation(relatedNode) ??
                                semanticModel.GetOperation(relatedNode.Parent);
                Visit(operation);
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