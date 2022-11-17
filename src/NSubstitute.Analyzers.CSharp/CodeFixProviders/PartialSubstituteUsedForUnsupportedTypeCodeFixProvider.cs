using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider
{
    protected override SyntaxNode UpdateInvocationExpression(IInvocationOperation invocationOperation, string identifierName)
    {
        var invocationExpression = (InvocationExpressionSyntax)invocationOperation.Syntax;
        var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;
        return invocationExpression.WithExpression(
            memberAccessExpression.WithName(
                memberAccessExpression.Name.WithIdentifier(Identifier(identifierName))));
    }
}