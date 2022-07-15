using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstituteAnalyzer<TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer where TInvocationExpressionSyntax : SyntaxNode
{
    private readonly ISubstituteProxyAnalysis _substituteProxyAnalysis;
    private readonly ISubstituteConstructorAnalysis _substituteConstructorAnalysis;
    private readonly ISubstituteConstructorMatcher _substituteConstructorMatcher;

    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    protected AbstractSubstituteAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstituteProxyAnalysis substituteProxyAnalysis,
        ISubstituteConstructorAnalysis substituteConstructorAnalysis,
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

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        if (operationAnalysisContext.Operation is not IInvocationOperation invocationOperation)
        {
           return;
        }

        var methodSymbol = invocationOperation.TargetMethod;

        if (methodSymbol.IsSubstituteCreateLikeMethod() == false)
        {
            return;
        }

        var substituteContext = new SubstituteContext(operationAnalysisContext, invocationOperation);

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

    private void AnalyzeSubstitute(SubstituteContext substituteContext)
    {
        if (AnalyzeProxies(substituteContext))
        {
            return;
        }

        var proxyType = _substituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext.InvocationOperation);

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

    private void AnalyzePartialSubstitute(SubstituteContext substituteContext)
    {
        if (AnalyzeProxies(substituteContext))
        {
            return;
        }

        var proxyType = _substituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext.InvocationOperation);

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

    private void AnalyzeConstructor(SubstituteContext substituteContext, ConstructorContext constructorContext)
    {
        if (AnalyzeConstructorAccessibility(substituteContext, constructorContext))
        {
            return;
        }

        if (AnalyzeConstructorParametersCount(substituteContext, constructorContext))
        {
            return;
        }

        AnalyzeConstructorInvocation(substituteContext, constructorContext);
    }

    private bool AnalyzeProxies(SubstituteContext substituteContext)
    {
        var proxies = _substituteProxyAnalysis.GetProxySymbols(substituteContext.InvocationOperation).ToList();
        var classProxies = proxies.Where(proxy => proxy.TypeKind == TypeKind.Class).Distinct();
        if (classProxies.Count() > 1)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteMultipleClasses,
                substituteContext.InvocationOperation.Syntax.GetLocation());

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorParametersCount(SubstituteContext substituteContext, ConstructorContext constructorContext)
    {
        var invocationArgumentTypes = constructorContext.InvocationParameters?.Length;
        switch (constructorContext.ConstructorType.TypeKind)
        {
            case TypeKind.Interface when invocationArgumentTypes > 0:
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForInterface,
                    substituteContext.InvocationOperation.Syntax.GetLocation(),
                    GetSubstituteMethodWithoutConstructorArguments(substituteContext.InvocationOperation.Syntax, substituteContext.InvocationOperation.TargetMethod));

                substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            case TypeKind.Interface:
                return false;
            case TypeKind.Delegate when invocationArgumentTypes > 0:
                var delegateDiagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate,
                    substituteContext.InvocationOperation.Syntax.GetLocation(),
                    GetSubstituteMethodWithoutConstructorArguments(substituteContext.InvocationOperation.Syntax, substituteContext.InvocationOperation.TargetMethod));

                substituteContext.OperationAnalysisContext.ReportDiagnostic(delegateDiagnostic);
                return true;
            case TypeKind.Delegate:
                return false;
        }

        if (constructorContext.PossibleConstructors != null && constructorContext.PossibleConstructors.Any() == false)
        {
            var symbol = substituteContext.InvocationOperation.TargetMethod;
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForConstructorParametersMismatch,
                substituteContext.InvocationOperation.Syntax.GetLocation(),
                symbol.ToMinimalMethodString(substituteContext.OperationAnalysisContext.Compilation.GetSemanticModel(substituteContext.InvocationOperation.Syntax.SyntaxTree)),
                constructorContext.ConstructorType);

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeTypeKind(SubstituteContext substituteContext, ITypeSymbol proxyType)
    {
        if (proxyType.TypeKind == TypeKind.Interface || proxyType.TypeKind == TypeKind.Delegate)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.PartialSubstituteForUnsupportedType,
                substituteContext.InvocationOperation.Syntax.GetLocation(),
                GetCorrespondingSubstituteMethod(substituteContext.InvocationOperation.Syntax, substituteContext.InvocationOperation.TargetMethod),
                substituteContext.InvocationOperation.Syntax);

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeTypeAccessability(SubstituteContext substituteContext, ITypeSymbol proxyType)
    {
        if (proxyType.DeclaredAccessibility == Accessibility.Internal && proxyType.InternalsVisibleToProxyGenerator() == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForInternalMember,
                substituteContext.InvocationOperation.Syntax.GetLocation());

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorInvocation(SubstituteContext substituteContext, ConstructorContext constructorContext)
    {
        if (constructorContext.ConstructorType.TypeKind != TypeKind.Class || constructorContext.InvocationParameters == null || constructorContext.PossibleConstructors == null)
        {
            return false;
        }

        if (constructorContext.PossibleConstructors.All(ctor =>
                _substituteConstructorMatcher.MatchesInvocation(
                    substituteContext.OperationAnalysisContext.Compilation, ctor, constructorContext.InvocationParameters) ==
                false))
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteConstructorMismatch,
                substituteContext.InvocationOperation.Syntax.GetLocation(),
                substituteContext.InvocationOperation.TargetMethod.ToMinimalMethodString(
                    substituteContext.OperationAnalysisContext.Compilation.GetSemanticModel(substituteContext
                        .InvocationOperation.Syntax.SyntaxTree)),
                constructorContext.ConstructorType.ToString());

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeConstructorAccessibility(SubstituteContext substituteContext, ConstructorContext constructorContext)
    {
        if (constructorContext.ConstructorType.TypeKind == TypeKind.Class && constructorContext.AccessibleConstructors != null && constructorContext.AccessibleConstructors.Any() == false)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.SubstituteForWithoutAccessibleConstructor,
                substituteContext.InvocationOperation.Syntax.GetLocation(),
                constructorContext.ConstructorType.ToString());

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private string GetCorrespondingSubstituteMethod(SyntaxNode syntaxNode, IMethodSymbol methodSymbol)
    {
        var invocationExpressionSyntax = (TInvocationExpressionSyntax)syntaxNode;
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

    private string GetSubstituteMethodWithoutConstructorArguments(SyntaxNode invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        return GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(
            (TInvocationExpressionSyntax)invocationExpressionSyntax, methodSymbol).ToString();
    }
}