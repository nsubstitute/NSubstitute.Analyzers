using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests
{
    public class CSharpDiagnosticVerifier<T> : DiagnosticVerifier where T : DiagnosticAnalyzer, new()
    {
        protected override string Language { get; } = LanguageNames.CSharp;

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new T();
        }

        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }
    }
}