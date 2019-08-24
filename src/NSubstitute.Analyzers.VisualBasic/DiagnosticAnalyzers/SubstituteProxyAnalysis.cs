using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal sealed class SubstituteProxyAnalysis : AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax>
    {
        public static SubstituteProxyAnalysis Instance { get; } = new SubstituteProxyAnalysis();

        private SubstituteProxyAnalysis()
        {
        }

        protected override IEnumerable<ExpressionSyntax> GetTypeOfLikeExpressions(IList<ExpressionSyntax> arrayParameters)
        {
            return arrayParameters.OfType<GetTypeExpressionSyntax>();
        }

        protected override IEnumerable<ExpressionSyntax> GetArrayInitializerArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList?.Arguments.FirstOrDefault().GetExpression()
                .GetParameterExpressionsFromArrayArgument();
        }
    }
}