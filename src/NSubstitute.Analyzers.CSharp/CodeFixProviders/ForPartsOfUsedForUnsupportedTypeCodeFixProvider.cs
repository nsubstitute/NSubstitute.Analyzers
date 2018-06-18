using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal class ForPartsOfUsedForUnsupportedTypeCodeFixProvider : AbstractForPartsOfUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, GenericNameSyntax>
    {
        protected override GenericNameSyntax GetGenericNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return (GenericNameSyntax)memberAccess.Name;
        }

        protected override GenericNameSyntax GetUpdatedGenericNameSyntax(GenericNameSyntax nameSyntax, string identifierName)
        {
            return nameSyntax.WithIdentifier(IdentifierName(identifierName).Identifier);
        }
    }
}