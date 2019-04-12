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
    internal class NonSubstitutableMemberReceivedAnalyzer : AbstractNonSubstitutableMemberReceivedAnalyzer<SyntaxKind, MemberAccessExpressionSyntax>
    {
        protected override ImmutableArray<Parent> PossibleParents { get; } = ImmutableArray.Create(
            Parent.Create<MemberAccessExpressionSyntax>(),
            Parent.Create<InvocationExpressionSyntax>(),
            Parent.Create<ElementAccessExpressionSyntax>());

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        public NonSubstitutableMemberReceivedAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }
    }
}