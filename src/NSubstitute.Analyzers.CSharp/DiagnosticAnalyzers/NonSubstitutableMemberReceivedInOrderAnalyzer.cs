using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableMemberReceivedInOrderAnalyzer
{
    private static ImmutableArray<int> IgnoredPaths { get; } =
        ImmutableArray.Create((int)SyntaxKind.Argument, (int)SyntaxKind.VariableDeclarator, (int)SyntaxKind.AddAssignmentExpression);

    public NonSubstitutableMemberReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
    {
    }
}