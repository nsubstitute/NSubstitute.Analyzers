using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class AsyncReceivedInOrderCallbackAnalyzer : AbstractAsyncReceivedInOrderCallbackAnalyzer
{
    public AsyncReceivedInOrderCallbackAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }

    protected override SyntaxToken? GetAsyncToken(SyntaxNode node)
    {
        return node switch
        {
            LambdaExpressionSyntax lambdaExpressionSyntax => lambdaExpressionSyntax.SubOrFunctionHeader.ChildTokens()
                .Select<SyntaxToken, SyntaxToken?>(token => token)
                .FirstOrDefault(token => token.HasValue && token.Value.IsKind(SyntaxKind.AsyncKeyword)),
            _ => null
        };
    }
}