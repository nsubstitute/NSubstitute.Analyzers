using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class NonSubstitutableMemberArgumentMatcherAnalyzer : AbstractNonSubstitutableMemberArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        internal static ImmutableArray<ImmutableArray<int>> AllowedPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.TryCastExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.DirectCastExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.CTypeExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create((int)SyntaxKind.AddHandlerStatement));

        private static ImmutableArray<ImmutableArray<int>> IgnoredPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.TryCastExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.DirectCastExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.CTypeExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator));

        public NonSubstitutableMemberArgumentMatcherAnalyzer()
            : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; } = AllowedPaths;

        protected override ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}