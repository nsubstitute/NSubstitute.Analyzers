using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, SimpleNameSyntax>
    {
        protected override SimpleNameSyntax GetNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return memberAccess.Name;
        }

        protected override SimpleNameSyntax GetUpdatedNameSyntax(SimpleNameSyntax nameSyntax, string identifierName)
        {
            return nameSyntax.WithIdentifier(IdentifierName(identifierName).Identifier);
        }
    }
}