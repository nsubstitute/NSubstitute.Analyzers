using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstitutionNodeFinder<in TInvocationExpressionSyntax> where TInvocationExpressionSyntax : SyntaxNode
{
    IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol = null);

    IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax whenInvocationExpression, IMethodSymbol whenInvocationSymbol = null);

    IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax receivedInOrderExpression, IMethodSymbol receivedInOrderInvocationSymbol = null);

    SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol = null);

    SyntaxNode FindForStandardExpression(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol invocationExpressionSymbol = null);
}