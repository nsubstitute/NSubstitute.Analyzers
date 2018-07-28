using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax, TArgumentSyntax> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
        where TArgumentSyntax : SyntaxNode
    {
        private readonly Lazy<AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax>> _substituteProxyAnalysis;
        private readonly Lazy<AbstractSubstituteConstructorAnalysis<TInvocationExpressionSyntax, TArgumentSyntax>> _substituteConstructorAnalysis;
        private readonly Lazy<AbstractSubstituteConstructorMatcher> _substituteConstructorMatcher;

        private AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> SubstituteProxyAnalysis => _substituteProxyAnalysis.Value;

        private AbstractSubstituteConstructorAnalysis<TInvocationExpressionSyntax, TArgumentSyntax> SubstituteConstructorAnalysis => _substituteConstructorAnalysis.Value;

        private AbstractSubstituteConstructorMatcher SubstituteConstructorMatcher => _substituteConstructorMatcher.Value;

        private static readonly ImmutableDictionary<string, string> MethodNamesMap =
            new Dictionary<string, string>
            {
                [MetadataNames.NSubstituteForMethod] = MetadataNames.NSubstituteSubstituteFullTypeName,
                [MetadataNames.NSubstituteForPartsOfMethod] = MetadataNames.NSubstituteSubstituteFullTypeName,
                [MetadataNames.SubstituteFactoryCreate] = MetadataNames.NSubstituteFactoryFullTypeName,
                [MetadataNames.SubstituteFactoryCreatePartial] = MetadataNames.NSubstituteFactoryFullTypeName
            }.ToImmutableDictionary();

        protected AbstractSubstituteAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
            _substituteProxyAnalysis = new Lazy<AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax>>(GetSubstituteProxyAnalysis);
            _substituteConstructorAnalysis = new Lazy<AbstractSubstituteConstructorAnalysis<TInvocationExpressionSyntax, TArgumentSyntax>>(GetSubstituteConstructorAnalysis);
            _substituteConstructorMatcher = new Lazy<AbstractSubstituteConstructorMatcher>(GetSubstituteConstructorMatcher);
        }

        protected abstract AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> GetSubstituteProxyAnalysis();

        protected abstract AbstractSubstituteConstructorAnalysis<TInvocationExpressionSyntax, TArgumentSyntax> GetSubstituteConstructorAnalysis();

        protected abstract AbstractSubstituteConstructorMatcher GetSubstituteConstructorMatcher();

        protected abstract TInvocationExpressionSyntax GetCorrespondingSubstituteInvocationExpressionSyntax(TInvocationExpressionSyntax invocationExpressionSyntax, string substituteName);

        protected abstract TInvocationExpressionSyntax GetSubstituteInvocationExpressionSyntaxWithoutConstructorArguments(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.PartialSubstituteForUnsupportedType,
            DiagnosticDescriptorsProvider.SubstituteForWithoutAccessibleConstructor,
            DiagnosticDescriptorsProvider.SubstituteForConstructorParametersMismatch,
            DiagnosticDescriptorsProvider.SubstituteForInternalMember,
            DiagnosticDescriptorsProvider.SubstituteConstructorMismatch,
            DiagnosticDescriptorsProvider.SubstituteMultipleClasses,
            DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForInterface,
            DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate);

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, InvocationExpressionKind);
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

            if (IsSubstituteMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
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

            var proxyType = SubstituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext);

            if (proxyType == null)
            {
                return;
            }

            if (AnalyzeTypeAccessability(substituteContext, proxyType))
            {
                return;
            }

            var constructorContext = SubstituteConstructorAnalysis.CollectConstructorContext(substituteContext, proxyType);
            AnalyzeConstructor(substituteContext, constructorContext);
        }

        private void AnalyzePartialSubstitute(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            if (AnalyzeProxies(substituteContext))
            {
                return;
            }

            var proxyType = SubstituteProxyAnalysis.GetActualProxyTypeSymbol(substituteContext);

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

            var constructorContext = SubstituteConstructorAnalysis.CollectConstructorContext(substituteContext, proxyType);
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
            var proxies = SubstituteProxyAnalysis.GetProxySymbols(substituteContext).ToList();
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
            var invocationArgumentTypes = constructorContext.InvocationParameters?.Count;
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
                    SubstituteConstructorMatcher.MatchesInvocation(
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

        private static bool IsSubstituteMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNamesMap.TryGetValue(memberName, out var containingType) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(containingType, StringComparison.Ordinal) == true;
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
}