using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer<SyntaxKind>
    {
        public ReEntrantSetupAnalyzer()
            : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, ReEntrantCallFinder.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}