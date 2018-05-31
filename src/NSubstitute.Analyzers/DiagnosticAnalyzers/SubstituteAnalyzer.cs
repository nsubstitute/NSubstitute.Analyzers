using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Extensions;
using static NSubstitute.Analyzers.DiagnosticAnalyzers.SubstituteAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#elif VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers.DiagnosticAnalyzers
{
#if CSHARP
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
#endif
    public class SubstituteAnalyzer : DiagnosticAnalyzer
    {
        private static readonly ImmutableHashSet<string> MethodNames = ImmutableHashSet.Create(
            MetadataNames.NSubstituteForMethod,
            MetadataNames.NSubstituteForPartsOfMethod);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            DiagnosticDescriptors.SubstituteForPartsOfUsedForInterface,
            DiagnosticDescriptors.SubstituteForWithoutAccessibleConstructor,
            DiagnosticDescriptors.SubstituteForConstructorParametersMismatch,
            DiagnosticDescriptors.SubstituteForInternalMember,
            DiagnosticDescriptors.SubstituteConstructorMismatch,
            DiagnosticDescriptors.SubstituteMultipleClasses,
            DiagnosticDescriptors.SubstituteConstructorArgumentsForInterface,
            DiagnosticDescriptors.SubstituteConstructorArgumentsForDelegate);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = (InvocationExpressionSyntax)syntaxNodeContext.Node;
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

            var substituteContext = new SubstituteContext(syntaxNodeContext, invocationExpression, methodSymbol);

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

        private void AnalyzeSubstituteForMethod(SubstituteContext substituteContext)
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

        private void AnalyzeSubstituteForPartsOf(SubstituteContext substituteContext)
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

        private void AnalyzeConstructor(SubstituteContext substituteContext, ConstructorContext constructorContext)
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

        private bool AnalyzeProxies(SubstituteContext substituteContext)
        {
            var proxies = GetProxySymbols(substituteContext).ToList();
            var classProxies = proxies.Where(proxy => proxy.TypeKind == TypeKind.Class).Distinct();
            if (classProxies.Count() > 1)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteMultipleClasses,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private ITypeSymbol GetActualProxyTypeSymbol(SubstituteContext substituteContext)
        {
            var proxies = GetProxySymbols(substituteContext).ToList();

            var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

            return classSymbol ?? proxies.FirstOrDefault();
        }

        private ImmutableArray<ITypeSymbol> GetProxySymbols(SubstituteContext substituteContext)
        {
            if (substituteContext.MethodSymbol.IsGenericMethod)
            {
                return substituteContext.MethodSymbol.TypeArguments;
            }

            var arrayParameters = substituteContext.InvocationExpression.ArgumentList.Arguments.First()
                .GetArgumentExpression()
                .GetParameterExpressionsFromArrayArgument();

            if (arrayParameters == null)
            {
                return ImmutableArray<ITypeSymbol>.Empty;
            }

            var proxyTypes = arrayParameters.OfType<TypeOfExpressionSyntax>()
                .Select(exp =>
                    substituteContext.SyntaxNodeAnalysisContext.SemanticModel
                        .GetTypeInfo(exp.DescendantNodes().First()))
                .Where(model => model.Type != null)
                .Select(model => model.Type)
                .ToImmutableArray();

            return arrayParameters.Count == proxyTypes.Length ? proxyTypes : ImmutableArray<ITypeSymbol>.Empty;
        }

        private bool AnalyzeConstructorParametersCount(SubstituteContext substituteContext, ConstructorContext constructorContext)
        {
            var invocationArgumentTypes = constructorContext.InvocationParameters?.Count;
            switch (constructorContext.ConstructorType.TypeKind)
            {
                case TypeKind.Interface when invocationArgumentTypes > 0:
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteConstructorArgumentsForInterface,
                        substituteContext.InvocationExpression.GetLocation());

                    substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                    return true;
                case TypeKind.Interface:
                    return false;
                case TypeKind.Delegate when invocationArgumentTypes > 0:
                    var delegateDiagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteConstructorArgumentsForDelegate,
                        substituteContext.InvocationExpression.GetLocation());

                    substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(delegateDiagnostic);
                    return true;
                case TypeKind.Delegate:
                    return false;
            }

            if (constructorContext.PossibleConstructors != null && constructorContext.PossibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForConstructorParametersMismatch,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private static bool AnalyzeTypeKind(SubstituteContext substituteContext, ITypeSymbol proxyType)
        {
            if (proxyType.TypeKind == TypeKind.Interface || proxyType.TypeKind == TypeKind.Delegate)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForPartsOfUsedForInterface,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeTypeAccessability(SubstituteContext substituteContext, ITypeSymbol proxyType)
        {
            if (proxyType.DeclaredAccessibility == Accessibility.Internal &&
                InternalsVisibleToProxyGenerator(proxyType) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForInternalMember,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
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
                    SubstituteConstructorMatcher.MatchesInvocation(
                        substituteContext.SyntaxNodeAnalysisContext.SemanticModel.Compilation, ctor, constructorContext.InvocationParameters) ==
                    false))
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteConstructorMismatch,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeConstructorAccessability(SubstituteContext substituteContext, ConstructorContext constructorContext)
        {
            if (constructorContext.ConstructorType.TypeKind == TypeKind.Class && constructorContext.AccessibleConstructors != null && constructorContext.AccessibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForWithoutAccessibleConstructor,
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