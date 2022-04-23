using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class ReceivedInReceivedInOrderAnalyzer : AbstractReceivedInReceivedInOrderAnalyzer<SyntaxKind>
{
    public ReceivedInReceivedInOrderAnalyzer()
        : base(SubstitutionNodeFinder.Instance, VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}