using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class ReceivedInReceivedInOrderAnalyzer : AbstractReceivedInReceivedInOrderAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public ReceivedInReceivedInOrderAnalyzer()
            : base(SubstitutionNodeFinder.Instance, CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
    }
}