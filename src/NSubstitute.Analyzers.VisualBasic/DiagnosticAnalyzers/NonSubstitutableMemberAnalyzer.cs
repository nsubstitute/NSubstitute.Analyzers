using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer<SyntaxKind>
{
    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public NonSubstitutableMemberAnalyzer()
        : base(NSubstitute.Analyzers.VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}