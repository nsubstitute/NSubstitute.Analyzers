using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal class SubstituteProxyAnalysis : AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax>
    {
        protected override IEnumerable<ExpressionSyntax> GetTypeOfLikeExpressions(IList<ExpressionSyntax> arrayParameters)
        {
            return arrayParameters.Where(param => param is GetTypeExpressionSyntax || param is TypeOfExpressionSyntax);
        }

        protected override IEnumerable<ExpressionSyntax> GetArrayInitializerArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList?.Arguments.FirstOrDefault().GetExpression()
                .GetParameterExpressionsFromArrayArgument();
        }
    }
}