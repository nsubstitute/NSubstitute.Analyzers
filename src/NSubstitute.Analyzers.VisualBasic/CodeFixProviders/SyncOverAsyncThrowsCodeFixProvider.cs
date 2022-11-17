using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider
{
    public SyncOverAsyncThrowsCodeFixProvider()
        : base(SubstitutionNodeFinder.Instance)
    {
    }

    protected override SyntaxNode UpdateMemberExpression(IInvocationOperation invocationOperation, SyntaxNode updatedNameSyntax)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)invocationOperation.Syntax;
        var expressionSyntax = invocationExpressionSyntax.Expression;
        return invocationExpressionSyntax.WithExpression(MemberAccessExpression(
            expressionSyntax.Kind(),
            ((MemberAccessExpressionSyntax)expressionSyntax).Expression,
            Token(SyntaxKind.DotToken),
            (SimpleNameSyntax)updatedNameSyntax));
    }
}