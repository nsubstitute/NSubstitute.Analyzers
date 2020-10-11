using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
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