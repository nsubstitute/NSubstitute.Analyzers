using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, GenericNameSyntax, IdentifierNameSyntax, SimpleNameSyntax>
    {
        protected override TInnerNameSyntax GetNameSyntax<TInnerNameSyntax>(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return (TInnerNameSyntax)memberAccess.Name;
        }

        protected override TInnerNameSyntax GetUpdatedNameSyntax<TInnerNameSyntax>(TInnerNameSyntax nameSyntax, string identifierName)
        {
            return (TInnerNameSyntax)nameSyntax.WithIdentifier(IdentifierName(identifierName).Identifier);
        }
    }
}