using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, NameSyntax>
    {
        protected override NameSyntax GetNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return memberAccess.Name;
        }
    }
}