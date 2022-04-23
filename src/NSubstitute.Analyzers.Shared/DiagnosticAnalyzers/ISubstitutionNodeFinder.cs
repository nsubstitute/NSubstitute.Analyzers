using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal interface ISubstitutionNodeFinder
{
    IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpression, IMethodSymbol invocationExpressionSymbol);

    IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol invocationExpressionSymbol = null);

    IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol whenInvocationSymbol = null);

    IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol receivedInOrderInvocationSymbol = null);

    SyntaxNode FindForStandardExpression(IInvocationOperation invocationOperation);

    IOperation FindOperationForStandardExpression(IInvocationOperation invocationOperation);
}