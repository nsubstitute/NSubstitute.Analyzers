using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests
{
    public abstract class VisualBasicSuppressDiagnosticSettingsVerifier : SuppressDiagnosticSettingsVerifier
    {
        private static readonly MetadataReference[] AdditionalReferences =
        {
            MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)
        };

        protected override string Language { get; } = LanguageNames.VisualBasic;

        protected override string FileExtension { get; } = "vb";

        protected override CompilationOptions GetCompilationOptions()
        {
            return new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optionStrict: OptionStrict.On);
        }

        protected override IEnumerable<MetadataReference> GetAdditionalMetadataReferences()
        {
            return AdditionalReferences;
        }
    }
}