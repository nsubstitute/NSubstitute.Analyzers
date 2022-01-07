using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractAsyncReceivedInOrderCallbackAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
    where TSyntaxKind : struct
    where TInvocationExpressionSyntax : SyntaxNode
{
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected AbstractAsyncReceivedInOrderCallbackAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(diagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract int AsyncExpressionRawKind { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected abstract IEnumerable<SyntaxNode> GetArgumentExpressions(TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract IEnumerable<SyntaxToken?> GetCallbackArgumentSyntaxTokens(SyntaxNode node);

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
        {
            return;
        }

        if (methodSymbolInfo.Symbol.IsReceivedInOrderMethod() == false)
        {
            return;
        }

        foreach (var expression in GetArgumentExpressions(invocationExpression))
        {
            var asyncToken = GetCallbackArgumentSyntaxTokens(expression)
                .FirstOrDefault(token => token.HasValue && token.Value.RawKind == AsyncExpressionRawKind);

            if (asyncToken.HasValue == false)
            {
                continue;
            }

            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.AsyncCallbackUsedInReceivedInOrder,
                asyncToken.Value.GetLocation());

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }
    }
}