using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
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
            return syntaxNode.Expression.GetParameterExpressionsFromArrayArgument()?.Select<ExpressionSyntax, SyntaxNode>(syntax => syntax).ToList();
        }
    }
}