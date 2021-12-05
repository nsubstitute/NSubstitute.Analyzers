using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class AsyncThrowsAnalyzer : AbstractAsyncThrowsAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public AsyncThrowsAnalyzer()
            : base(CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind => SyntaxKind.InvocationExpression;
    }
}