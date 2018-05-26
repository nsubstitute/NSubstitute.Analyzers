using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
#if VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
#endif

namespace NSubstitute.Analyzers
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

            if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForMethod, StringComparison.Ordinal))
            {
                AnalyzeSubstituteForMethod(syntaxNodeContext, invocationExpression, methodSymbol);
                return;
            }

            if (methodSymbol.Name.Equals(MetadataNames.NSubstituteForPartsOfMethod, StringComparison.Ordinal))
            {
                AnalyzeSubstituteForPartsOf(syntaxNodeContext, invocationExpression, methodSymbol);
                return;
            }
        }

        private void AnalyzeSubstituteForMethod(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            if (AnalyzeProxies(syntaxNodeContext, invocationExpression, methodSymbol))
            {
                return;
            }

            var proxyType = GetActualProxyTypeSymbol(syntaxNodeContext, invocationExpression, methodSymbol);

            if (AnalyzeTypeAccessability(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorAccessability(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorParametersCount(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorInvocation(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }
        }

        private void AnalyzeSubstituteForPartsOf(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            var proxyType = methodSymbol.TypeArguments.FirstOrDefault();

            if (proxyType == null)
            {
                return;
            }

            if (AnalyzeTypeKind(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeTypeAccessability(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (proxyType.TypeKind != TypeKind.Class)
            {
                return;
            }

            if (AnalyzeConstructorAccessability(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorParametersCount(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }

            if (AnalyzeConstructorInvocation(syntaxNodeContext, invocationExpression, proxyType))
            {
                return;
            }
        }

        private bool AnalyzeProxies(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            var proxies = GetProxySymbols(syntaxNodeContext, invocationExpression, methodSymbol).ToList();
            var classProxies = proxies.Where(proxy => proxy.TypeKind == TypeKind.Class).Distinct();
            if (classProxies.Count() > 1)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteMultipleClasses,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private ITypeSymbol GetActualProxyTypeSymbol(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            var proxies = GetProxySymbols(syntaxNodeContext, invocationExpression, methodSymbol).ToList();

            var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

            return classSymbol ?? proxies.FirstOrDefault();
        }

        private ImmutableArray<ITypeSymbol> GetProxySymbols(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            return methodSymbol.TypeArguments;
        }

        private bool AnalyzeConstructorInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, ITypeSymbol typeSymbol)
        {
            if (typeSymbol.TypeKind != TypeKind.Class)
            {
                return false;
            }

            var possibleConstructors = GetPossibleConstructors(typeSymbol, invocationExpression);
            var argumentTypes = invocationExpression.ArgumentList.Arguments
                .Select(arg => syntaxNodeContext.SemanticModel.GetTypeInfo(arg.DescendantNodes().First()).Type)
                .Where(type => type != null)
                .ToList();

            if (argumentTypes.Count != invocationExpression.ArgumentList.Arguments.Count)
            {
                return false;
            }

            if (possibleConstructors.All(ctor => SubstituteConstructorMatcher.MatchesInvocation(syntaxNodeContext.Compilation, ctor, argumentTypes) == false))
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteConstructorMismatch,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeConstructorParametersCount(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Interface when GetInvocationArgumentCount(invocationExpression) > 0:
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteConstructorArgumentsForInterface,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return true;
                case TypeKind.Interface:
                    return false;
                case TypeKind.Delegate when GetInvocationArgumentCount(invocationExpression) > 0:
                    var delegateDiagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteConstructorArgumentsForDelegate,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(delegateDiagnostic);
                    return true;
                case TypeKind.Delegate:
                    return false;
            }

            var possibleConstructors = GetPossibleConstructors(typeSymbol, invocationExpression);

            if (possibleConstructors.Any() == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForConstructorParametersMismatch,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private IEnumerable<IMethodSymbol> GetPossibleConstructors(ITypeSymbol typeSymbol, InvocationExpressionSyntax invocationExpression)
        {
            var count = GetInvocationArgumentCount(invocationExpression);
            return GetAccessibleConstructors(typeSymbol).Where(ctor => ctor.Parameters.Length == count);
        }

        private int GetInvocationArgumentCount(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Count;
        }

        private static bool AnalyzeTypeKind(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, ITypeSymbol proxyType)
        {
            if (proxyType.TypeKind == TypeKind.Interface || proxyType.TypeKind == TypeKind.Delegate)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForPartsOfUsedForInterface,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeTypeAccessability(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, ITypeSymbol proxyType)
        {
            if (proxyType.DeclaredAccessibility == Accessibility.Internal && InternalsVisibleToProxyGenerator(proxyType) == false)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForInternalMember,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool AnalyzeConstructorAccessability(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax invocationExpression, ITypeSymbol typeSymbol)
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
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return true;
            }

            return false;
        }

        private bool InternalsVisibleToProxyGenerator(ISymbol typeSymbol)
        {
            var internalsVisibleToAttribute = typeSymbol.ContainingAssembly.GetAttributes()
                .FirstOrDefault(att => att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName);

            return internalsVisibleToAttribute != null &&
                   internalsVisibleToAttribute.ConstructorArguments.Any(arg =>
                       arg.Value.ToString() == MetadataNames.CastleDynamicProxyGenAssembly2Name);
        }

        private static IList<ITypeSymbol> GetProxyTypes(InvocationExpressionSyntax invocationExpression, IMethodSymbol methodSymbol)
        {
            // TODO try handle non-generic Substitute.For
            return methodSymbol.TypeArguments;
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
    }
}