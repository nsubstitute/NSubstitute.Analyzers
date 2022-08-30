using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp;

public class CSharpProjectOptions : ProjectOptions
{
    public static CSharpProjectOptions Default { get; } = new(
        RuntimeMetadataReference.Default,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private CSharpProjectOptions(
        ImmutableArray<MetadataReference> metadataReferences,
        CompilationOptions compilationOptions)
        : base(metadataReferences, compilationOptions)
    {
    }

    public CSharpProjectOptions WithWarningLevel(int warningLevel)
    {
        var compilationOptions = (CSharpCompilationOptions)CompilationOptions;
        return new CSharpProjectOptions(
            MetadataReferences,
            new CSharpCompilationOptions(compilationOptions.OutputKind, warningLevel: warningLevel));
    }

    public CSharpProjectOptions WithAdditionalMetadataReferences(MetadataReference[] metadataReferences)
    {
        return new CSharpProjectOptions(MetadataReferences.AddRange(metadataReferences), CompilationOptions);
    }
}