using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzerTests
{
    public class VisualBasicDiagnosticVerifier<T> : DiagnosticVerifier where T : DiagnosticAnalyzer, new()
    {
        private static readonly MetadataReference[] AdditionalReferences =
        {
            MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)
        };

        protected override string Language { get; } = LanguageNames.VisualBasic;

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new T();
        }

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