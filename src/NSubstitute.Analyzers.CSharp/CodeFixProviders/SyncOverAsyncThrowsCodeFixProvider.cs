using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider<InvocationExpressionSyntax>
{
    protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) => ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Expression;

    protected override SyntaxNode UpdateMemberExpression(InvocationExpressionSyntax invocationExpressionSyntax, SyntaxNode updatedNameSyntax)
    {
        var expressionSyntax = invocationExpressionSyntax.Expression;
        return invocationExpressionSyntax.WithExpression(MemberAccessExpression(
            expressionSyntax.Kind(),
            ((MemberAccessExpressionSyntax)expressionSyntax).Expression,
            (SimpleNameSyntax)updatedNameSyntax));
    }
}