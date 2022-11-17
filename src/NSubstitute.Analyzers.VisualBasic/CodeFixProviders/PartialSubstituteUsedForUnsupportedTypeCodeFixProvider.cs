using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
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