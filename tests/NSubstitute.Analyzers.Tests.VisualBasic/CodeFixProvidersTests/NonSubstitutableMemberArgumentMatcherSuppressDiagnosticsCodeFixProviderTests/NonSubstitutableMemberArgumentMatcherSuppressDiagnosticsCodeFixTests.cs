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
    public class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixTests
        : VisualBasicSuppressDiagnosticSettingsVerifier, INonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixVerifier
    {
        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenArgMatcherUsedInNonVirtualMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal arg As Integer) As Integer
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

            await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar(System.Int32)~System.Int32", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettings_WhenArgMatcherUsedInNonVirtualIndexer()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim result = substitute(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace
";

            await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim result = substitute(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace
";

            await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage, codeFixIndex: 1);
        }

        [Fact]
        public async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim result = substitute(Arg.Any(Of Integer)())
        End Sub
    End Class
End Namespace
";

            await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage, codeFixIndex: 2);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonSubstitutableMemberArgumentMatcherAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider();
        }
    }
}