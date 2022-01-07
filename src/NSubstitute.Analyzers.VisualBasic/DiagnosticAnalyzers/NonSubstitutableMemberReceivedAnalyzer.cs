using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberReceivedAnalyzer : AbstractNonSubstitutableMemberReceivedAnalyzer<SyntaxKind, MemberAccessExpressionSyntax>
{
    protected override ImmutableHashSet<int> PossibleParentsRawKinds { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.SimpleMemberAccessExpression,
        (int)SyntaxKind.InvocationExpression);

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public NonSubstitutableMemberReceivedAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}