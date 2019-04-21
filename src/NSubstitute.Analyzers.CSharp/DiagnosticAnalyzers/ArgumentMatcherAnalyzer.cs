using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ArgumentMatcherAnalyzer : AbstractArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax>
    {
        public ArgumentMatcherAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
        
        protected override AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax> CreateArgumentMatcherCompilationAnalyzer()
        {
            return new ArgumentMatcherCompilationAnalyzer(DiagnosticDescriptorsProvider);
        }
    }
}