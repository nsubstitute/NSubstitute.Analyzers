using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSyncOverAsyncThrowsAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
    where TInvocationExpressionSyntax : SyntaxNode
    where TSyntaxKind : struct
{
    private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected AbstractSyncOverAsyncThrowsAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.SyncOverAsyncThrows);

        _analyzeInvocationAction = AnalyzeInvocation;
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = syntaxNodeContext.Node;
        if (!(syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol methodSymbol))
        {
            return;
        }

        if (!methodSymbol.IsThrowLikeMethod())
        {
            return;
        }

        if (!(syntaxNodeContext.SemanticModel.GetOperation(invocationExpression) is IInvocationOperation invocationOperation))
        {
            return;
        }

        var substitutedExpression = _substitutionNodeFinder.FindForStandardExpression(
            invocationOperation);

        if (substitutedExpression == null)
        {
            return;
        }

        var semanticModel = syntaxNodeContext.SemanticModel.GetSymbolInfo(substitutedExpression);

        ITypeSymbol returnType;
        switch (semanticModel.Symbol)
        {
            case IMethodSymbol method:
                returnType = method.ReturnType;
                break;
            case IPropertySymbol property:
                returnType = property.Type;
                break;
            default:
                returnType = null;
                break;
        }

        if (!IsTask(returnType, syntaxNodeContext.Compilation))
        {
            return;
        }

        syntaxNodeContext.ReportDiagnostic(
            Diagnostic.Create(DiagnosticDescriptorsProvider.SyncOverAsyncThrows, invocationExpression.GetLocation()));
    }

    private static bool IsTask(ITypeSymbol returnType, Compilation compilation)
    {
        if (returnType == null)
        {
            return false;
        }

        var taskSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");

        if (taskSymbol == null)
        {
            return false;
        }

        return returnType.Equals(taskSymbol) ||
               (returnType.BaseType != null && returnType.BaseType.Equals(taskSymbol));
    }
}