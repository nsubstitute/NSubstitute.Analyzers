using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class NonSubstitutableMemberArgumentMatcherAnalyzer : AbstractNonSubstitutableMemberArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        internal static ImmutableArray<ImmutableArray<int>> AllowedPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.BracketedArgumentList,
                (int)SyntaxKind.ElementAccessExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.CastExpression,
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.AsExpression,
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.CastExpression,
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.BracketedArgumentList,
                (int)SyntaxKind.ElementAccessExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.AsExpression,
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.BracketedArgumentList,
                (int)SyntaxKind.ElementAccessExpression),
            ImmutableArray.Create((int)SyntaxKind.AddAssignmentExpression));

        private static ImmutableArray<ImmutableArray<int>> IgnoredPaths { get; } = ImmutableArray.Create(
                ImmutableArray.Create(
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator),
                ImmutableArray.Create(
                    (int)SyntaxKind.CastExpression,
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator),
                ImmutableArray.Create(
                    (int)SyntaxKind.AsExpression,
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator));

        public NonSubstitutableMemberArgumentMatcherAnalyzer()
            : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; } = AllowedPaths;

        protected override ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}