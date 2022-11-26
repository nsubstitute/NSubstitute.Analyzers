using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal readonly struct NonSubstitutableMemberAnalysisResult
{
    public bool CanBeSubstituted { get; }

    public bool NonVirtualMemberSubstitution { get; }

    public bool InternalMemberSubstitution { get; }

    public ISymbol? Symbol { get; }

    public string MemberName { get; }

    public SyntaxNode Member { get; }

    public NonSubstitutableMemberAnalysisResult(
        bool nonVirtualMemberSubstitution,
        bool internalMemberSubstitution,
        ISymbol? symbol,
        SyntaxNode member,
        string memberName)
    {
        NonVirtualMemberSubstitution = nonVirtualMemberSubstitution;
        InternalMemberSubstitution = internalMemberSubstitution;
        Member = member;
        MemberName = memberName;
        Symbol = symbol;
        CanBeSubstituted = !NonVirtualMemberSubstitution && !InternalMemberSubstitution;
    }
}