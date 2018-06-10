using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
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
            return ((ArgumentSyntax)syntaxNode).Expression.GetParameterExpressionsFromArrayArgument()
                .Select<ExpressionSyntax, SyntaxNode>(syntax => syntax).ToList();
        }
    }
}