using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class ArgumentMatcherCompilationAnalyzer : AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> AncestorPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.BracketedArgumentList,
                (int)SyntaxKind.ElementAccessExpression));

        public ArgumentMatcherCompilationAnalyzer(ISubstitutionNodeFinder<InvocationExpressionSyntax> substitutionNodeFinder, IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(substitutionNodeFinder, diagnosticDescriptorsProvider)
        {
        }

        protected override ImmutableArray<ImmutableArray<int>> PossibleAncestorPathsForArgument { get; } = AncestorPaths;

        protected override SyntaxNode GetOperationSyntax(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, ArgumentSyntax argumentExpression)
        {
            var operation = syntaxNodeAnalysisContext.SemanticModel.GetOperation(argumentExpression);
            return operation?.Parent?.Syntax;
        }
    }
}