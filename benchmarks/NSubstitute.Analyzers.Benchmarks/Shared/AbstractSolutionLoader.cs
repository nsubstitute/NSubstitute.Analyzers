using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Benchmarks.Shared;

public abstract class AbstractSolutionLoader
{
    protected abstract string DocumentFileExtension { get; }

    protected abstract string ProjectFileExtension { get; }

    protected abstract string Language { get; }

    private string _analyzerSettingsFileName = "nsubstitute.json";

    public Solution CreateSolution(string projectDirectory, MetadataReference[] metadataReferences)
    {
        var projectName = Path.GetFileName(projectDirectory);
        using (var adhocWorkspace = new AdhocWorkspace())
        {
            var projectId = ProjectId.CreateNewId();
            var solution = adhocWorkspace
                .CurrentSolution
                .AddProject(projectId, projectName, projectName, Language);

            foreach (var fileInfo in GetFiles(projectDirectory).Where(fileInfo => fileInfo.Extension != ProjectFileExtension))
            {
                if (fileInfo.Name == _analyzerSettingsFileName)
                {
                    solution = solution.AddAdditionalDocument(DocumentId.CreateNewId(projectId), fileInfo.Name, File.ReadAllText(fileInfo.FullName));
                }

                if (fileInfo.Extension == DocumentFileExtension)
                {
                    solution = solution.AddDocument(DocumentId.CreateNewId(projectId), fileInfo.Name, File.ReadAllText(fileInfo.FullName));
                }
            }

            return solution.AddMetadataReferences(projectId, metadataReferences)
                .WithProjectCompilationOptions(projectId, GetCompilationOptions(projectName));
        }
    }

    protected abstract CompilationOptions GetCompilationOptions(string rootNamespace);

    private static IEnumerable<FileInfo> GetFiles(string path)
    {
        var queue = new Queue<DirectoryInfo>();
        queue.Enqueue(new DirectoryInfo(path));
        while (queue.Count > 0)
        {
            var info = queue.Dequeue();
            foreach (var subDir in info.EnumerateDirectories().Where(dir => dir.Name.Equals("bin", StringComparison.OrdinalIgnoreCase) == false &&
                                                                            dir.Name.Equals("obj", StringComparison.OrdinalIgnoreCase) == false))
            {
                queue.Enqueue(subDir);
            }

            foreach (var fileInfo in info.EnumerateFiles())
            {
                yield return fileInfo;
            }
        }
    }
}