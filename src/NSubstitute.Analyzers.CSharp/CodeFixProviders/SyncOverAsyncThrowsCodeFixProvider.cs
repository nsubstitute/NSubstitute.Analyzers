using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider<InvocationExpressionSyntax>
{
    protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) => ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Expression;
}