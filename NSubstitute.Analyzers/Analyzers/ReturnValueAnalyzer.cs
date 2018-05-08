using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static NSubstitute.Analyzers.Analyzers.SubstituteSymbolAnalysis;

namespace NSubstitute.Analyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnValueAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptors.DoNotCreateSubstituteForNonVirtualMembers);

        private static readonly HashSet<string> MethodNames = new HashSet<string>
        {
            MetadataNames.NSubstituteReturnsMethod,
            MetadataNames.NSubstituteReturnsForAnyArgsMethod
        };

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var substituteExtensionsType =
                    compilationContext.Compilation.GetTypeByMetadataName(MetadataNames.NSubstituteSubstituteExtensions);
                if (substituteExtensionsType == null)
                {
                    return;
                }

                compilationContext.RegisterSyntaxNodeAction(syntaxContext =>
                {
                    var invocation = (InvocationExpressionSyntax) syntaxContext.Node;

                    var symbolInfo =
                        syntaxContext.SemanticModel.GetSymbolInfo(invocation, syntaxContext.CancellationToken);
                    if (symbolInfo.Symbol?.Kind != SymbolKind.Method)
                        return;

                    var methodSymbol = (IMethodSymbol) symbolInfo.Symbol;
                    if (methodSymbol.ContainingType != substituteExtensionsType ||
                        !MethodNames.Contains(methodSymbol.Name))
                    {
                        return;
                    }

                    switch (methodSymbol.MethodKind)
                    {
                        case MethodKind.ReducedExtension:
                            AnalyzeReducedExtensionMethod(invocation, syntaxContext, substituteExtensionsType);
                            break;
                        case MethodKind.Ordinary:
                        case MethodKind.Constructor:
                        case MethodKind.StaticConstructor:
                        case MethodKind.LocalFunction:
                            AnalyzeOrdinaryMethod(invocation, syntaxContext);
                            break;
                    }
                }, SyntaxKind.InvocationExpression);
            });
        }

        private static void AnalyzeOrdinaryMethod(InvocationExpressionSyntax invocation, SyntaxNodeAnalysisContext syntaxContext)
        {
            var argumentSyntax = invocation.ArgumentList.Arguments.First().ChildNodes().First();
            var symbol = syntaxContext.SemanticModel.GetSymbolInfo(argumentSyntax);
            if (symbol.Symbol.IsVirtual == false && symbol.Symbol.IsAbstract == false && IsPartOfInterfaceImplementation(symbol.Symbol) == false)
            {
                var location = invocation.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First()
                    .DescendantNodes().OfType<SimpleNameSyntax>().Last();
                syntaxContext.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DoNotCreateSubstituteForNonVirtualMembers, location.GetLocation()));
            }
        }

        private static void AnalyzeReducedExtensionMethod(InvocationExpressionSyntax invocation,
            SyntaxNodeAnalysisContext syntaxContext, INamedTypeSymbol assertType)
        {
            var identifierNameSyntaxs = invocation.DescendantNodes().OfType<SimpleNameSyntax>().ToList();

            foreach (var nameSyntax in identifierNameSyntaxs.Where(identifier => MethodNames.Contains(identifier.Identifier.ValueText)))
            {
                var info = syntaxContext.SemanticModel.GetSymbolInfo(nameSyntax);
                if (nameSyntax.Parent != null && info.Symbol != null && info.Symbol.Kind == SymbolKind.Method &&
                    info.Symbol.ContainingType == assertType)
                {
                    var syntaxTokens = nameSyntax.Parent.ChildNodes().ToList();
                    var returnsChild = syntaxTokens.IndexOf(nameSyntax);
                    var symbol = syntaxContext.SemanticModel.GetSymbolInfo(syntaxTokens[returnsChild - 1]);
                    if (symbol.Symbol.IsVirtual == false && symbol.Symbol.IsAbstract == false && IsPartOfInterfaceImplementation(symbol.Symbol) == false)
                    {
                        syntaxContext.ReportDiagnostic(Diagnostic.Create(
                            DiagnosticDescriptors.DoNotCreateSubstituteForNonVirtualMembers, nameSyntax.GetLocation()));
                    }
                }
            }
        }
    }
}