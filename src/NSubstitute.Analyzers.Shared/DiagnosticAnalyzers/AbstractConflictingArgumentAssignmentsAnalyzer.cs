using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractConflictingArgumentAssignmentsAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly ICallInfoFinder _callInfoFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractConflictingArgumentAssignmentsAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder callInfoFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _callInfoFinder = callInfoFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ConflictingArgumentAssignments);
    }

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext syntaxNodeContext)
    {
        var invocationOperation = (IInvocationOperation)syntaxNodeContext.Operation;

        if (invocationOperation.TargetMethod.IsAndDoesLikeMethod() == false)
        {
            return;
        }

        if (invocationOperation.GetSubstituteOperation() is not IInvocationOperation substituteOperation)
        {
            return;
        }

        var andDoesIndexers = FindCallInfoIndexers(invocationOperation).ToList();

        if (andDoesIndexers.Count == 0)
        {
            return;
        }

        var previousCallIndexers = FindCallInfoIndexers(substituteOperation);

        var previousCallIndexerPositions = previousCallIndexers
            .Select(indexerPropertyReferenceOperation => indexerPropertyReferenceOperation.GetIndexerPosition())
            .ToImmutableHashSet();

        foreach (var indexerOperation in andDoesIndexers)
        {
            var position = indexerOperation.GetIndexerPosition();
            if (position.HasValue && previousCallIndexerPositions.Contains(position.Value))
            {
                syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ConflictingArgumentAssignments,
                    indexerOperation.Syntax.GetLocation()));
            }
        }
    }

    private IEnumerable<IOperation> FindCallInfoIndexers(IInvocationOperation invocationOperation)
    {
        // perf - dont use linq in hotpaths
        foreach (var argumentOperation in invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            foreach (var propertyReference in _callInfoFinder
                         .GetCallInfoContext(argumentOperation).IndexerAccessesOperations)
            {
                if (propertyReference is IPropertyReferenceOperation && propertyReference.Parent is ISimpleAssignmentOperation)
                {
                    yield return propertyReference;
                }
            }
        }
    }
}