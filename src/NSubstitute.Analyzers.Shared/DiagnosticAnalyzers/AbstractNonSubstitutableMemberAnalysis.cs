using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberAnalysis : INonSubstitutableMemberAnalysis
{
    protected abstract ImmutableHashSet<Type> KnownNonVirtualSyntaxKinds { get; }

    public NonSubstitutableMemberAnalysisResult Analyze(
        in SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode accessedMember,
        ISymbol symbol = null) =>
        Analyze(accessedMember, symbol ?? syntaxNodeContext.SemanticModel.GetSymbolInfo(accessedMember).Symbol);

    public NonSubstitutableMemberAnalysisResult Analyze(IOperation operation)
    {
        var symbol = ExtractSymbol(operation);

        return Analyze(operation.Syntax, symbol);
    }

    private bool CanBeSubstituted(
        SyntaxNode accessedMember,
        ISymbol symbol)
    {
        return !KnownNonVirtualSyntaxKinds.Contains(accessedMember.GetType()) &&
               CanBeSubstituted(symbol);
    }

    private NonSubstitutableMemberAnalysisResult Analyze(SyntaxNode accessedMember, ISymbol symbol)
    {
        if (symbol == null)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: KnownNonVirtualSyntaxKinds.Contains(accessedMember.GetType()),
                internalMemberSubstitution: false,
                symbol: null,
                member: accessedMember,
                memberName: accessedMember.ToString());
        }

        var canBeSubstituted = CanBeSubstituted(accessedMember, symbol);

        if (canBeSubstituted == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: true,
                internalMemberSubstitution: false,
                symbol: symbol,
                member: accessedMember,
                memberName: symbol.Name);
        }

        if (symbol.MemberVisibleToProxyGenerator() == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: false,
                internalMemberSubstitution: true,
                symbol: symbol,
                member: accessedMember,
                memberName: symbol.Name);
        }

        return new NonSubstitutableMemberAnalysisResult(
            nonVirtualMemberSubstitution: false,
            internalMemberSubstitution: false,
            symbol: symbol,
            member: accessedMember,
            memberName: symbol.Name);
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

    private static ISymbol ExtractSymbol(IOperation operation)
    {
        var symbol = operation switch
        {
            IInvocationOperation invocationOperation => invocationOperation.TargetMethod,
            IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Property,
            IConversionOperation conversionOperation => ExtractSymbol(conversionOperation.Operand),
            _ => null
        };
        return symbol;
    }
}