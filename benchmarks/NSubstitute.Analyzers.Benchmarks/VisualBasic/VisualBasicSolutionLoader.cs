using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Benchmarks.Shared;

namespace NSubstitute.Analyzers.Benchmarks.VisualBasic;

public sealed class VisualBasicSolutionLoader : AbstractSolutionLoader
{
    protected override string DocumentFileExtension { get; } = ".vb";

    protected override string ProjectFileExtension { get; } = ".vbproj";

    protected override string Language { get; } = LanguageNames.VisualBasic;

    protected override CompilationOptions GetCompilationOptions(string rootNamespace)
    {
        return new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, rootNamespace: rootNamespace, embedVbCoreRuntime: true);
    }
}