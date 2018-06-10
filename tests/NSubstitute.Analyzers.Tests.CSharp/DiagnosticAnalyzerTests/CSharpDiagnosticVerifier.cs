using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests
{
    public abstract class CSharpDiagnosticVerifier : DiagnosticVerifier
    {
        protected override string Language { get; } = LanguageNames.CSharp;

        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }
    }
}