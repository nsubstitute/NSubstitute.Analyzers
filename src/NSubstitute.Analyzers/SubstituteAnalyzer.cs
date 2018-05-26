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
            DiagnosticDescriptors.SubstituteConstructorMismatch);

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

            var genericArgument = methodSymbol.TypeArguments.FirstOrDefault();

            if (genericArgument == null)
            {
                return;
            }

            if ((genericArgument.TypeKind == TypeKind.Interface || genericArgument.TypeKind == TypeKind.Delegate) &&
                methodSymbol.Name.Equals(MetadataNames.NSubstituteForPartsOfMethod, StringComparison.Ordinal))
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SubstituteForPartsOfUsedForInterface,
                    invocationExpression.GetLocation());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
                return;
            }

            bool isInternalyVisible = false;
            if (genericArgument.DeclaredAccessibility == Accessibility.Internal)
            {
                var internalsVisibleToAttribute = genericArgument.ContainingAssembly.GetAttributes().FirstOrDefault(att => att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName);
                if (internalsVisibleToAttribute == null || internalsVisibleToAttribute.ConstructorArguments.Any(
                    arg => arg.Value.ToString() == MetadataNames.CastleDynamicProxyGenAssembly2Name) == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteForInternalMember,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return;
                }

                isInternalyVisible = true;
            }

            if (genericArgument.TypeKind != TypeKind.Interface && genericArgument.TypeKind != TypeKind.Delegate)
            {
                var accessibleConstructors = GetAccessibleConstructors(genericArgument, isInternalyVisible);

                if (accessibleConstructors.Any() == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteForWithoutAccessibleConstructor,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return;
                }

                var argumentsCount = invocationExpression.ArgumentList.Arguments.Count;

                var possibleConstructors = accessibleConstructors.Where(ctor => ctor.Parameters.Length == argumentsCount).ToList();

                if (possibleConstructors.Any() == false)
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteForConstructorParametersMismatch,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                    return;
                }

                if (possibleConstructors.All(ctor => MatchesInvocation(syntaxNodeContext, ctor, invocationExpression.ArgumentList.Arguments) == false))
                {
                    var diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.SubstituteConstructorMismatch,
                        invocationExpression.GetLocation());

                    syntaxNodeContext.ReportDiagnostic(diagnostic);
                }
            }
        }

        private bool MatchesInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            for (int i = 0; i < methodSymbol.Parameters.Length; i++)
            {
                var typeInfo = syntaxNodeContext.SemanticModel.GetTypeInfo(arguments[i].DescendantNodes().First());
                if (typeInfo.Type != null)
                {
                    if (DetermineIsConvertible(syntaxNodeContext.Compilation, typeInfo.Type, methodSymbol.Parameters[i].Type) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static List<IMethodSymbol> GetAccessibleConstructors(ITypeSymbol genericArgument, bool isInternalyVisible)
        {
            return genericArgument.GetMembers().OfType<IMethodSymbol>().Where(symbol =>
                    symbol.MethodKind == MethodKind.Constructor &&
                    symbol.IsStatic == false &&
                    (symbol.DeclaredAccessibility == Accessibility.Protected ||
                     symbol.DeclaredAccessibility == Accessibility.Public ||
                     (isInternalyVisible && symbol.DeclaredAccessibility == Accessibility.Internal)))
                .ToList();
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

        private static bool DetermineIsConvertible(Compilation compilation, ITypeSymbol source, ITypeSymbol destination)
        {
                var conversion = compilation.ClassifyConversion(source, destination);

#if CSHARP
                return conversion.Exists && conversion.IsImplicit && conversion.IsReference;

#elif VISUAL_BASIC
            return conversion.Exists && conversion.IsReference;
#endif
        }
    }
}