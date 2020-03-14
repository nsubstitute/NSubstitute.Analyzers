using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.InternalSetupSpecificationCodeFixProviderTests
{
    [CombinatoryData("SubstituteExtensions.When")]
    public class WhenAsOrdinaryMethodTests : InternalSetupSpecificationCodeFixProviderVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberWhenAnalyzer();

        public override async Task ChangesInternalToPublic_ForIndexer_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb(0)
            End Sub)
        End Sub
    End Class
End Namespace";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Default Public Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb(0)
            End Sub)
        End Sub
    End Class
End Namespace";

            await VerifyFix(oldSource, newSource, 1);
        }

        public override async Task ChangesInternalToPublic_ForProperty_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub)
        End Sub
    End Class
End Namespace";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Public Overridable ReadOnly Property Bar As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub)
        End Sub
    End Class
End Namespace";

            await VerifyFix(oldSource, newSource, 1);
        }

        public override async Task ChangesInternalToPublic_ForMethod_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar()
            End Sub)
        End Sub
    End Class
End Namespace";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar()
            End Sub)
        End Sub
    End Class
End Namespace";

            await VerifyFix(oldSource, newSource, 1);
        }

        public override async Task AppendsProtectedInternal_ToIndexer_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb(0)
            End Sub)
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Protected Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb(0)
            End Sub)
        End Sub
    End Class
End Namespace
";

            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsProtectedInternal_ToProperty_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub)
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable ReadOnly Property Bar As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub)
        End Sub
    End Class
End Namespace
";

            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsProtectedInternal_ToMethod_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar()
            End Sub)
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb.Bar()
            End Sub)
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalMember(string method, string call)
        {
            var oldSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherAssembly"")>
Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer

        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb{call}
            End Sub)
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherAssembly"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Friend Overridable ReadOnly Property Bar As Integer

        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, Sub(sb As Foo)
                Dim x = sb{call}
            End Sub)
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource, 2);
        }
    }
}