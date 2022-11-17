using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
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
            (SimpleNameSyntax)updatedNameSyntax));
    }
}