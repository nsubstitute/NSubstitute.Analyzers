using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Tests.Shared;

public abstract class WorkspaceFactory
{
    public static string DefaultDocumentName { get; } = "Test";

    protected internal abstract string DocumentExtension { get; }

    protected abstract string Language { get; }

    public const string DefaultProjectName = "TestProject";

    protected abstract ProjectOptions ProjectOptions { get; }

    public Project AddProject(Solution solution)
    {
        return AddProject(solution, ProjectOptions);
    }

    public Project AddProject(Solution solution, ProjectOptions projectOptions)
    {
        return solution.AddProject(DefaultProjectName, DefaultProjectName, Language)
            .WithMetadataReferences(projectOptions.MetadataReferences)
            .WithCompilationOptions(projectOptions.CompilationOptions);
    }

    public Project AddProject(Solution solution, string source, string? analyzerSettings)
    {
        return AddProject(solution, new[] { source }, analyzerSettings);
    }

    public Project AddProject(Solution solution, string[] sources, string? analyzerSettings)
    {
        var project = AddProject(solution);
        project = AddDocuments(project, sources)?.Project ?? project;
        if (analyzerSettings != null)
        {
            project = AddAnalyzerSettings(project, analyzerSettings).Project;
        }

        return project;
    }

    public Document AddDocument(Project project, string source)
    {
        var documentName = CreateDocumentName(project.Documents.Count());

        var document = project.AddDocument(documentName, SourceText.From(source));

        return document;
    }

    public Document? AddDocuments(Project project, string[] sources)
    {
        Document? document = null;

        foreach (var source in sources)
        {
            document = AddDocument(project, source);
            project = document.Project;
        }

        return document;
    }

    public TextDocument AddAnalyzerSettings(Project project, string? settings)
    {
        return project.AddAdditionalDocument(AnalyzersSettings.AnalyzerFileName, SourceText.From(settings));
    }

    private string CreateDocumentName(int number)
    {
        return $"{DefaultDocumentName}{number}.{DocumentExtension}";
    }
}