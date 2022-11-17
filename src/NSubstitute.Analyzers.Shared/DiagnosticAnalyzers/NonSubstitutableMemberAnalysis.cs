using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class NonSubstitutableMemberAnalysis : INonSubstitutableMemberAnalysis
{
    public static readonly INonSubstitutableMemberAnalysis Instance = new NonSubstitutableMemberAnalysis();

    private static readonly ImmutableHashSet<OperationKind> KnownNonVirtualOperationKinds =
        ImmutableHashSet.Create(OperationKind.Literal);

    public NonSubstitutableMemberAnalysisResult Analyze(IOperation operation)
    {
        var symbol = operation.ExtractSymbol();

        return Analyze(operation, symbol);
    }

    private bool CanBeSubstituted(
        IOperation operation,
        ISymbol symbol) =>
        !KnownNonVirtualOperationKinds.Contains(operation.Kind) && CanBeSubstituted(symbol);

    private NonSubstitutableMemberAnalysisResult Analyze(IOperation operation, ISymbol symbol)
    {
        if (symbol == null)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: KnownNonVirtualOperationKinds.Contains(ExtractActualOperation(operation).Kind),
                internalMemberSubstitution: false,
                symbol: null,
                member: operation.Syntax,
                memberName: operation.Syntax.ToString());
        }

        var canBeSubstituted = CanBeSubstituted(operation, symbol);

        if (canBeSubstituted == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: true,
                internalMemberSubstitution: false,
                symbol: symbol,
                member: operation.Syntax,
                memberName: symbol.Name);
        }

        if (symbol.MemberVisibleToProxyGenerator() == false)
        {
            return new NonSubstitutableMemberAnalysisResult(
                nonVirtualMemberSubstitution: false,
                internalMemberSubstitution: true,
                symbol: symbol,
                member: operation.Syntax,
                memberName: symbol.Name);
        }

        return new NonSubstitutableMemberAnalysisResult(
            nonVirtualMemberSubstitution: false,
            internalMemberSubstitution: false,
            symbol: symbol,
            member: operation.Syntax,
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

    private static IOperation ExtractActualOperation(IOperation operation)
    {
        return operation switch
        {
            IConversionOperation conversionOperation => conversionOperation.Operand,
            _ => operation
        };
    }
}