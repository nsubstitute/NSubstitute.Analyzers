using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class NonVirtualSetupAnalyzer : AbstractNonVirtualSetupAnalyzer<SyntaxKind, MemberAccessExpressionSyntax, InvocationExpressionSyntax>
    {
        protected override SyntaxKind SimpleMemberAccessExpressionKind { get; } = SyntaxKind.SimpleMemberAccessExpression;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableHashSet<Type> KnownNonVirtualSyntaxTypes { get; } = ImmutableHashSet.Create(
                typeof(LiteralExpressionSyntax));

        protected override ImmutableHashSet<int> SupportedMemberAccesses { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.InvocationExpression,
            (int)SyntaxKind.SimpleMemberAccessExpression,
            (int)SyntaxKind.NumericLiteralExpression,
            (int)SyntaxKind.CharacterLiteralExpression,
            (int)SyntaxKind.FalseLiteralExpression,
            (int)SyntaxKind.TrueLiteralExpression,
            (int)SyntaxKind.StringLiteralExpression);

        public NonVirtualSetupAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

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