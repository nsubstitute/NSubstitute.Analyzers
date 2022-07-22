using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider<InvocationExpressionSyntax>
{
    protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) => ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).Expression;

    protected override SyntaxNode UpdateMemberExpression(InvocationExpressionSyntax invocationExpressionSyntax, SyntaxNode updatedNameSyntax)
    {
        var expressionSyntax = invocationExpressionSyntax.Expression;
        return invocationExpressionSyntax.WithExpression(MemberAccessExpression(
            expressionSyntax.Kind(),
            ((MemberAccessExpressionSyntax)expressionSyntax).Expression,
            Token(SyntaxKind.DotToken),
            (SimpleNameSyntax)updatedNameSyntax));
    }
}