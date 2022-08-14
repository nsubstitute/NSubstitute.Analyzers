using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class WithAnyArgsArgumentMatcherAnalyzer : AbstractWithAnyArgsArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    internal static ImmutableHashSet<int> MaybeAllowedAncestors { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.InvocationExpression,
        (int)SyntaxKind.ElementAccessExpression,
        (int)SyntaxKind.SimpleAssignmentExpression);

    public WithAnyArgsArgumentMatcherAnalyzer()
        : base(CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance)
    {
    }

    protected override ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; } = MaybeAllowedAncestors;

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}