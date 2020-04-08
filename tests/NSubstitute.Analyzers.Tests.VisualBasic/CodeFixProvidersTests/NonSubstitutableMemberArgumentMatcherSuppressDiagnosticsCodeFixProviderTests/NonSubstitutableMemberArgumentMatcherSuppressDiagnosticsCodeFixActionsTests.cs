using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProviderTests
{
    public class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, INonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixActionsVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberArgumentMatcherAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider();

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
            Dim result = substitute(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for indexer Item in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace
";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for method Bar in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInStandaloneExpression()
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Arg.Any(Of Integer)()
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Fact]
        public async Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInConstructor()
        {
            var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub New(ByVal firstArg As Integer)
        End Sub

        Public Sub Test()
            Dim result = New FooTests(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace";
            await VerifyCodeActions(source, Array.Empty<string>());
        }
    }
}