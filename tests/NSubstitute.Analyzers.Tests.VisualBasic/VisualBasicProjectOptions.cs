using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic
{
    public class VisualBasicProjectOptions : ProjectOptions
    {
        public static VisualBasicProjectOptions Default { get; } = new VisualBasicProjectOptions(
            RuntimeMetadataReference.Default.Add(MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)),
            new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        private VisualBasicProjectOptions(
            ImmutableArray<MetadataReference> metadataReferences,
            CompilationOptions compilationOptions)
            : base(metadataReferences, compilationOptions)
        {
        }

        public VisualBasicProjectOptions WithOptionStrict(OptionStrict optionStrict)
        {
            var compilationOptions = (VisualBasicCompilationOptions)CompilationOptions;

            return new VisualBasicProjectOptions(
                MetadataReferences,
                new VisualBasicCompilationOptions(compilationOptions.OutputKind, optionStrict: optionStrict));
        }

        public VisualBasicProjectOptions WithAdditionalMetadataReferences(MetadataReference[] metadataReferences)
        {
           return new VisualBasicProjectOptions(MetadataReferences.AddRange(metadataReferences), CompilationOptions);
        }
    }
}