using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal class ArgumentMatcherAnalyzer : AbstractArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax>
    {
        private readonly ISubstitutionNodeFinder<InvocationExpressionSyntax> _substitutionNodeFinder = new SubstitutionNodeFinder();

        public ArgumentMatcherAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax> CreateArgumentMatcherCompilationAnalyzer()
        {
            return new ArgumentMatcherCompilationAnalyzer(_substitutionNodeFinder, DiagnosticDescriptorsProvider);
        }
    }
}