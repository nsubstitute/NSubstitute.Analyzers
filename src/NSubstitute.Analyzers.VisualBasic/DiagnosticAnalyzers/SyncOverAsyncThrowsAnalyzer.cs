using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsAnalyzer : AbstractSyncOverAsyncThrowsAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    public SyncOverAsyncThrowsAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance)
    {
    }

    protected override SyntaxKind InvocationExpressionKind => SyntaxKind.InvocationExpression;
}