using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal interface ISubstitutionNodeFinder<in TInvocationExpressionSyntax> where TInvocationExpressionSyntax : SyntaxNode
    {
        IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol invocationExpressionSymbol = null);

        IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol whenInvocationSymbol = null);

        IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax receivedInOrderExpression, IMethodSymbol receivedInOrderInvocationSymbol = null);

        SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol = null);

        SyntaxNode FindForStandardExpression(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol invocationExpressionSymbol = null);
    }
}