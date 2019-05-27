using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Benchmarks.Shared;

namespace NSubstitute.Analyzers.Benchmarks.CSharp
{
    public class SolutionLoader : AbstractSolutionLoader
    {
        protected override string DocumentFileExtension { get; } = ".cs";
        
        protected override string ProjectFileExtension { get; } = ".csproj";
        
        protected override string Language { get; } = LanguageNames.CSharp;
        
        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }
    }
}