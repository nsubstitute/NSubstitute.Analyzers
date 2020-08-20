using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class NonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableMemberReceivedInOrderAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax, BlockSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> IgnoredPaths { get; } = ImmutableArray.Create(
                ImmutableArray.Create(
                    (int)SyntaxKind.Argument),
                ImmutableArray.Create(
                    (int)SyntaxKind.AwaitExpression,
                    (int)SyntaxKind.Argument),
                ImmutableArray.Create(
                    (int)SyntaxKind.CastExpression,
                    (int)SyntaxKind.Argument),
                ImmutableArray.Create(
                    (int)SyntaxKind.AsExpression,
                    (int)SyntaxKind.Argument),
                ImmutableArray.Create(
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator),
                ImmutableArray.Create(
                    (int)SyntaxKind.AwaitExpression,
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator),
                ImmutableArray.Create(
                    (int)SyntaxKind.AsExpression,
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator),
                ImmutableArray.Create(
                    (int)SyntaxKind.CastExpression,
                    (int)SyntaxKind.EqualsValueClause,
                    (int)SyntaxKind.VariableDeclarator));

        public NonSubstitutableMemberReceivedInOrderAnalyzer()
            : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override ISymbol GetDeclarationSymbol(SemanticModel semanticModel, SyntaxNode node)
        {
            return semanticModel.GetDeclaredSymbol(node);
        }
    }
}