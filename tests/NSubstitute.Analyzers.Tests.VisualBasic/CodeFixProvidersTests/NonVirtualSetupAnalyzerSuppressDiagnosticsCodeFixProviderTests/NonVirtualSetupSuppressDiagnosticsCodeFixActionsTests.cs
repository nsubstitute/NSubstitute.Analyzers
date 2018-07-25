using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.NonVirtualSetupAnalyzerSuppressDiagnosticsCodeFixProviderTests
{
    public class NonVirtualSetupSuppressDiagnosticsCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, INonVirtualSetupSuppressDiagnosticsCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForIndexer()
        {
            var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            SubstituteExtensions.Returns(substitute(1), 1)
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, new[]
            {
                "Suppress NS001 for indexer Item in nsubstitute.json",
                "Suppress NS001 for class Foo in nsubstitute.json",
                "Suppress NS001 for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForProperty()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            SubstituteExtensions.Returns(substitute.Bar, 1)
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, new[]
            {
                "Suppress NS001 for property Bar in nsubstitute.json",
                "Suppress NS001 for class Foo in nsubstitute.json",
                "Suppress NS001 for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.Returns(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyCodeActions(source, new[]
            {
                "Suppress NS001 for method Bar in nsubstitute.json",
                "Suppress NS001 for class Foo in nsubstitute.json",
                "Suppress NS001 for namespace MyNamespace in nsubstitute.json"
            });
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new NonVirtualSetupSuppressDiagnosticsCodeFixProvider();
        }
    }
}