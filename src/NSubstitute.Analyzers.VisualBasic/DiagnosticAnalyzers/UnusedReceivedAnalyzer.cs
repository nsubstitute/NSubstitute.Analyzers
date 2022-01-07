using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class UnusedReceivedAnalyzer : AbstractUnusedReceivedAnalyzer<SyntaxKind>
{
    protected override ImmutableHashSet<int> PossibleParentsRawKinds { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.SimpleMemberAccessExpression,
        (int)SyntaxKind.InvocationExpression);

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public UnusedReceivedAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }
}