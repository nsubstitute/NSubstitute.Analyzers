using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class UnusedReceivedAnalyzer : AbstractUnusedReceivedAnalyzer<SyntaxKind>
    {
        protected override ImmutableArray<Parent> PossibleParents { get; } = ImmutableArray.Create(
            Parent.Create<MemberAccessExpressionSyntax>(),
            Parent.Create<InvocationExpressionSyntax>());

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        public UnusedReceivedAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }
    }
}