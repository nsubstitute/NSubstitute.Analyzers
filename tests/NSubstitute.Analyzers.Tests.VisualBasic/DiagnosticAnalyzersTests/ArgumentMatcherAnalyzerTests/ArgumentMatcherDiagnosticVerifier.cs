using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherDiagnosticVerifier : VisualBasicDiagnosticVerifier, IArgumentMatcherDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedWithoutSpecifyingCall;

        [CombinatoryTheory]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("TryCast(Arg.Any(Of Integer)(), Object)")]
        [InlineData("CType(Arg.Any(Of Integer)(), Integer)")]
        [InlineData("DirectCast(Arg.Any(Of Integer)(), Integer)")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("TryCast(Arg.Compat.Any(Of Integer)(), Object)")]
        [InlineData("CType(Arg.Compat.Any(Of Integer)(), Integer)")]
        [InlineData("DirectCast(Arg.Compat.Any(Of Integer)(), Integer)")]
        [InlineData("Arg.Is(1)")]
        [InlineData("TryCast(Arg.Is(1), Object)")]
        [InlineData("CType(Arg.Is(1), Integer)")]
        [InlineData("DirectCast(Arg.Is(1), Integer)")]
        [InlineData("Arg.Compat.Is(1)")]
        [InlineData("TryCast(Arg.Compat.Is(1), Object)")]
        [InlineData("CType(Arg.Compat.Is(1), Integer)")]
        [InlineData("DirectCast(Arg.Compat.Is(1), Integer)")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg);

        [CombinatoryTheory]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("TryCast(Arg.Any(Of Integer)(), Object)")]
        [InlineData("CType(Arg.Any(Of Integer)(), Integer)")]
        [InlineData("DirectCast(Arg.Any(Of Integer)(), Integer)")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("TryCast(Arg.Compat.Any(Of Integer)(), Object)")]
        [InlineData("CType(Arg.Compat.Any(Of Integer)(), Integer)")]
        [InlineData("DirectCast(Arg.Compat.Any(Of Integer)(), Integer)")]
        [InlineData("Arg.Is(1)")]
        [InlineData("TryCast(Arg.Is(1), Object)")]
        [InlineData("CType(Arg.Is(1), Integer)")]
        [InlineData("DirectCast(Arg.Is(1), Integer)")]
        [InlineData("Arg.Compat.Is(1)")]
        [InlineData("TryCast(Arg.Compat.Is(1), Object)")]
        [InlineData("CType(Arg.Compat.Is(1), Integer)")]
        [InlineData("DirectCast(Arg.Compat.Is(1), Integer)")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg);

        [CombinatoryTheory]
        [InlineData("[|Arg.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Compat.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}