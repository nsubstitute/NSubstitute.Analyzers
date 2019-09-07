using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal sealed class ArgumentMatcherCompilationAnalyzer : AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> AllowedPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression));

        private static ImmutableArray<ImmutableArray<int>> IgnoredPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.EqualsValue,
                (int)SyntaxKind.VariableDeclarator));

        public ArgumentMatcherCompilationAnalyzer(ISubstitutionNodeFinder<InvocationExpressionSyntax> substitutionNodeFinder, IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(substitutionNodeFinder, diagnosticDescriptorsProvider)
        {
        }

        protected override ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; } = AllowedPaths;

        protected override ImmutableArray<ImmutableArray<int>> IgnoredAncestorPaths { get; } = IgnoredPaths;

        protected override SyntaxNode GetOperationSyntax(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ArgumentSyntax argumentExpression)
        {
            var operation = syntaxNodeAnalysisContext.SemanticModel.GetOperation(argumentExpression);
            return operation?.Parent?.Syntax;
        }

        protected override IEnumerable<SyntaxNode> TryGetArgumentExpressions(
            SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode syntaxNode)
        {
            SeparatedSyntaxList<ArgumentSyntax> argumentList = default;
            switch (syntaxNode)
            {
                case InvocationExpressionSyntax invocation:
                    argumentList = invocation.ArgumentList.Arguments;
                    break;
            }

            return argumentList.Select<ArgumentSyntax, SyntaxNode>(node => node.GetExpression());
        }
    }
}