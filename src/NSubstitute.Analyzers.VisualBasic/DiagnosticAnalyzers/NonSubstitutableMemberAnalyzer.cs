using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableHashSet<int> SupportedMemberAccesses { get; } = ImmutableHashSet.Create(
            (int)SyntaxKind.InvocationExpression,
            (int)SyntaxKind.SimpleMemberAccessExpression,
            (int)SyntaxKind.NumericLiteralExpression,
            (int)SyntaxKind.CharacterLiteralExpression,
            (int)SyntaxKind.FalseLiteralExpression,
            (int)SyntaxKind.TrueLiteralExpression,
            (int)SyntaxKind.StringLiteralExpression);

        public NonSubstitutableMemberAnalyzer()
            : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
        {
        }
    }
}