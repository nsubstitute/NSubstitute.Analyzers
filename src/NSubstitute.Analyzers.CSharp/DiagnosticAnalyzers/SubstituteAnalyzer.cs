using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SubstituteAnalyzer : AbstractSubstituteAnalyzer<SyntaxKind, InvocationExpressionSyntax, ExpressionSyntax>
    {
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
            throw new System.NotImplementedException();
        }

        protected override ConstructorContext CollectConstructorContext(SubstituteContext<InvocationExpressionSyntax> substituteContext, ITypeSymbol proxyTypeSymbol)
        {
            throw new System.NotImplementedException();
        }
    }
}