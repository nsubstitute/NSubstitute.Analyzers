using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class ReEntrantCallFinder : IReEntrantCallFinder
{
    private readonly ISubstitutionOperationFinder _substitutionOperationFinder;

    public static ReEntrantCallFinder Instance { get; } = new(SubstitutionOperationFinder.Instance);

    protected ReEntrantCallFinder(ISubstitutionOperationFinder substitutionOperationFinder)
    {
        _substitutionOperationFinder = substitutionOperationFinder;
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

    private IEnumerable<IOperation> GetOtherSubstitutionsForSymbol(Compilation compilation, IOperation rootOperation, ISymbol? rootNodeSymbol)
    {
        if (rootNodeSymbol == null)
        {
            yield break;
        }

        var rootIdentifierNode = GetLocalReferenceOperation(rootOperation);

        var rootIdentifierSymbol = rootIdentifierNode.ExtractSymbol();

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

            var substitutedNode = _substitutionOperationFinder.FindForStandardExpression(operation);

            if (substitutedNode == null)
            {
               yield break;
            }

            var substituteNodeSymbol = substitutedNode.ExtractSymbol();

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
        SemanticModel? semanticModel = null;
        foreach (var constructorLocation in fieldReferenceOperation.ContainingType.Constructors
                     .Where(methodSymbol => methodSymbol.Locations.Length > 0)
                     .SelectMany(x => x.Locations)
                     .Where(location => location.IsInSource))
        {
            var root = constructorLocation.SourceTree.GetRoot();
            var relatedNode = root.FindNode(constructorLocation.SourceSpan);

            // perf - take original semantic model whenever possible
            // but keep in mind that we might traverse outside of the original one https://github.com/nsubstitute/NSubstitute.Analyzers/issues/56
            semanticModel = semanticModel == null || semanticModel.SyntaxTree != constructorLocation.SourceTree
                ? compilation.TryGetSemanticModel(constructorLocation.SourceTree)
                : semanticModel;

            var operation = semanticModel?.GetOperation(relatedNode) ?? semanticModel?.GetOperation(relatedNode.Parent);

            if (operation is not null)
            {
                yield return operation;
            }
        }
    }

    private IOperation? GetLocalReferenceOperation(IOperation? node)
    {
        var child = node?.Children.FirstOrDefault();
        return child is ILocalReferenceOperation or IFieldReferenceOperation ? child : null;
    }

    private IEnumerable<IOperation> GetReEntrantSymbols(Compilation compilation, IInvocationOperation invocationOperation, IOperation rootNode)
    {
        var reentryVisitor = new ReEntrantCallVisitor(compilation, invocationOperation);
        reentryVisitor.Visit(rootNode);

        return reentryVisitor.InvocationOperations;
    }

    private sealed class ReEntrantCallVisitor : OperationWalker
    {
        private readonly Compilation _compilation;
        private readonly HashSet<IOperation> _visitedOperations = new();
        private readonly List<IInvocationOperation> _invocationOperation = new();
        private SemanticModel? _semanticModel;

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

                if (semanticModel == null)
                {
                   continue;
                }

                var operation = semanticModel.GetOperation(relatedNode) ??
                                semanticModel.GetOperation(relatedNode.Parent);
                Visit(operation);
            }
        }

        private SemanticModel? GetSemanticModel(SyntaxNode syntaxNode)
        {
            var syntaxTree = syntaxNode.SyntaxTree;

            // perf - take original semantic model whenever possible
            // but keep in mind that we might traverse outside of the original one https://github.com/nsubstitute/NSubstitute.Analyzers/issues/56
            if (_semanticModel == null || _semanticModel.SyntaxTree != syntaxTree)
            {
                _semanticModel = _compilation.TryGetSemanticModel(syntaxTree);
            }

            return _semanticModel;
        }
    }
}