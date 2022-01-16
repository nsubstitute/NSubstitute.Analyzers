using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer<SyntaxKind, InvocationExpressionSyntax, ArgumentSyntax>
{
    public ReEntrantSetupAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, ReEntrantCallFinder.Instance)
    {
    }

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    protected override IEnumerable<SyntaxNode> GetExpressionsFromArrayInitializer(ArgumentSyntax syntaxNode)
    {
        return syntaxNode.GetExpression().GetParameterExpressionsFromArrayArgument();
    }

    protected override IEnumerable<ArgumentSyntax> GetArguments(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        return invocationExpressionSyntax.ArgumentList.Arguments;
    }

    protected override SyntaxNode GetArgumentExpression(ArgumentSyntax argumentSyntax)
    {
        return argumentSyntax.GetExpression();
    }
}