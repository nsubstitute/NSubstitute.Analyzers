using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    internal sealed class ArgumentMatcherAnalyzer : AbstractArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax>
    {
        public ArgumentMatcherAnalyzer()
            : base(VisualBasic.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax> CreateArgumentMatcherCompilationAnalyzer()
        {
            return new ArgumentMatcherCompilationAnalyzer(SubstitutionNodeFinder.Instance, DiagnosticDescriptorsProvider);
        }
    }
}