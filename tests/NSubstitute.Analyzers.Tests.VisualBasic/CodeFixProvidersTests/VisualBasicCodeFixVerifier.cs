using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests
{
    public abstract class VisualBasicCodeFixVerifier : CodeFixVerifier
    {
        protected override string Language { get; } = LanguageNames.VisualBasic;

        protected override string FileExtension { get; } = "vb";

        protected override CompilationOptions GetCompilationOptions()
        {
            return new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optionStrict: OptionStrict.Off);
        }
    }
}