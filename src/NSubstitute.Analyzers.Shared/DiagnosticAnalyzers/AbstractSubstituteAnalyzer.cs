using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstituteAnalyzer : AbstractDiagnosticAnalyzer
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

    protected abstract SyntaxNode GetCorrespondingSubstituteInvocationExpressionSyntax(IInvocationOperation invocationOperation, string substituteName);

    protected abstract SyntaxNode GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(IInvocationOperation invocationOperation);

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

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

        if (AnalyzeTypeAccessibility(substituteContext, proxyType))
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

        if (AnalyzeTypeAccessibility(substituteContext, proxyType))
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
                    GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(substituteContext.InvocationOperation));

                substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            case TypeKind.Interface:
                return false;
            case TypeKind.Delegate when invocationArgumentTypes > 0:
                var delegateDiagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate,
                    substituteContext.InvocationOperation.Syntax.GetLocation(),
                    GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(substituteContext.InvocationOperation));

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
        if (proxyType.TypeKind is TypeKind.Interface or TypeKind.Delegate)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.PartialSubstituteForUnsupportedType,
                substituteContext.InvocationOperation.Syntax.GetLocation(),
                GetCorrespondingSubstituteMethod(substituteContext.InvocationOperation),
                substituteContext.InvocationOperation.Syntax);

            substituteContext.OperationAnalysisContext.ReportDiagnostic(diagnostic);
            return true;
        }

        return false;
    }

    private bool AnalyzeTypeAccessibility(SubstituteContext substituteContext, ITypeSymbol proxyType)
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

    private string GetCorrespondingSubstituteMethod(IInvocationOperation invocationOperation)
    {
        return invocationOperation.TargetMethod.Name switch
        {
            MetadataNames.SubstituteFactoryCreatePartial => GetCorrespondingSubstituteInvocationExpressionSyntax(
                    invocationOperation, MetadataNames.SubstituteFactoryCreate)
                .ToString(),
            MetadataNames.NSubstituteForPartsOfMethod => GetCorrespondingSubstituteInvocationExpressionSyntax(
                    invocationOperation, MetadataNames.NSubstituteForMethod)
                .ToString(),
            _ => string.Empty
        };
    }
}