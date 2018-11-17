using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ArgumentMatcherAnalyzer : AbstractArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        private ImmutableArray<ImmutableArray<Parent>> PossibleAncestorPaths { get; } = ImmutableArray.Create(
            ImmutableArray.Create(
                Parent.Create<ArgumentSyntax>(),
                Parent.Create<ArgumentListSyntax>(),
                Parent.Create<InvocationExpressionSyntax>()),
            ImmutableArray.Create(
                Parent.Create<ArgumentSyntax>(),
                Parent.Create<BracketedArgumentListSyntax>(),
                Parent.Create<ElementAccessExpressionSyntax>()));

        public ArgumentMatcherAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override SyntaxNode FindEnclosingExpression(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpression)
        {
            return GetEnclosingSyntaxNode(syntaxNodeAnalysisContext, invocationExpression);
        }

        protected override bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var parentNote = invocationExpressionSyntax.Parent;

            if (parentNote is MemberAccessExpressionSyntax)
            {
                var child = parentNote.ChildNodes().Except(new[] { invocationExpressionSyntax }).FirstOrDefault();

                return child != null && IsSetupLikeMethod(syntaxNodeContext, syntaxNodeContext.SemanticModel.GetSymbolInfo(child).Symbol);
            }

            if (parentNote is ArgumentSyntax)
            {
                var operation = syntaxNodeContext.SemanticModel.GetOperation(parentNote);
                return IsSetupLikeMethod(syntaxNodeContext, syntaxNodeContext.SemanticModel.GetSymbolInfo(operation.Parent.Syntax).Symbol);
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
                return IsReceivedLikeMethod(syntaxNodeContext, syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol);
            }

            return false;
        }

        protected override bool IsWithinWhenInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax)
        {
            var argumentListSyntax = invocationExpressionSyntax.Ancestors().OfType<ArgumentListSyntax>().FirstOrDefault();
            return argumentListSyntax?.Parent != null && IsWhenLikeMethod(syntaxNodeContext, syntaxNodeContext.SemanticModel.GetSymbolInfo(argumentListSyntax.Parent).Symbol);
        }

        private SyntaxNode GetEnclosingSyntaxNode(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode receivedSyntaxNode)
        {
            // finding usage of Arg like method in element access expressions and method invocation
            // deliberately skipping odd usages like var x = Arg.Any<int>() in order not to report false positives
            foreach (var possibleAncestorPath in PossibleAncestorPaths)
            {
                using (var syntaxEnumerator = receivedSyntaxNode.Ancestors().GetEnumerator())
                {
                    var parentsEnumerator = possibleAncestorPath.GetEnumerator();
                    while (parentsEnumerator.MoveNext() && syntaxEnumerator.MoveNext())
                    {
                        if (parentsEnumerator.Current.Type.GetTypeInfo()
                                .IsAssignableFrom(syntaxEnumerator.Current.GetType().GetTypeInfo()) == false)
                        {
                            break;
                        }
                    }

                    if (parentsEnumerator.MoveNext() == false)
                    {
                        return syntaxEnumerator.Current;
                    }
                }
            }

            return null;
        }
    }
}