using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax>
    {
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
            throw new System.NotImplementedException();
        }

        protected override ConstructorContext CollectConstructorContext(SubstituteContext<InvocationExpressionSyntax> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            throw new System.NotImplementedException();
        }
    }
}