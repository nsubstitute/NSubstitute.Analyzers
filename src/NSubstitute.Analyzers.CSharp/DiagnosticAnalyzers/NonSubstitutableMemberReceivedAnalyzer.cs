using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class NonSubstitutableMemberReceivedAnalyzer : AbstractNonSubstitutableMemberReceivedAnalyzer<SyntaxKind, MemberAccessExpressionSyntax>
    {
        protected override ImmutableHashSet<int> PossibleParentsRawKinds { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.SimpleMemberAccessExpression,
            (int)SyntaxKind.InvocationExpression,
            (int)SyntaxKind.ElementAccessExpression);

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        public NonSubstitutableMemberReceivedAnalyzer()
            : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }
    }
}