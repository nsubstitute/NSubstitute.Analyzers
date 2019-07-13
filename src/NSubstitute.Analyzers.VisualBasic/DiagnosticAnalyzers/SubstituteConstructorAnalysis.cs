using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal class SubstituteConstructorAnalysis : AbstractSubstituteConstructorAnalysis<InvocationExpressionSyntax, ArgumentSyntax>
    {
        public static SubstituteConstructorAnalysis Instance { get; } = new SubstituteConstructorAnalysis();

        private SubstituteConstructorAnalysis()
        {
        }

        protected override IList<ArgumentSyntax> GetInvocationArguments(InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.ArgumentList?.Arguments.ToList();
        }

        protected override IList<SyntaxNode> GetParameterExpressionsFromArrayArgument(ArgumentSyntax syntaxNode)
        {
            return syntaxNode.GetExpression().GetParameterExpressionsFromArrayArgument()?
                .Select<ExpressionSyntax, SyntaxNode>(syntax => syntax).ToList();
        }
    }
}