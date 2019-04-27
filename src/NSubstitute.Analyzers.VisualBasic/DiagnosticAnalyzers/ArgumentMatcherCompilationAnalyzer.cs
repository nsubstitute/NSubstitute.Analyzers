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
    internal class ArgumentMatcherCompilationAnalyzer : AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> AncestorPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.SimpleArgument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression));

        public ArgumentMatcherCompilationAnalyzer(ISubstitutionNodeFinder<InvocationExpressionSyntax> substitutionNodeFinder, IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(substitutionNodeFinder, diagnosticDescriptorsProvider)
        {
        }

        protected override ImmutableArray<ImmutableArray<int>> PossibleAncestorPaths { get; } = AncestorPaths;

        protected override bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var parentNote = invocationExpressionSyntax.Parent;

            if (parentNote is MemberAccessExpressionSyntax)
            {
                var child = parentNote.ChildNodes().Except(new[] { invocationExpressionSyntax }).FirstOrDefault();

                return child != null && IsSetupLikeMethod(syntaxNodeContext.SemanticModel.GetSymbolInfo(child).Symbol);
            }

            if (parentNote is ArgumentSyntax)
            {
                var operation = syntaxNodeContext.SemanticModel.GetOperation(parentNote);
                return IsSetupLikeMethod(syntaxNodeContext.SemanticModel.GetSymbolInfo(operation.Parent.Syntax).Symbol);
            }

            return false;
        }
    }
}