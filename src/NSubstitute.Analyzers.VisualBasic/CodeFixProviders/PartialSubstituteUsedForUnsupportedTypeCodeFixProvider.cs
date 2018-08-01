using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, GenericNameSyntax, IdentifierNameSyntax, SimpleNameSyntax>
    {
        protected override TInnerNameSyntax GetNameSyntax<TInnerNameSyntax>(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return memberAccess.Name as TInnerNameSyntax;
        }

        protected override TInnerNameSyntax GetUpdatedNameSyntax<TInnerNameSyntax>(TInnerNameSyntax nameSyntax, string identifierName)
        {
            return (TInnerNameSyntax)nameSyntax.WithIdentifier(IdentifierName(identifierName).Identifier);
        }
    }
}