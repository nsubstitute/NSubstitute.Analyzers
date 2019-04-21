using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class ArgumentMatcherCompilationAnalyzer : AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax>
    {
        private static ImmutableArray<ImmutableArray<int>> PossibleAncestorPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.ArgumentList,
                (int)SyntaxKind.InvocationExpression),
            ImmutableArray.Create(
                (int)SyntaxKind.Argument,
                (int)SyntaxKind.BracketedArgumentList,
                (int)SyntaxKind.ElementAccessExpression));
        
        public ArgumentMatcherCompilationAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        protected override ISubstitutionNodeFinder<InvocationExpressionSyntax> GetFinder()
        {
            return new SubstitutionNodeFinder();
        }
        
        protected override SyntaxNode FindEnclosingExpression(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpression)
        {
            return GetEnclosingSyntaxNode(syntaxNodeAnalysisContext, invocationExpression);
        }

        protected override bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var parentNote = invocationExpressionSyntax.Parent;

            if (parentNote is MemberAccessExpressionSyntax)
            {
                var child = parentNote.ChildNodes().Except(new[] {invocationExpressionSyntax}).FirstOrDefault();

                return child != null && IsSetupLikeMethod(syntaxNodeContext,
                           syntaxNodeContext.SemanticModel.GetSymbolInfo(child).Symbol);
            }

            if (parentNote is ArgumentSyntax)
            {
                var operation = syntaxNodeContext.SemanticModel.GetOperation(parentNote);
                return IsSetupLikeMethod(syntaxNodeContext,
                    syntaxNodeContext.SemanticModel.GetSymbolInfo(operation.Parent.Syntax).Symbol);
            }

            return false;
        }

        protected override bool IsPrecededByReceivedInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var syntaxNodes = invocationExpressionSyntax.Parent.DescendantNodes().ToList();
            var index = syntaxNodes.IndexOf(invocationExpressionSyntax.DescendantNodes().First());

            if (index >= 0 && index + 1 < syntaxNodes.Count - 1)
            {
                var syntaxNode = syntaxNodes[index + 1];
                return IsReceivedLikeMethod(syntaxNodeContext,
                    syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol);
            }

            return false;
        }
        
        private SyntaxNode GetEnclosingSyntaxNode(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode receivedSyntaxNode)
        {
            // finding usage of Arg like method in element access expressions and method invocation
            // deliberately skipping odd usages like var x = Arg.Any<int>() in order not to report false positives
            foreach (var possibleAncestorPath in PossibleAncestorPaths)
            {
                var node = receivedSyntaxNode.GetAncestorNode(possibleAncestorPath);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }
    }
}