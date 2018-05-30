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

            if (AnalyzeConstructorAccessability(substituteContext, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorParametersCount(substituteContext, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorInvocation(substituteContext, proxyType))
            {
                return;
            }
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

            if (AnalyzeConstructorAccessability(substituteContext, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorParametersCount(substituteContext, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorInvocation(substituteContext, proxyType))
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

        private bool AnalyzeConstructorInvocation(SubstituteContext substituteContext, ITypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind != TypeKind.Class)
            {
                return false;
            }

            var possibleConstructors = GetPossibleConstructors(substituteContext, typeSymbol);
            var invocationInfo = GetInvocationInfo(substituteContext);

            if (invocationInfo != null && possibleConstructors.All(ctor =>
                    SubstituteConstructorMatcher.MatchesInvocation(
                        substituteContext.SyntaxNodeAnalysisContext.SemanticModel.Compilation, ctor, invocationInfo) ==
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

        private bool AnalyzeConstructorParametersCount(SubstituteContext substituteContext, ITypeSymbol typeSymbol)
        {
            var invocationArgumentTypes = GetInvocationInfo(substituteContext)?.Count;
            switch (typeSymbol.TypeKind)
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

            var possibleConstructors = GetPossibleConstructors(substituteContext, typeSymbol);

            if (possibleConstructors != null && possibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForConstructorParametersMismatch,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private IEnumerable<IMethodSymbol> GetPossibleConstructors(SubstituteContext substituteContext, ITypeSymbol typeSymbol)
        {
            var count = GetInvocationInfo(substituteContext)?.Count;

            return count.HasValue
                ? GetAccessibleConstructors(typeSymbol).Where(ctor => ctor.Parameters.Length == count)
                : null;
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

        private bool AnalyzeConstructorAccessability(SubstituteContext substituteContext, ITypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind != TypeKind.Class)
            {
                return false;
            }

            var accessibleConstructors = GetAccessibleConstructors(typeSymbol);
            if (accessibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForWithoutAccessibleConstructor,
                    substituteContext.InvocationExpression.GetLocation());

                substituteContext.SyntaxNodeAnalysisContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool InternalsVisibleToProxyGenerator(ISymbol typeSymbol)
        {
            var internalsVisibleToAttribute = typeSymbol.ContainingAssembly.GetAttributes()
                .FirstOrDefault(att =>
                    att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName);

            return internalsVisibleToAttribute != null &&
                   internalsVisibleToAttribute.ConstructorArguments.Any(arg =>
                       arg.Value.ToString() == MetadataNames.CastleDynamicProxyGenAssembly2Name);
        }

        private IEnumerable<IMethodSymbol> GetAccessibleConstructors(ITypeSymbol genericArgument)
        {
            var internalsVisibleToProxy = InternalsVisibleToProxyGenerator(genericArgument);

            return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
                symbol.MethodKind == MethodKind.Constructor &&
                symbol.IsStatic == false &&
                (symbol.DeclaredAccessibility == Accessibility.Protected ||
                 symbol.DeclaredAccessibility == Accessibility.Public ||
                 (internalsVisibleToProxy && symbol.DeclaredAccessibility == Accessibility.Internal)));
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

        public readonly struct SubstituteContext
        {
            public SyntaxNodeAnalysisContext SyntaxNodeAnalysisContext { get; }

            public InvocationExpressionSyntax InvocationExpression { get; }

            public IMethodSymbol MethodSymbol { get; }

            public SubstituteContext(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
            {
                SyntaxNodeAnalysisContext = syntaxNodeAnalysisContext;
                InvocationExpression = invocationExpression;
                MethodSymbol = methodSymbol;
            }
        }
    }
}