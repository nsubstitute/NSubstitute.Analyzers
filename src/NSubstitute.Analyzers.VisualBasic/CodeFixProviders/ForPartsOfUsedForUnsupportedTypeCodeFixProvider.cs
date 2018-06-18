using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
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