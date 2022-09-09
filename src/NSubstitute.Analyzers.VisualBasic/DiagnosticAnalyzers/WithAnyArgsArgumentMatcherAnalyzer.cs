using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class WithAnyArgsArgumentMatcherAnalyzer : AbstractWithAnyArgsArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    internal static ImmutableHashSet<int> MaybeAllowedAncestors { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.InvocationExpression,
        (int)SyntaxKind.ObjectCreationExpression,
        (int)SyntaxKind.EqualsExpression,
        (int)SyntaxKind.SimpleAssignmentStatement);

    public WithAnyArgsArgumentMatcherAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance)
    {
    }

    protected override ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; } = MaybeAllowedAncestors;

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}