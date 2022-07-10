using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableMemberReceivedInOrderAnalyzer
{
    private static ImmutableArray<int> IgnoredPaths { get; } = ImmutableArray.Create(
        (int)SyntaxKind.SimpleArgument,
        (int)SyntaxKind.VariableDeclarator,
        (int)SyntaxKind.AddAssignmentStatement);

    public NonSubstitutableMemberReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }
}