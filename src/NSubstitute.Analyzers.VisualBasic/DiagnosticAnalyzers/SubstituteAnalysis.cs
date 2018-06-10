using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    public class SubstituteAnalysis : AbstractSubstituteAnalysis<InvocationExpressionSyntax>
    {
        protected override IList<SyntaxNode> GetInvocationArguments(InvocationExpressionSyntax invocationExpression)
        {
            return invocationExpression.ArgumentList.Arguments.Select<ArgumentSyntax, SyntaxNode>(syntax => syntax).ToList();
        }

        // TODO get rid of casts
        protected override IList<SyntaxNode> GetParameterExpressionsFromArrayArgument(SyntaxNode syntaxNode)
        {
            return ((ArgumentSyntax)syntaxNode).GetExpression().GetParameterExpressionsFromArrayArgument()
                .Select<ExpressionSyntax, SyntaxNode>(syntax => syntax).ToList();
        }
    }
}