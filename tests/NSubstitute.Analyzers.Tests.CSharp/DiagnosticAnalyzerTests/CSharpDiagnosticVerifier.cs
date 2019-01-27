using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests
{
    public abstract class CSharpDiagnosticVerifier : DiagnosticVerifier
    {
        protected override string Language { get; } = LanguageNames.CSharp;

        protected override string FileExtension { get; } = "cs";

        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }
    }
}