using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class UnusedReceivedAnalyzer : AbstractUnusedReceivedAnalyzer<SyntaxKind>
{
    protected override ImmutableHashSet<int> PossibleParentsRawKinds { get; } = ImmutableHashSet.Create(
        (int)SyntaxKind.SimpleMemberAccessExpression,
        (int)SyntaxKind.InvocationExpression,
        (int)SyntaxKind.ElementAccessExpression);

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public UnusedReceivedAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}