using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class ConflictingArgumentAssignmentsAnalyzer : AbstractConflictingArgumentAssignmentsAnalyzer<SyntaxKind>
{
    public ConflictingArgumentAssignmentsAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, CallInfoCallFinder.Instance)
    {
    }

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;
}