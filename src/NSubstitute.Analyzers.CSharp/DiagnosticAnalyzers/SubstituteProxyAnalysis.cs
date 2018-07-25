using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class SubstituteProxyAnalysis : AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax>
    {
        protected override IEnumerable<ExpressionSyntax> GetTypeOfLikeExpressions(IList<ExpressionSyntax> arrayParameters)
        {
            return arrayParameters.OfType<TypeOfExpressionSyntax>();
        }

        protected override IEnumerable<ExpressionSyntax> GetArrayInitializerArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList?.Arguments.FirstOrDefault().Expression
                .GetParameterExpressionsFromArrayArgument();
        }
    }
}