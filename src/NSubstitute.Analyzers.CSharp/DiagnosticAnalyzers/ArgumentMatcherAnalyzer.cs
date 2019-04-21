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
        private readonly ISubstitutionNodeFinder<InvocationExpressionSyntax> _substitutionNodeFinder = new SubstitutionNodeFinder();
        
        public ArgumentMatcherAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
        
        protected override AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax> CreateArgumentMatcherCompilationAnalyzer()
        {
            return new ArgumentMatcherCompilationAnalyzer(_substitutionNodeFinder, DiagnosticDescriptorsProvider);
        }
    }
}