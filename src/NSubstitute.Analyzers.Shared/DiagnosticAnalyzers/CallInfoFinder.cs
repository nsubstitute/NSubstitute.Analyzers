using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal sealed class CallInfoFinder : ICallInfoFinder
{
    public static CallInfoFinder Instance { get; } = new();

    public CallInfoContext GetCallInfoContext(IArgumentOperation argumentOperation)
    {
        var callInfoContext = CallInfoContext.Empty;
        foreach (var operation in GetCallInfoOperations(argumentOperation))
        {
            var callInfoParameterSymbol = GetCallInfoParameterSymbol(operation);

            if (callInfoParameterSymbol == null)
            {
               continue;
            }

            var indexVisitor = new CallInfoVisitor();
            indexVisitor.Visit(argumentOperation);

            var currentContext = new CallInfoContext(
                indexVisitor.ArgAtInvocations,
                indexVisitor.ArgInvocations,
                indexVisitor.DirectIndexerAccesses);

            callInfoContext =
                callInfoContext.Merge(CreateFilteredCallInfoContext(currentContext, callInfoParameterSymbol));
        }

        return callInfoContext;
    }

    private static CallInfoContext CreateFilteredCallInfoContext(
        CallInfoContext callContext,
        IParameterSymbol callInfoParameterSymbol)
    {
        return new CallInfoContext(
            argAtInvocations: GetMatchingNodes(callContext.ArgAtInvocationsOperations, callInfoParameterSymbol),
            argInvocations: GetMatchingNodes(callContext.ArgInvocationsOperations, callInfoParameterSymbol),
            indexerAccesses: GetMatchingNodes(callContext.IndexerAccessesOperations, callInfoParameterSymbol));
    }

    private static IReadOnlyList<T> GetMatchingNodes<T>(
        IReadOnlyList<T> nodes,
        IParameterSymbol parameterSymbol) where T : IOperation
    {
        return nodes.Where(node => HasMatchingParameterReference(node, parameterSymbol)).ToList();
    }

    private static bool HasMatchingParameterReference(
        IOperation operation,
        IParameterSymbol callInfoParameterSymbol)
    {
        var parameterReferenceOperation = FindMatchingParameterReference(operation);

        return parameterReferenceOperation != null &&
               parameterReferenceOperation.Parameter.Equals(callInfoParameterSymbol);
    }

    private static IParameterReferenceOperation FindMatchingParameterReference(IOperation operation)
    {
        var parameterReferenceOperation = operation switch
        {
            IInvocationOperation invocationOperation => invocationOperation.Instance as IParameterReferenceOperation,
            IPropertyReferenceOperation propertyReferenceOperation =>
                propertyReferenceOperation.Instance as IParameterReferenceOperation,
            _ => null
        };

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

    private static IEnumerable<IOperation> GetCallInfoOperations(IArgumentOperation argumentOperation)
    {
        if (!argumentOperation.Parameter.IsParams)
        {
            yield return argumentOperation.Value;
            yield break;
        }

        var initializerElementValues = argumentOperation.Value.GetArrayElementValues();

        foreach (var operation in initializerElementValues ?? Enumerable.Empty<IOperation>())
        {
            yield return operation;
        }
    }

    private static IParameterSymbol GetCallInfoParameterSymbol(IOperation operation)
    {
        return operation switch
        {
            IInvocationOperation invocationOperation => invocationOperation.TargetMethod.Parameters.FirstOrDefault(),
            IDelegateCreationOperation delegateCreationOperation => GetCallInfoParameterSymbol(delegateCreationOperation.Target),
            IAnonymousFunctionOperation anonymousFunctionOperation => anonymousFunctionOperation.Symbol.Parameters.FirstOrDefault(),
            _ => null
        };
    }

    private sealed class CallInfoVisitor : OperationWalker
    {
        public List<IInvocationOperation> ArgAtInvocations { get; } = new();

        public List<IInvocationOperation> ArgInvocations { get; } = new();

        public List<IOperation> DirectIndexerAccesses { get; } = new();

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

        public override void VisitArrayElementReference(IArrayElementReferenceOperation operation)
        {
            ISymbol arrayReferenceSymbol = operation.ArrayReference switch
            {
                IInvocationOperation invocationOperation => invocationOperation.TargetMethod,
                IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Property,
                _ => null
            };

            if (arrayReferenceSymbol != null && arrayReferenceSymbol.ContainingType.IsCallInfoSymbol())
            {
               DirectIndexerAccesses.Add(operation);
            }

            base.VisitArrayElementReference(operation);
        }
    }
}