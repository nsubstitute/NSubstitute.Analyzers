using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
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
            return arrayParameters.OfType<TypeOfExpressionSyntax>();
        }

        protected override IEnumerable<ExpressionSyntax> GetArrayInitializerArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Skip(1).First().Expression
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