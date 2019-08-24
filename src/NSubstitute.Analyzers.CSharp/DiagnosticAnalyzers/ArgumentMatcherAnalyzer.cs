using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class ArgumentMatcherAnalyzer : AbstractArgumentMatcherAnalyzer<SyntaxKind, InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax>
    {
        public ArgumentMatcherAnalyzer()
            : base(CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override AbstractArgumentMatcherCompilationAnalyzer<InvocationExpressionSyntax, MemberAccessExpressionSyntax, ArgumentSyntax> CreateArgumentMatcherCompilationAnalyzer()
        {
            return new ArgumentMatcherCompilationAnalyzer(SubstitutionNodeFinder.Instance, DiagnosticDescriptorsProvider);
        }
    }
}