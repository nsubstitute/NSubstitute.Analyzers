using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TExpressionSyntax> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
    {
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteForMethod,
            MetadataNames.NSubstituteForPartsOfMethod);

        protected AbstractSubstituteAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptorsProvider.SubstituteForPartsOfUsedForInterface,
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

        protected abstract IEnumerable<TExpressionSyntax> GetTypeOfLikeExpressions(IList<TExpressionSyntax> arrayParameters);

        protected abstract IEnumerable<TExpressionSyntax> GetArrayInitializerArguments(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract ConstructorContext CollectConstructorContext(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ITypeSymbol proxyTypeSymbol);

        protected abstract bool MatchesInvocation(Compilation semanticModelCompilation, IMethodSymbol ctor, IList<ITypeSymbol> constructorContextInvocationParameters);

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

            if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForMethod, StringComparison.Ordinal))
            {
                AnalyzeSubstituteForMethod(substituteContext);
                return;
            }

            if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForPartsOfMethod, StringComparison.Ordinal))
            {
                AnalyzeSubstituteForPartsOf(substituteContext);
                return;
            }
        }

        private void AnalyzeSubstituteForMethod(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            if (AnalyzeProxies(substituteContext))
            {
                return;
            }

            var proxyType = GetActualProxyTypeSymbol(substituteContext);

            if (proxyType == null)
            {
                return;
            }

            if (AnalyzeTypeAccessability(substituteContext, proxyType))
            {
                return;
            }

            var constructorContext = CollectConstructorContext(substituteContext, proxyType);
            AnalyzeConstructor(substituteContext, constructorContext);
        }

        private void AnalyzeSubstituteForPartsOf(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            var proxyType = substituteContext.MethodSymbol.TypeArguments.FirstOrDefault();

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

            var constructorContext = CollectConstructorContext(substituteContext, proxyType);
            AnalyzeConstructor(substituteContext, constructorContext);
        }

        private void AnalyzeConstructor(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
        {
            if (AnalyzeConstructorAccessability(substituteContext, constructorContext))
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
            var proxies = GetProxySymbols(substituteContext).ToList();
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

        private ITypeSymbol GetActualProxyTypeSymbol(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            var proxies = GetProxySymbols(substituteContext).ToList();

            var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

            return classSymbol ?? proxies.FirstOrDefault();
        }

        private ImmutableArray<ITypeSymbol> GetProxySymbols(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            if (substituteContext.MethodSymbol.IsGenericMethod)
            {
                return substituteContext.MethodSymbol.TypeArguments;
            }

            var arrayParameters = GetArrayInitializerArguments(substituteContext.InvocationExpression)?.ToList();

            if (arrayParameters == null)
            {
                return ImmutableArray<ITypeSymbol>.Empty;
            }

            var proxyTypes = GetTypeOfLikeExpressions(arrayParameters)
                .Select(exp =>
                    substituteContext.SyntaxNodeAnalysisContext.SemanticModel
                        .GetTypeInfo(exp.DescendantNodes().First()))
                .Where(model => model.Type != null)
                .Select(model => model.Type)
                .ToImmutableArray();

            return arrayParameters.Count == proxyTypes.Length ? proxyTypes : ImmutableArray<ITypeSymbol>.Empty;
        }

        private bool AnalyzeConstructorParametersCount(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
        {
            var invocationArgumentTypes = constructorContext.InvocationParameters?.Count;
            switch (constructorContext.ConstructorType.TypeKind)
            {
                case TypeKind.Interface when invocationArgumentTypes > 0:
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForInterface,
                        substituteContext.InvocationExpression.GetLocation());

                    substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                    return true;
                case TypeKind.Interface:
                    return false;
                case TypeKind.Delegate when invocationArgumentTypes > 0:
                    var delegateDiagnostic = Diagnostic.Create(
                        DiagnosticDescriptorsProvider.SubstituteConstructorArgumentsForDelegate,
                        substituteContext.InvocationExpression.GetLocation());

                    substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(delegateDiagnostic);
                    return true;
                case TypeKind.Delegate:
                    return false;
            }

            if (constructorContext.PossibleConstructors != null && constructorContext.PossibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteForConstructorParametersMismatch,
                    substituteContext.InvocationExpression.GetLocation());

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
                    DiagnosticDescriptorsProvider.SubstituteForPartsOfUsedForInterface,
                    substituteContext.InvocationExpression.GetLocation());

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
                    MatchesInvocation(
                        substituteContext.SyntaxNodeAnalysisContext.SemanticModel.Compilation, ctor, constructorContext.InvocationParameters) ==
                    false))
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteConstructorMismatch,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeConstructorAccessability(SubstituteContext<TInvocationExpressionSyntax> substituteContext, ConstructorContext constructorContext)
        {
            if (constructorContext.ConstructorType.TypeKind == TypeKind.Class && constructorContext.AccessibleConstructors != null && constructorContext.AccessibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.SubstituteForWithoutAccessibleConstructor,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private static bool IsSubstituteMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax, string memberName)
        {
            if (MethodNames.Contains(memberName) == false)
            {
                return false;
            }

            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

            return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == true &&
                   symbol.Symbol?.ContainingType?.ToString().Equals(MetadataNames.NSubstituteSubstituteFullTypeName, StringComparison.Ordinal) == true;
        }
    }
}