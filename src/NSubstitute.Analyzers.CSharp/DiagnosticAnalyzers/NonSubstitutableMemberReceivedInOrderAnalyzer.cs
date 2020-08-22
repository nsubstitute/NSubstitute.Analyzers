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
        private static ImmutableArray<int> IgnoredPaths { get; } =
            ImmutableArray.Create((int)SyntaxKind.Argument, (int)SyntaxKind.VariableDeclarator, (int)SyntaxKind.AddAssignmentExpression);

        public NonSubstitutableMemberReceivedInOrderAnalyzer()
            : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableArray<int> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override ISymbol GetDeclarationSymbol(SemanticModel semanticModel, SyntaxNode node)
        {
            return semanticModel.GetDeclaredSymbol(node);
        }
    }
}