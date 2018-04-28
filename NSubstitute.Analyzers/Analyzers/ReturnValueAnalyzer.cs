using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Extensions;

namespace NSubstitute.Analyzers.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnValueAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "NSubstituteAnalyzers";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private static readonly HashSet<string> MethodNames = new HashSet<string>
        {
            "Returns",
            "ReturnsForAnyArgs"
        };

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var assertType = compilationContext.Compilation.GetTypeByMetadataName("NSubstitute.SubstituteExtensions");
                if (assertType == null)
                    return;

                compilationContext.RegisterSyntaxNodeAction(syntaxContext =>
                {
                    var invocation = (InvocationExpressionSyntax) syntaxContext.Node;

                    var symbolInfo = syntaxContext.SemanticModel.GetSymbolInfo(invocation, syntaxContext.CancellationToken);
                    if (symbolInfo.Symbol?.Kind != SymbolKind.Method)
                        return;

                    var methodSymbol = (IMethodSymbol) symbolInfo.Symbol;
                    if (methodSymbol.ContainingType != assertType || !MethodNames.Contains(methodSymbol.Name))
                    {
                        return;
                    }

                    if (methodSymbol.MethodKind == MethodKind.ReducedExtension)
                    {
                        var identifierNameSyntaxs = invocation.DescendantNodes().OfType<SimpleNameSyntax>().ToList();

                        foreach (var nameSyntax in identifierNameSyntaxs.Where(identifier => MethodNames.Contains(identifier.Identifier.ValueText)))
                        {
                            var info = syntaxContext.SemanticModel.GetSymbolInfo(nameSyntax);
                            if (nameSyntax.Parent != null && info.Symbol != null && info.Symbol.Kind == SymbolKind.Method && info.Symbol.ContainingType == assertType)
                            {
                                var syntaxTokens = nameSyntax.Parent.ChildNodes().ToList();
                                var returnsChild = syntaxTokens.IndexOf(nameSyntax);
                                var symbol = syntaxContext.SemanticModel.GetSymbolInfo(syntaxTokens[returnsChild - 1]);
                                if (symbol.Symbol.IsVirtual == false && symbol.Symbol.IsAbstract == false && symbol.Symbol.IsInterfaceImplementation() == false)
                                {
                                    syntaxContext.ReportDiagnostic(Diagnostic.Create(Rule, nameSyntax.GetLocation()));
                                }
                            }
                        }
                    }
                    else if(methodSymbol.MethodKind == MethodKind.Ordinary)
                    {
                        var argumentSyntax = invocation.ArgumentList.Arguments.First().ChildNodes().First();
                        var symbol = syntaxContext.SemanticModel.GetSymbolInfo(argumentSyntax);
                        if (symbol.Symbol.IsVirtual == false && symbol.Symbol.IsAbstract == false && symbol.Symbol.IsInterfaceImplementation() == false)
                        {
                            var methodSymbol1 = symbol.Symbol as IMethodSymbol;
                            var location = invocation.DescendantNodes().OfType<MemberAccessExpressionSyntax>().First().DescendantNodes().OfType<SimpleNameSyntax>().Last();
                            syntaxContext.ReportDiagnostic(Diagnostic.Create(Rule, location.GetLocation()));
                        }
                    }

                }, SyntaxKind.InvocationExpression);
            });
        }
    }
}