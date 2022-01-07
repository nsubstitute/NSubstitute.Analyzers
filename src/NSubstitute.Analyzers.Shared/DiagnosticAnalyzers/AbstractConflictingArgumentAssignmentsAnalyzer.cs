using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractConflictingArgumentAssignmentsAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TIndexerExpressionSyntax> : AbstractDiagnosticAnalyzer
    where TInvocationExpressionSyntax : SyntaxNode
    where TExpressionSyntax : SyntaxNode
    where TIndexerExpressionSyntax : SyntaxNode
    where TSyntaxKind : struct
{
    private readonly ICallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> _callInfoFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected AbstractConflictingArgumentAssignmentsAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ICallInfoFinder<TInvocationExpressionSyntax, TIndexerExpressionSyntax> callInfoFinder)
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

    protected abstract IEnumerable<TExpressionSyntax> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract int? GetIndexerPosition(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract ISymbol GetIndexerSymbol(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax);

    protected abstract SyntaxNode GetAssignmentExpression(TIndexerExpressionSyntax indexerExpressionSyntax);

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
        {
            return;
        }

        var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

        if (methodSymbol.IsAndDoesLikeMethod() == false)
        {
            return;
        }

        var previousCall = GetSubstituteCall(syntaxNodeContext, methodSymbol, invocationExpression) as TInvocationExpressionSyntax;

        if (previousCall == null)
        {
            return;
        }

        var andDoesIndexers = FindCallInfoIndexers(syntaxNodeContext, invocationExpression).ToList();

        if (andDoesIndexers.Count == 0)
        {
            return;
        }

        var previousCallIndexers = FindCallInfoIndexers(syntaxNodeContext, previousCall);

        var immutableHashSet = previousCallIndexers.Select(indexerExpression => GetIndexerPosition(syntaxNodeContext, indexerExpression))
            .ToImmutableHashSet();

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

    private bool IsAssigned(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TIndexerExpressionSyntax indexerExpressionSyntax)
    {
        return GetAssignmentExpression(indexerExpressionSyntax) != null &&
               GetIndexerSymbol(syntaxNodeAnalysisContext, indexerExpressionSyntax) is IPropertySymbol propertySymbol &&
               propertySymbol.ContainingType.IsCallInfoSymbol();
    }

    private IEnumerable<TIndexerExpressionSyntax> FindCallInfoIndexers(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpressionSyntax)
    {
        // perf - dont use linq in hotpaths
        foreach (var argumentExpression in GetArgumentExpressions(invocationExpressionSyntax))
        {
            foreach (var indexerExpressionSyntax in _callInfoFinder.GetCallInfoContext(syntaxNodeContext.SemanticModel, argumentExpression).IndexerAccesses)
            {
                if (IsAssigned(syntaxNodeContext, indexerExpressionSyntax))
                {
                    yield return indexerExpressionSyntax;
                }
            }
        }
    }
}