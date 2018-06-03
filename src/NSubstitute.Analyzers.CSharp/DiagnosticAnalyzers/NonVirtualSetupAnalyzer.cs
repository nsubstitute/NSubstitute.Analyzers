using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonVirtualSetupAnalyzer : AbstractNonVirtualSetupAnalyzer<SyntaxKind, MemberAccessExpressionSyntax, InvocationExpressionSyntax>
    {
        protected override SyntaxKind SimpleMemberAccessExpressionKind { get; } = SyntaxKind.SimpleMemberAccessExpression;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableHashSet<Type> KnownNonVirtualSyntaxTypes { get; } = ImmutableHashSet.Create(
            typeof(LiteralExpressionSyntax));

        protected override ImmutableHashSet<int> SupportedMemberAccesses { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.InvocationExpression,
            (int)SyntaxKind.SimpleMemberAccessExpression,
            (int)SyntaxKind.ElementAccessExpression,
            (int)SyntaxKind.NumericLiteralExpression,
            (int)SyntaxKind.CharacterLiteralExpression,
            (int)SyntaxKind.FalseLiteralExpression,
            (int)SyntaxKind.TrueLiteralExpression,
            (int)SyntaxKind.StringLiteralExpression);

        protected override SyntaxNode GetArgument(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.FirstOrDefault()?.DescendantNodes().FirstOrDefault();
        }

        protected override string GetAccessedMemberName(MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return memberAccessExpressionSyntax.Name.Identifier.Text;
        }
    }
}