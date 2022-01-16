using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberArgumentMatcherAnalyzer : AbstractNonSubstitutableMemberArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    internal static ImmutableHashSet<int> MaybeAllowedAncestors { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.InvocationExpression,
        (int)SyntaxKind.ElementAccessExpression,
        (int)SyntaxKind.AddAssignmentExpression,
        (int)SyntaxKind.ObjectCreationExpression,
        (int)SyntaxKind.SimpleAssignmentExpression);

    private static ImmutableHashSet<int> IgnoredAncestors { get; } =
        ImmutableHashSet.Create(
            (int)SyntaxKind.VariableDeclarator);

    public NonSubstitutableMemberArgumentMatcherAnalyzer()
        : base(NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }

    protected override ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; } = MaybeAllowedAncestors;

    protected override ImmutableHashSet<int> IgnoredArgMatcherAncestors { get; } = IgnoredAncestors;

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}