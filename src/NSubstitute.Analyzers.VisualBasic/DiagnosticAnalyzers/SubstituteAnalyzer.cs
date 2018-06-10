using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax>
    {
        private readonly SubstituteAnalysis _substituteAnalysis = new SubstituteAnalysis();
        private readonly SubstituteConstructorMatcher _substituteConstructorMatcher = new SubstituteConstructorMatcher();

        public SubstituteAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<ExpressionSyntax> GetTypeOfLikeExpressions(IList<ExpressionSyntax> arrayParameters)
        {
            return arrayParameters.Where(param => param is GetTypeExpressionSyntax || param is TypeOfExpressionSyntax);
        }

        protected override IEnumerable<ExpressionSyntax> GetArrayInitializerArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Skip(1).First().GetExpression()
                .GetParameterExpressionsFromArrayArgument();
        }

        protected override ConstructorContext CollectConstructorContext(SubstituteContext<InvocationExpressionSyntax> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            return _substituteAnalysis.CollectConstructorContext(substituteContext, proxyTypeSymbol);
        }

        protected override bool MatchesInvocation(Compilation semanticModelCompilation, IMethodSymbol ctor, IList<ITypeSymbol> constructorContextInvocationParameters)
        {
            return _substituteConstructorMatcher.MatchesInvocation(semanticModelCompilation, ctor, constructorContextInvocationParameters);
        }
    }
}