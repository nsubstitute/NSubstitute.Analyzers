using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public class ArgumentMatcherMisuseTests : ArgumentMatcherMisuseDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
    End Class

    Public Class Bar
        Public Function FooBar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg}, {arg})
            Dim bar = substitute.Bar({arg}, {arg})
            Dim newBar = New Bar().FooBar({arg}, {arg})
            substitute.[When](Function(x)
                                  Dim innerNewBar = New Bar().FooBar({arg}, {arg})
                              End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
    End Class

    Public Class Bar
        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim indexer = substitute({arg}, {arg})
            Dim newBar = (New Bar())({arg}, {arg})
            substitute.[When](Function(x)
                Dim innerNewBar = (New Bar())({arg}, {arg})
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
    End Class

    Public Class Bar
        Public Function FooBar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class Arg
        Public Shared Function Any(Of T)() As T
            Return Nothing
        End Function

        Public Shared Function [Is](Of T)(ByVal value As T) As T
            Return Nothing
        End Function

        Class Compat
            Public Shared Function Any(Of T)() As T
                Return Nothing
            End Function

            Public Shared Function [Is](Of T)(ByVal value As T) As T
                Return Nothing
            End Function
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg}, {arg})
            Dim bar = substitute.Bar({arg}, {arg})
            Dim newBar = (New Bar()).FooBar({arg}, {arg})
            substitute.[When](Function(x)
                Dim innerBar = (New Bar()).FooBar({arg}, {arg})
            End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }
    }
}