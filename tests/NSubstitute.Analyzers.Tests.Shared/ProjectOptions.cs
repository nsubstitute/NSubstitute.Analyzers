using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Tests.Shared
{
    public abstract class ProjectOptions
    {
        public CompilationOptions CompilationOptions { get; }

        public ImmutableArray<MetadataReference> MetadataReferences { get; }

        protected ProjectOptions(ImmutableArray<MetadataReference> metadataReferences, CompilationOptions compilationOptions)
        {
            MetadataReferences = metadataReferences;
            CompilationOptions = compilationOptions;
        }
    }
}