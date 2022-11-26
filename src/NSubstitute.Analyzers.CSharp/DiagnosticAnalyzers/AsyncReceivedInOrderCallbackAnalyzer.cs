using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AsyncReceivedInOrderCallbackAnalyzer : AbstractAsyncReceivedInOrderCallbackAnalyzer
{
    public AsyncReceivedInOrderCallbackAnalyzer()
        : base(CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }

    protected override SyntaxToken? GetAsyncToken(SyntaxNode node)
    {
        return node.ChildTokens().Select<SyntaxToken, SyntaxToken?>(token => token).FirstOrDefault(token =>
            token.HasValue && token.Value.IsKind(SyntaxKind.AsyncKeyword));
    }
}