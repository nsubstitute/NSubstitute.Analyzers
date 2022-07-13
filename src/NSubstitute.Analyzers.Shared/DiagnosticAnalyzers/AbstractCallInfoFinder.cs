using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractCallInfoFinder : ICallInfoFinder
{
    public CallInfoContext GetCallInfoContext(IArgumentOperation argumentOperation)
    {
        var indexVisitor = new IndexerVisitor();
        indexVisitor.Visit(argumentOperation);

        return new CallInfoContext(
            indexVisitor.ArgAtInvocations,
            indexVisitor.ArgInvocations,
            indexVisitor.DirectIndexerAccesses);
    }

    private static CallInfoContext CreateFilteredCallInfoContext(
        SemanticModel semanticModel,
        CallInfoContext callContext,
        IParameterSymbol callInfoParameterSymbol)
    {
        return new CallInfoContext(
            argAtInvocations: GetMatchingNodes(semanticModel, callContext.ArgAtInvocations, callInfoParameterSymbol),
            argInvocations: GetMatchingNodes(semanticModel, callContext.ArgInvocations, callInfoParameterSymbol),
            indexerAccesses: GetMatchingNodes(semanticModel, callContext.IndexerAccesses, callInfoParameterSymbol));
    }

    private static IReadOnlyList<T> GetMatchingNodes<T>(
        SemanticModel semanticModel,
        IReadOnlyList<T> nodes,
        IParameterSymbol parameterSymbol) where T : SyntaxNode
    {
        return nodes.Where(node => HasMatchingParameterReference(semanticModel, node, parameterSymbol)).ToList();
    }

    private static bool HasMatchingParameterReference(
        SemanticModel semanticModel,
        SyntaxNode syntaxNode,
        IParameterSymbol callInfoParameterSymbol)
    {
        var parameterReferenceOperation = FindMatchingParameterReference(semanticModel, syntaxNode);

        return parameterReferenceOperation != null &&
               parameterReferenceOperation.Parameter.Equals(callInfoParameterSymbol);
    }

    private static IParameterReferenceOperation FindMatchingParameterReference(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        var operation = semanticModel.GetOperation(syntaxNode);
        return FindMatchingParameterReference(operation);
    }

    private static IParameterReferenceOperation FindMatchingParameterReference(IOperation operation)
    {
        IParameterReferenceOperation parameterReferenceOperation = null;
        switch (operation)
        {
            case IInvocationOperation invocationOperation:
                parameterReferenceOperation = invocationOperation.Instance as IParameterReferenceOperation;
                break;
            case IPropertyReferenceOperation propertyReferenceOperation:
                parameterReferenceOperation = propertyReferenceOperation.Instance as IParameterReferenceOperation;
                break;
        }

        if (parameterReferenceOperation != null)
        {
            return parameterReferenceOperation;
        }

        foreach (var innerOperation in operation?.Children ?? Enumerable.Empty<IOperation>())
        {
            parameterReferenceOperation = FindMatchingParameterReference(innerOperation);
            if (parameterReferenceOperation != null)
            {
                return parameterReferenceOperation;
            }
        }

        return null;
    }

    private static IParameterSymbol GetCallInfoParameterSymbol(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        if (semanticModel.GetSymbolInfo(syntaxNode).Symbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind != MethodKind.Constructor)
        {
            return methodSymbol.Parameters.FirstOrDefault();
        }

        return null;
    }

    private class IndexerVisitor : OperationWalker
    {
        public List<IInvocationOperation> ArgAtInvocations { get; } = new ();

        public List<IInvocationOperation> ArgInvocations { get; } = new ();

        public List<IPropertyReferenceOperation> DirectIndexerAccesses { get; } = new ();

        public override void VisitInvocation(IInvocationOperation operation)
        {
            if (operation.TargetMethod.ContainingType.IsCallInfoSymbol())
            {
                switch (operation.TargetMethod.Name)
                {
                    case MetadataNames.CallInfoArgAtMethod:
                        ArgAtInvocations.Add(operation);
                        break;
                    case MetadataNames.CallInfoArgMethod:
                        ArgInvocations.Add(operation);
                        break;
                }
            }

            base.VisitInvocation(operation);
        }

        public override void VisitPropertyReference(IPropertyReferenceOperation operation)
        {
            if (operation.Property.ContainingType.IsCallInfoSymbol() && operation.Property.Parameters.Length > 0)
            {
               DirectIndexerAccesses.Add(operation);
            }

            base.VisitPropertyReference(operation);
        }
    }
}