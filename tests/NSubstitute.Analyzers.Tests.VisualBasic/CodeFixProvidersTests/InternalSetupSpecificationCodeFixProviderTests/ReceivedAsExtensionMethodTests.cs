using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.InternalSetupSpecificationCodeFixProviderTests
{
    [CombinatoryData("Received")]
    public class ReceivedAsExtensionMethodTests : InternalSetupSpecificationCodeFixProviderVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedAnalyzer();

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
            Dim x = substitute.{method}()(1)
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
            Dim x = substitute.{method}()(1)
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
            Dim x = substitute.{method}().Bar
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
            Dim x = substitute.{method}().Bar
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
            Dim x = substitute.{method}().Bar()
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
            Dim x = substitute.{method}().Bar()
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
            Dim x = substitute.{method}()(0)
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
            Dim x = substitute.{method}()(0)
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
            Dim x = substitute.{method}().Bar
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
            Dim x = substitute.{method}().Bar
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
            Dim x = substitute.{method}().Bar()
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
            Dim x = substitute.{method}().Bar()
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
            Dim x = substitute.{method}(){call}
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherAssembly"")>
<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
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
            Dim x = substitute.{method}(){call}
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource, 2);
        }

        public override async Task AppendsInternalsVisibleToWithFullyQualifiedName_WhenUsedWithInternalMemberAndCompilerServicesNotImported(string method)
        {
            var oldSource = $@"Imports NSubstitute

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherAssembly"")>
Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.{method}().FooBar()
        End Sub
    End Class
End Namespace
";

            var newSource = $@"Imports NSubstitute

<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherAssembly"")>
<Assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.{method}().FooBar()
        End Sub
    End Class
End Namespace
";
            await VerifyFix(oldSource, newSource, 2);
        }
    }
}