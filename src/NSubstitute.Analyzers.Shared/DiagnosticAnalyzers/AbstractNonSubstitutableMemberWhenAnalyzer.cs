using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberWhenAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractNonSubstitutableSetupAnalyzer
    where TInvocationExpressionSyntax : SyntaxNode
    where TSyntaxKind : struct, Enum
{
    private readonly ISubstitutionNodeFinder<TInvocationExpressionSyntax> _substitutionNodeFinder;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected AbstractNonSubstitutableMemberWhenAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder<TInvocationExpressionSyntax> substitutionNodeFinder,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification,
            DiagnosticDescriptorsProvider.InternalSetupSpecification);
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualWhenSetupSpecification;
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol == null || methodSymbolInfo.Symbol.Kind != SymbolKind.Method)
        {
            return;
        }

        var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

        if (methodSymbol.IsWhenLikeMethod() == false)
        {
            return;
        }

        var expressionsForAnalysys = _substitutionNodeFinder.FindForWhenExpression(syntaxNodeContext, invocationExpression, methodSymbol);
        foreach (var analysedSyntax in expressionsForAnalysys)
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(analysedSyntax);
            if (symbolInfo.Symbol != null)
            {
                Analyze(syntaxNodeContext, analysedSyntax, symbolInfo.Symbol);
            }
        }
    }
}