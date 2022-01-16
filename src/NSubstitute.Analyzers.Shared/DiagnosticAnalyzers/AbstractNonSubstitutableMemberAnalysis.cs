using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberAnalysis : INonSubstitutableMemberAnalysis
{
    protected abstract ImmutableHashSet<Type> KnownNonVirtualSyntaxKinds { get; }

    public NonSubstitutableMemberAnalysisResult Analyze(
        in SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode accessedMember,
        ISymbol symbol = null)
    {
        var accessedSymbol = symbol ?? syntaxNodeContext.SemanticModel.GetSymbolInfo(accessedMember).Symbol;

        if (accessedSymbol == null)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: KnownNonVirtualSyntaxKinds.Contains(accessedMember.GetType()),
                internalMemberSubstitution: false,
                symbol: null,
                member: accessedMember,
                memberName: accessedMember.ToString());
        }

        var canBeSubstituted = CanBeSubstituted(syntaxNodeContext, accessedMember, accessedSymbol);

        if (canBeSubstituted == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: true,
                internalMemberSubstitution: false,
                symbol: accessedSymbol,
                member: accessedMember,
                memberName: accessedSymbol.Name);
        }

        if (accessedSymbol.MemberVisibleToProxyGenerator() == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: false,
                internalMemberSubstitution: true,
                symbol: accessedSymbol,
                member: accessedMember,
                memberName: accessedSymbol.Name);
        }

        return new NonSubstitutableMemberAnalysisResult(
            nonVirtualMemberSubstitution: false,
            internalMemberSubstitution: false,
            symbol: accessedSymbol,
            member: accessedMember,
            memberName: accessedSymbol.Name);
    }

    protected virtual bool CanBeSubstituted(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode accessedMember,
        ISymbol symbol)
    {
        return !KnownNonVirtualSyntaxKinds.Contains(accessedMember.GetType()) &&
               CanBeSubstituted(symbol);
    }

    private static bool CanBeSubstituted(ISymbol symbol)
    {
        return IsInterfaceMember(symbol) || IsVirtual(symbol);
    }

    private static bool IsInterfaceMember(ISymbol symbol)
    {
        return symbol.ContainingType?.TypeKind == TypeKind.Interface;
    }

    private static bool IsVirtual(ISymbol symbol)
    {
        var isVirtual = symbol.IsVirtual
                        || (symbol.IsOverride && !symbol.IsSealed)
                        || symbol.IsAbstract;

        return isVirtual;
    }
}