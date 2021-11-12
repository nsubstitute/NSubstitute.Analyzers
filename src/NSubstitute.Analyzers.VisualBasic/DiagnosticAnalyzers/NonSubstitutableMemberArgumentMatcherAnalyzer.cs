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
        internal static ImmutableHashSet<int> MaybeAllowedAncestors { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.InvocationExpression,
            (int)SyntaxKind.AddHandlerStatement,
            (int)SyntaxKind.ObjectCreationExpression,
            (int)SyntaxKind.EqualsExpression,
            (int)SyntaxKind.SimpleAssignmentStatement);

        private static ImmutableHashSet<int> IgnoredAncestors { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.VariableDeclarator);

        public NonSubstitutableMemberArgumentMatcherAnalyzer()
            : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; } = MaybeAllowedAncestors;

        protected override ImmutableHashSet<int> IgnoredArgMatcherAncestors { get; } = IgnoredAncestors;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}