using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    protected override ImmutableHashSet<int> SupportedMemberAccesses { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.InvocationExpression,
        (int)SyntaxKind.SimpleMemberAccessExpression,
        (int)SyntaxKind.ElementAccessExpression,
        (int)SyntaxKind.NumericLiteralExpression,
        (int)SyntaxKind.CharacterLiteralExpression,
        (int)SyntaxKind.FalseLiteralExpression,
        (int)SyntaxKind.TrueLiteralExpression,
        (int)SyntaxKind.StringLiteralExpression);

    public NonSubstitutableMemberAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}