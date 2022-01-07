using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstituteAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TArgumentSyntax> : AbstractDiagnosticAnalyzer
    where TSyntaxKind : struct
    where TInvocationExpressionSyntax : SyntaxNode
    where TExpressionSyntax : SyntaxNode
    where TArgumentSyntax : SyntaxNode
{
    private readonly ISubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> _substituteProxyAnalysis;
    private readonly ISubstituteConstructorAnalysis<TInvocationExpressionSyntax> _substituteConstructorAnalysis;
    private readonly ISubstituteConstructorMatcher _substituteConstructorMatcher;

    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected AbstractSubstituteAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> substituteProxyAnalysis,
        ISubstituteConstructorAnalysis<TInvocationExpressionSyntax> substituteConstructorAnalysis,
        ISubstituteConstructorMatcher substituteConstructorMatcher)
        : base(diagnosticDescriptorsProvider)
    {
        _analyzeInvocationAction = AnalyzeInvocation;
        _substituteProxyAnalysis = substituteProxyAnalysis;
        _substituteConstructorAnalysis = substituteConstructorAnalysis;
        _substituteConstructorMatcher = substituteConstructorMatcher;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.PartialSubstituteForUnsupportedType,
            DiagnosticDescriptorsProvider.SubstituteForWithoutAccessibleConstructor,
            DiagnosticDescriptorsProvider.SubstituteForConstructorParametersMismatch,
            DiagnosticDescriptorsProvider.SubstituteForInternalMember,
            DiagnosticDescriptorsProvider.SubstituteConstructorMismatch,
            DiagnosticDescriptorsProvider.SubstituteMultipleClasses,
            DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForInterface,
            DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate);
    }

    protected abstract TInvocationExpressionSyntax GetCorrespondingSubstituteInvocationExpressionSyntax(TInvocationExpressionSyntax invocationExpressionSyntax, string substituteName);

    protected abstract TInvocationExpressionSyntax GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
        {
            return;
        }

        var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;
        if (methodSymbol == null || methodSymbol.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        if (methodSymbol.IsSubstituteCreateLikeMethod() == false)
        {
            return;
        }

        var substituteContext = new SubstituteContext<TInvocationExpressionSyntax>(syntaxNodeContext, invocationExpression, methodSymbol);

        if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForMethod, StringComparison.Ordinal) ||
            methodSymbol.Name.Equals(MetadataNames.SubstituteFactoryCreate, StringComparison.Ordinal))
        {
            AnalyzeSubstitute(substituteContext);
            return;
        }

        if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForPartsOfMethod, StringComparison.Ordinal) ||
            methodSymbol.Name.Equals(MetadataNames.SubstituteFactoryCreatePartial, StringComparison.Ordinal))
        {
            AnalyzePartialSubstitute(substituteContext);
        }
    }

    private void AnalyzeSubstitute(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
    {
        if (AnalyzeProxies(substituteContext))
        {
            return;
        }

        var proxyType = _substituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext);

        if (proxyType == null)
        {
            return;
        }

        if (AnalyzeTypeAccessability(substituteContext, proxyType))
        {
            return;
        }

        var constructorContext = _substituteConstructorAnalysis.CollectConstructorContext(substituteContext, proxyType);
        AnalyzeConstructor(substituteContext, constructorContext);
    }

    private void AnalyzePartialSubstitute(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
    {
        if (AnalyzeProxies(substituteContext))
        {
            return;
        }

        var proxyType = _substituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext);

        if (proxyType == null)
        {
            return;
        }

        if (AnalyzeTypeKind(substituteContext, proxyType))
        {
            return;
        }

        if (AnalyzeTypeAccessability(substituteContext, proxyType))
        {
            return;
        }

        if (proxyType.TypeKind != TypeKind.Class)
        {
            return;
        }

        var constructorContext = _substituteConstructorAnalysis.CollectConstructorContext(substituteContext, proxyType);
        AnalyzeConstructor(substituteContext, constructorContext);
    }

    private void AnalyzeConstructor(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
    {
        if (AnalyzeConstructorAccessibility(substituteContext, constructorContext))
        {
            return;
        }

        if (AnalyzeConstructorParametersCount(substituteContext, constructorContext))
        {
            return;
        }

        if (AnalyzeConstructorInvocation(substituteContext, constructorContext))
        {
            return;
        }
    }

    private bool AnalyzeProxies(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
    {
        var proxies = _substituteProxyAnalysis.GetProxySymbols(substituteContext).ToList();
        var classProxies = proxies.Where(proxy => proxy.TypeKind == TypeKind.Class).Distinct();
        if (classProxies.Count() > 1)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteMultipleClasses,
                substituteContext.InvocationExpression.GetLocation());

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorParametersCount(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
    {
        var invocationArgumentTypes = constructorContext.InvocationParameters?.Length;
        switch (constructorContext.ConstructorType.TypeKind)
        {
            case TypeKind.Interface when invocationArgumentTypes > 0:
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForInterface,
                    substituteContext.InvocationExpression.GetLocation(),
                    GetSubstituteMethodWithoutConstructorArguments(substituteContext.InvocationExpression, substituteContext.MethodSymbol));

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            case TypeKind.Interface:
                return false;
            case TypeKind.Delegate when invocationArgumentTypes > 0:
                var delegateDiagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate,
                    substituteContext.InvocationExpression.GetLocation(),
                    GetSubstituteMethodWithoutConstructorArguments(substituteContext.InvocationExpression, substituteContext.MethodSymbol));

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(delegateDiagnostic);
                return true;
            case TypeKind.Delegate:
                return false;
        }

        if (constructorContext.PossibleConstructors != null && constructorContext.PossibleConstructors.Any() == false)
        {
            var symbol = substituteContext.SyntaxNodeAnalysisContext.SemanticModel.GetSymbolInfo(substituteContext.InvocationExpression);
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForConstructorParametersMismatch,
                substituteContext.InvocationExpression.GetLocation(),
                symbol.Symbol.ToMinimalMethodString(substituteContext.SyntaxNodeAnalysisContext.SemanticModel),
                constructorContext.ConstructorType);

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeTypeKind(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ITypeSymbol proxyType)
    {
        if (proxyType.TypeKind == TypeKind.Interface || proxyType.TypeKind == TypeKind.Delegate)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.PartialSubstituteForUnsupportedType,
                substituteContext.InvocationExpression.GetLocation(),
                GetCorrespondingSubstituteMethod(substituteContext.InvocationExpression, substituteContext.MethodSymbol),
                substituteContext.InvocationExpression.ToString());

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeTypeAccessability(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ITypeSymbol proxyType)
    {
        if (proxyType.DeclaredAccessibility == Accessibility.Internal && proxyType.InternalsVisibleToProxyGenerator() == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForInternalMember,
                substituteContext.InvocationExpression.GetLocation());

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorInvocation(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
    {
        if (constructorContext.ConstructorType.TypeKind != TypeKind.Class || constructorContext.InvocationParameters == null || constructorContext.PossibleConstructors == null)
        {
            return false;
        }

        if (constructorContext.PossibleConstructors.All(ctor =>
                _substituteConstructorMatcher.MatchesInvocation(
                    substituteContext.SyntaxNodeAnalysisContext.SemanticModel.Compilation, ctor, constructorContext.InvocationParameters) ==
                false))
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteConstructorMismatch,
                substituteContext.InvocationExpression.GetLocation(),
                substituteContext.MethodSymbol.ToMinimalMethodString(substituteContext.SyntaxNodeAnalysisContext.SemanticModel),
                constructorContext.ConstructorType.ToString());

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorAccessibility(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
    {
        if (constructorContext.ConstructorType.TypeKind == TypeKind.Class && constructorContext.AccessibleConstructors != null && constructorContext.AccessibleConstructors.Any() == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForWithoutAccessibleConstructor,
                substituteContext.InvocationExpression.GetLocation(),
                constructorContext.ConstructorType.ToString());

            substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private string GetCorrespondingSubstituteMethod(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        switch (methodSymbol.Name)
        {
            case MetadataNames.SubstituteFactoryCreatePartial:
                return GetCorrespondingSubstituteInvocationExpressionSyntax(invocationExpressionSyntax, MetadataNames.SubstituteFactoryCreate).ToString();
            case MetadataNames.NSubstituteForPartsOfMethod:
                return GetCorrespondingSubstituteInvocationExpressionSyntax(invocationExpressionSyntax, MetadataNames.NSubstituteForMethod).ToString();
            default:
                return string.Empty;
        }
    }

    private string GetSubstituteMethodWithoutConstructorArguments(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        return GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(invocationExpressionSyntax, methodSymbol).ToString();
    }
}