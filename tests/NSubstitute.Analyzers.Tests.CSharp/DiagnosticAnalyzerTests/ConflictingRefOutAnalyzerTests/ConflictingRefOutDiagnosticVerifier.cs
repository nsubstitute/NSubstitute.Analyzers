using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ConflictingRefOutAnalyzerTests
{
    public class ConflictingRefOutDiagnosticVerifier : CSharpDiagnosticVerifier
    {
        public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ConflictingAssignmentsToOutRefArgument;

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ConflictingRefOutAnalyzer();
        }
    }
}