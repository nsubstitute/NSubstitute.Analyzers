using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Semantics;

namespace NSubstitute.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnForNonVirtualMethodAnalyzer : DiagnosticAnalyzer
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

        private readonly string[] _methodNames = {"Returns"};

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
                    if (methodSymbol.ContainingType != assertType || !_methodNames.Contains(methodSymbol.Name))
                    {
                        return;
                    }

                    var identifierNameSyntaxs = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();

                    foreach (var nameSyntax in identifierNameSyntaxs.Where(identifier => identifier.ToString().Equals("Returns", StringComparison.OrdinalIgnoreCase)))
                    {
                        var info = syntaxContext.SemanticModel.GetSymbolInfo(nameSyntax);
                        if (nameSyntax.Parent != null && info.Symbol != null && info.Symbol.Kind == SymbolKind.Method && info.Symbol.ContainingType == assertType)
                        {
                            var index = identifierNameSyntaxs.IndexOf(nameSyntax);
                            var predecessorIndex = index - 1;
                            if (predecessorIndex > -1 && identifierNameSyntaxs.Count - 1 >= predecessorIndex)
                            {
                                var identifierNameSyntax = identifierNameSyntaxs[predecessorIndex];
                                var predecessorSymbol = syntaxContext.SemanticModel.GetSymbolInfo(identifierNameSyntax);
                                if (predecessorSymbol.Symbol.IsVirtual == false && predecessorSymbol.Symbol.IsAbstract == false)
                                {
                                    syntaxContext.ReportDiagnostic(Diagnostic.Create(Rule, nameSyntax.GetLocation()));
                                }
                            }
                        }
                    }

                }, SyntaxKind.InvocationExpression);
            });
        }
    }
}