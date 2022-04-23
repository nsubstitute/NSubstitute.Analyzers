using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractConflictingArgumentAssignmentsAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
    where TSyntaxKind : struct
{
    private readonly ICallInfoFinder _callInfoFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected AbstractConflictingArgumentAssignmentsAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder callInfoFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _callInfoFinder = callInfoFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ConflictingArgumentAssignments);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation
                invocationOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsAndDoesLikeMethod() == false)
        {
            return;
        }

        if (!(invocationOperation.GetSubstituteOperation() is IInvocationOperation substituteOperation))
        {
            return;
        }

        var andDoesIndexers = FindCallInfoIndexers(syntaxNodeContext, invocationOperation).ToList();

        if (andDoesIndexers.Count == 0)
        {
            return;
        }

        var previousCallIndexers = FindCallInfoIndexers(syntaxNodeContext, substituteOperation);

        var immutableHashSet = previousCallIndexers
            .Select(indexerExpression => GetIndexerPosition(syntaxNodeContext, indexerExpression)).ToImmutableHashSet();

        foreach (var indexerExpressionSyntax in andDoesIndexers)
        {
            var position = GetIndexerPosition(syntaxNodeContext, indexerExpressionSyntax);
            if (position.HasValue && immutableHashSet.Contains(position.Value))
            {
                syntaxNodeContext.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ConflictingArgumentAssignments,
                    indexerExpressionSyntax.GetLocation()));
            }
        }
    }

    private int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode indexerExpression)
    {
        return syntaxNodeContext.SemanticModel.GetOperation(indexerExpression).GetIndexerPosition();
    }

    private bool IsAssigned(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode indexerExpressionSyntax)
    {
        return syntaxNodeAnalysisContext.SemanticModel.GetOperation(indexerExpressionSyntax) is
                   IPropertyReferenceOperation propertyReferenceOperation &&
               propertyReferenceOperation.Parent is ISimpleAssignmentOperation;
    }

    private IEnumerable<SyntaxNode> FindCallInfoIndexers(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation)
    {
        // perf - dont use linq in hotpaths
        foreach (var argumentOperation in invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument())
        {
            foreach (var indexerExpressionSyntax in _callInfoFinder
                         .GetCallInfoContext(syntaxNodeContext.SemanticModel, argumentOperation).IndexerAccesses)
            {
                if (IsAssigned(syntaxNodeContext, indexerExpressionSyntax))
                {
                    yield return indexerExpressionSyntax;
                }
            }
        }
    }
}