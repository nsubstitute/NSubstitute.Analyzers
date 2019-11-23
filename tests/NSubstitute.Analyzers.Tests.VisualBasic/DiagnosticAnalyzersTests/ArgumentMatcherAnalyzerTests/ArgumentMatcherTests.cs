using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public class ArgumentMatcherTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenUsedInNonVirtualMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        
        Public Function Bar(ByVal firstArg As Object) As Integer
            Return 2
        End Function

        Public Function Bar(ByVal firstArg As Integer) As Integer
            Return 2
        End Function

        Public Function Bar(ByVal firstArg As Action) As Integer
            Return 2
        End Function

    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsDiagnostics_WhenUsedInStaticMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Shared Function Bar(ByVal firstArg As Object) As Integer
            Return 2
        End Function

        Public Shared Function Bar(ByVal firstArg As Integer) As Integer
            Return 2
        End Function

        Public Shared Function Bar(ByVal firstArg As Action) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Foo.Bar({arg})
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInVirtualMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal firstArg As Integer?) As Integer
            Return 2
        End Function

        Public Overridable Function Bar(ByVal firstArg As Object) As Integer
            Return 2
        End Function

        Public Overridable Function Bar(ByVal firstArg As Action) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInNonSealedOverrideMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal firstArg As Integer?) As Integer
            Return 2
        End Function
    
        Public Overridable Function Bar(ByVal firstArg As Action) As Integer
            Return 2
        End Function

    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Public Overrides Function Bar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInDelegate(string arg)
        {
            var delegateArgType = arg.EndsWith("Invoke()") ? "Action" : "Integer?";

            var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of {delegateArgType}, Integer))()
            Dim x = substitute({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedInSealedOverrideMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo

        Public Overridable Function Bar(ByVal firstArg As Object) As Integer
            Return 2
        End Function
        
        Public Overridable Function Bar(ByVal firstArg As Integer) As Integer
            Return 2
        End Function

        Public Overridable Function Bar(ByVal firstArg As Action) As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar(ByVal firstArg As Object) As Integer
            Return 1
        End Function

        Public NotOverridable Overrides Function Bar(ByVal firstArg As Integer) As Integer
            Return 1
        End Function
        
        Public NotOverridable Overrides Function Bar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInAbstractMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal firstArg As Object) As Integer

        Public MustOverride Function Bar(ByVal firstArg As Integer?) As Integer

        Public MustOverride Function Bar(ByVal firstArg As Action) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInInterfaceMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal firstArg As Integer?) As Integer

        Function Bar(ByVal firstArg As Action) As Integer

        Function Bar(ByVal firstArg As Object) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInGenericInterfaceMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo(Of T)
        Function Bar(Of T)(ByVal firstArg as Object) As Integer

        Function Bar(Of T)(ByVal firstArg as Integer?) As Integer

        Function Bar(Of T)(ByVal firstArg as Action) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            substitute.Bar(Of Integer)({arg})
        End Sub
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInInterfaceIndexer(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface IFoo
        Default ReadOnly Property Item(ByVal i As Integer?) As Integer

        Default ReadOnly Property Item(ByVal i As Action) As Integer

        Default ReadOnly Property Item(ByVal i As Object) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            Dim x = substitute({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInVirtualIndexer(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public Overridable ReadOnly Property Item(ByVal x As Integer?) As Integer
            Get
                Return 0
            End Get
        End Property
    
        Default Public Overridable ReadOnly Property Item(ByVal x As Action) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedInNonVirtualIndexer(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        
        Default Public ReadOnly Property Item(ByVal x As Object) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Action) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal firstArg As Action) As Integer
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

        Public Shared Function Invoke() As Action
            Return Nothing
        End Function

        Public Shared Function [Do](Of T)(ByVal value As Action(Of T)) As T
            Return Nothing
        End Function

        Public Shared Function [InvokeDelegate](Of T)() As T
            Return Nothing
        End Function

        Public Class Compat
            Public Shared Function Any(Of T)() As T
                Return Nothing
            End Function

            Public Shared Function [Is](Of T)(ByVal value As T) As T
                Return Nothing
            End Function

            Public Shared Function Invoke() As Action
                Return Nothing
            End Function

            Public Shared Function [Do](Of T)(ByVal value As Action(Of T)) As T
                Return Nothing
            End Function

            Public Shared Function [InvokeDelegate](Of T)() As T
                Return Nothing
            End Function
        End Class
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithPotentiallyValidAssignment(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim x = {arg}
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedAsStandaloneExpression(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
             {arg}
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsDiagnostics_WhenUsedInConstructor(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        
        Public Sub New(ByVal firstArg As Object)
        End Sub

        Public Sub New(ByVal firstArg As Integer)
        End Sub

        Public Sub New(ByVal firstArg As Action)
        End Sub

        Public Sub Test()
            Dim x = New FooTests({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToNotApplied(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        
        Friend Overridable Function FooBar(ByVal firstArg As Object) As Integer
            Return 1
        End Function

        Friend Overridable Function FooBar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Friend Overridable Function FooBar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.FooBar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToApplied(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: InternalsVisibleTo(""OtherSecondAssembly"")>
Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function FooBar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function
    
        Friend Overridable Function FooBar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.FooBar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherAssembly"")>
Namespace MyNamespace
    Public Class Foo

        Friend Overridable Function FooBar(ByVal firstArg As Object) As Integer
            Return 1
        End Function
        
        Friend Overridable Function FooBar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Friend Overridable Function FooBar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.FooBar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedInProtectedInternalVirtualMember(string arg)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable Function FooBar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Protected Friend Overridable Function FooBar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.FooBar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string arg)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", ArgumentMatcherUsedWithoutSpecifyingCall.Id);
            Settings.Suppressions.Add(new Suppression
            {
                Target = "M:MyNamespace.Foo.Bar(System.Action,System.Action)",
                Rules = new List<string> { ArgumentMatcherUsedWithoutSpecifyingCall.Id }
            });

            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 2
        End Function

        Public Function Bar(ByVal x As Action) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal x As Action, ByVal y As Action) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar({arg}, {arg})
            substitute.Bar([|{arg}|])
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }

        public override async Task ReportsNoDiagnostic_WhenOverloadCannotBeInferred()
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable Function FooBar(ByVal firstArg As Integer?) As Integer
            Return 1
        End Function

        Protected Friend Overridable Function FooBar(ByVal firstArg As Action) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.FooBar(Arg.Any(Of Object)())
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }
    }
}