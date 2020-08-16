using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class NonSubstitutableMemberReceivedInOrderAnalyzer : AbstractNonSubstitutableMemberReceivedInOrderAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax, LambdaExpressionSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> IgnoredPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.TryCastExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.PredefinedCastExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.DirectCastExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.CTypeExpression,
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.TryCastExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.PredefinedCastExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.DirectCastExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator),
            ImmutableArray.Create(
                (int)SyntaxKind.CTypeExpression,
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator));

        public NonSubstitutableMemberReceivedInOrderAnalyzer()
            : base(SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance, NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override ISymbol GetDeclarationSymbol(SemanticModel semanticModel, SyntaxNode node)
        {
            var symbol = semanticModel.GetDeclaredSymbol(node);

            if (symbol != null)
            {
                return symbol;
            }

            if (!(node is VariableDeclaratorSyntax variableDeclaratorSyntax))
            {
                return null;
            }

            var modifiedIdentifierSyntax = variableDeclaratorSyntax.Names.FirstOrDefault();

            if (modifiedIdentifierSyntax == null)
            {
                return null;
            }

            return semanticModel.GetDeclaredSymbol(modifiedIdentifierSyntax);
        }
    }
}