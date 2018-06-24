﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Newtonsoft.Json;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractSuppressDiagnosticsCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonVirtualSetupSpecification);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var project = context.Document.Project;
            var workspace = project.Solution.Workspace;

            // check if we are allowed to add it
            if (!workspace.CanApplyChange(ApplyChangesKind.AddAdditionalDocument))
            {
                return;
            }

            var settingsFile = GetSettingsFile(project);

            // creating additional document from Roslyn is broken (https://github.com/dotnet/roslyn/issues/4655) the nsubstitute.json file have to be created by users manually
            // if there is no settings file do not provide refactorings
            if (settingsFile == null)
            {
                return;
            }

            var root = await context.Document.GetSyntaxRootAsync();
            var model = await context.Document.GetSemanticModelAsync();
            var i = 1;
            foreach (var diagnostic in context.Diagnostics.Where(diagnostic => FixableDiagnosticIds.Contains(diagnostic.Id)))
            {
                var syntaxNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
                var symbolInfo = model.GetSymbolInfo(syntaxNode);

                foreach (var innerSymbol in GetSuppressibleSymbol(symbolInfo.Symbol))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            CreateCodeFixTitle(diagnostic, innerSymbol),
                            cancellationToken => GetTransformedSolutionAsync(context, diagnostic, settingsFile, innerSymbol),
                            (i++).ToString()),
                        diagnostic);
                }
            }
        }

        private static string CreateCodeFixTitle(Diagnostic diagnostic, ISymbol innerSymbol)
        {
            var prefix = GetSymbolTitlePrefix(innerSymbol);
            return $"Suppress {diagnostic.Id} for {prefix} {innerSymbol.Name} in {AnalyzersSettings.AnalyzerFileName}";
        }

        private static string GetSymbolTitlePrefix(ISymbol innerSymbol)
        {
            switch (innerSymbol)
            {
                    case IMethodSymbol methodSymbol:
                        return "method";
                    case IPropertySymbol propertySymbol when propertySymbol.IsIndexer:
                        return "indexer";
                    case IPropertySymbol propertySymbol:
                        return "property";
                    case ITypeSymbol typeSymbol:
                        return "class";
                    case INamespaceSymbol namespaceSymbol:
                        return "namespace";
                    default:
                        return string.Empty;
            }
        }

        private Task<Solution> GetTransformedSolutionAsync(CodeFixContext context, Diagnostic diagnostic, TextDocument settingsFile, ISymbol symbol)
        {
            var project = context.Document.Project;
            var solution = project.Solution;

            var options = GetUpdatedAnalyzersOptions(context, diagnostic, symbol);

            project = project.RemoveAdditionalDocument(settingsFile.Id);
            solution = project.Solution;

            var newDocumentId = settingsFile.Id ?? DocumentId.CreateNewId(project.Id);

            solution = solution.AddAdditionalDocument(
                newDocumentId,
                AnalyzersSettings.AnalyzerFileName,
                JsonConvert.SerializeObject(options, Formatting.Indented));

            return Task.FromResult(solution);
        }

        private static AnalyzersSettings GetUpdatedAnalyzersOptions(CodeFixContext context, Diagnostic diagnostic, ISymbol symbol)
        {
            var options = context.Document.Project.AnalyzerOptions.GetSettings(default(CancellationToken));
            var target = DocumentationCommentId.CreateDeclarationId(symbol);
            options.Suppressions = options.Suppressions ?? new List<Suppression>();

            var existingSuppression = options.Suppressions.FirstOrDefault(suppression => suppression.Target == target);

            if (existingSuppression != null)
            {
                existingSuppression.Rules = existingSuppression.Rules ?? new List<string>();
                existingSuppression.Rules.Add(diagnostic.Id);
            }
            else
            {
                options.Suppressions.Add(new Suppression
                {
                    Target = DocumentationCommentId.CreateDeclarationId(symbol),
                    Rules = new List<string>
                    {
                        diagnostic.Id
                    }
                });
            }

            return options;
        }

        private static TextDocument GetSettingsFile(Project project)
        {
            return project.AdditionalDocuments.SingleOrDefault(document =>
                document.Name.Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase));
        }

        private IEnumerable<ISymbol> GetSuppressibleSymbol(ISymbol symbol)
        {
            if (symbol == null)
            {
                yield break;
            }

            yield return symbol;

            if (!(symbol is ITypeSymbol))
            {
                yield return symbol.ContainingType;
                yield return symbol.ContainingType.ContainingNamespace;
            }

            if (symbol is ITypeSymbol)
            {
                yield return symbol.ContainingNamespace;
            }
        }
    }
}