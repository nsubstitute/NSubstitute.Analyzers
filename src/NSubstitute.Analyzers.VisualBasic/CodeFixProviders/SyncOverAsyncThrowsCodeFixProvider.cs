using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider<InvocationExpressionSyntax>
{
    protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) => ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Expression;
}