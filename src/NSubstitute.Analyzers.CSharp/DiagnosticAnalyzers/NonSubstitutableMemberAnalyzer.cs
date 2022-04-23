using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class NonSubstitutableMemberAnalyzer : AbstractNonSubstitutableMemberAnalyzer<SyntaxKind>
{
    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    public NonSubstitutableMemberAnalyzer()
        : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance, NonSubstitutableMemberAnalysis.Instance)
    {
    }
}