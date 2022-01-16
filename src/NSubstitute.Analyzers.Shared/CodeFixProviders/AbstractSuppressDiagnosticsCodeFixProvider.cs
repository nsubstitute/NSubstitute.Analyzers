using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractSuppressDiagnosticsCodeFixProvider : CodeFixProvider
{
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
        foreach (var diagnostic in context.Diagnostics
                     .Where(diagnostic => FixableDiagnosticIds.Contains(diagnostic.Id)))
        {
            var syntaxNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var symbolInfo = model.GetSymbolInfo(syntaxNode);

            foreach (var innerSymbol in GetSuppressibleSymbol(model, syntaxNode, symbolInfo.Symbol))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        CreateCodeFixTitle(diagnostic, innerSymbol),
                        cancellationToken => GetTransformedSolutionAsync(context, diagnostic, settingsFile, innerSymbol)),
                    diagnostic);
            }
        }
    }

    protected virtual IEnumerable<ISymbol> GetSuppressibleSymbol(SemanticModel model, SyntaxNode syntaxNode, ISymbol symbol)
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

    private static string CreateCodeFixTitle(Diagnostic diagnostic, ISymbol innerSymbol)
    {
        var prefix = GetSymbolTitlePrefix(innerSymbol);
        return $"Suppress {diagnostic.Id} for {prefix} {innerSymbol.Name} in {AnalyzersSettings.AnalyzerFileName}";
    }

    private static string GetSymbolTitlePrefix(ISymbol innerSymbol)
    {
        switch (innerSymbol)
        {
            case IMethodSymbol _:
                return "method";
            case IPropertySymbol propertySymbol when propertySymbol.IsIndexer:
                return "indexer";
            case IPropertySymbol _:
                return "property";
            case ITypeSymbol _:
                return "class";
            case INamespaceSymbol _:
                return "namespace";
            default:
                return string.Empty;
        }
    }

    private Task<Solution> GetTransformedSolutionAsync(CodeFixContext context, Diagnostic diagnostic, TextDocument settingsFile, ISymbol symbol)
    {
        var project = context.Document.Project;
        var settingsFileId = settingsFile?.Id;
        if (settingsFileId != null)
        {
            project = project.RemoveAdditionalDocument(settingsFileId);
        }
        else
        {
            settingsFileId = DocumentId.CreateNewId(project.Id);
        }

        var options = GetUpdatedAnalyzersOptions(context, diagnostic, symbol);

        var solution = project.Solution;

        solution = solution.AddAdditionalDocument(
            settingsFileId,
            AnalyzersSettings.AnalyzerFileName,
            Json.Encode(options, pretty: true));

        return Task.FromResult(solution);
    }

    private static AnalyzersSettings GetUpdatedAnalyzersOptions(CodeFixContext context, Diagnostic diagnostic, ISymbol symbol)
    {
        var options = context.Document.Project.AnalyzerOptions.GetSettings(default(CancellationToken));
        var target = CreateSuppressionTarget(symbol);
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
                Target = target,
                Rules = new List<string>
                {
                    diagnostic.Id
                }
            });
        }

        return options;
    }

    private static string CreateSuppressionTarget(ISymbol symbol)
    {
        var actualSymbol = symbol;
        if (actualSymbol is IMethodSymbol methodSymbol && methodSymbol.ReducedFrom != null)
        {
            actualSymbol = methodSymbol.ReducedFrom;
        }

        return DocumentationCommentId.CreateDeclarationId(actualSymbol);
    }

    private static TextDocument GetSettingsFile(Project project)
    {
        return project.AdditionalDocuments.SingleOrDefault(document =>
            document.Name.Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase));
    }
}