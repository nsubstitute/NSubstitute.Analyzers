using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class PartialSubstituteUsedForUnsupportedTypeCodeFixProvider : AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<InvocationExpressionSyntax, NameSyntax>
    {
        protected override NameSyntax GetNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return memberAccess.Name;
        }
    }
}