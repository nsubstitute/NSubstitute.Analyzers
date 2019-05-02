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

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
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

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
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

        public override async Task ReportsDiagnostics_WhenUseTogetherWithUnfortunatelyNamedArgDoInvoke(string argDoInvoke, string arg)
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
        Public Shared Function [Do](Of T)(ByVal value As T) As T
            Return Nothing
        End Function

        Public Shared Function [Invoke](Of T)(ByVal value As T) As T
            Return Nothing
        End Function

        Public Shared Function [InvokeDelegate](Of T)(ByVal value As T) As T
                Return Nothing
        End Function
    
        Class Compat
            Public Shared Function [Do](Of T)(ByVal value As T) As T
                Return Nothing
            End Function

            Public Shared Function [Invoke](Of T)(ByVal value As T) As T
                Return Nothing
            End Function

            Public Shared Function [InvokeDelegate](Of T)(ByVal value As T) As T
                Return Nothing
            End Function
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg}, {argDoInvoke})
            Dim bar = substitute.Bar({argDoInvoke}, {arg})
            Dim newBar = (New Bar()).FooBar({arg}, {argDoInvoke})
            substitute.[When](Function(x)
                Dim innerBar = (New Bar()).FooBar({argDoInvoke}, {arg})
            End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForMethodCall(string argDo, string arg)
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
            substitute.Bar({arg}, {argDo})
            Dim bar = substitute.Bar({argDo}, {arg})
            Dim newBar = New Bar().FooBar({arg}, {argDo})
            substitute.[When](Function(x)
                                  Dim innerNewBar = New Bar().FooBar({argDo}, {arg})
                              End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForIndexerCall(string argDo, string arg)
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
            Dim indexer = substitute({arg}, {argDo})
            Dim newBar = (New Bar())({argDo}, {arg})
            substitute.[When](Function(x)
                Dim innerNewBar = (New Bar())({arg}, {argDo})
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForMethodCall(string argInvoke, string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer, ByVal y As Action(Of Integer)) As Integer
    End Class

    Public Class Bar
        Public Function FooBar(ByVal x As Integer, ByVal y As Action(Of Integer)) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg}, {argInvoke})
            Dim bar = substitute.Bar({arg}, {argInvoke})
            Dim newBar = New Bar().FooBar({arg}, {argInvoke})
            substitute.[When](Function(x)
                Dim innerNewBar = New Bar().FooBar({arg}, {argInvoke})
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForIndexerCall(string argInvoke, string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Integer, ByVal y As Action(Of Integer)) As Integer
    End Class

    Public Class Bar
        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Action(Of Integer)) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim indexer = substitute({arg}, {argInvoke})
            Dim newBar = (New Bar())({arg}, {argInvoke})
            substitute.[When](Function(x)
                Dim innerNewBar = (New Bar())({arg}, {argInvoke})
            End Function)
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }
    }
}