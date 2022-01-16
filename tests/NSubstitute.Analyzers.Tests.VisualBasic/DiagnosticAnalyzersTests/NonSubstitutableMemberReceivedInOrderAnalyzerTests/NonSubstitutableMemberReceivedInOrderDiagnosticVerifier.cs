using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberReceivedInOrderAnalyzerTests;

public class NonSubstitutableMemberReceivedInOrderDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonSubstitutableMemberReceivedInOrderDiagnosticVerifier
{
    internal AnalyzersSettings Settings { get; set; }

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedInOrderAnalyzer();

    private readonly DiagnosticDescriptor _internalSetupSpecificationDescriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

    private readonly DiagnosticDescriptor _nonVirtualReceivedInOrderSetupSpecificationDescriptor = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualReceivedInOrderSetupSpecification;

    protected override string AnalyzerSettings => Settings != null ? Json.Encode(Settings) : null;

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithoutAssignment()
    {
        var source = @"Imports NSubstitute
Imports System.Threading.Tasks
Namespace MyNamespace
    Public Class Foo
        Public Property Nested As Foo

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooBar
        Public Property Nested As FooBar

        Public Function Bar() As Task(Of Integer)
            Return Task.FromResult(1)
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of FooBar)()

            NSubstitute.Received.InOrder(Function()
                                 [|substitute.Bar()|]
                             End Function)
            NSubstitute.Received.InOrder(Function() [|substitute.Bar()|])
            NSubstitute.Received.InOrder(Sub() [|substitute.Bar()|])
            NSubstitute.Received.InOrder(Function()
                                 [|substitute.Nested.Bar()|]
                             End Function)
            NSubstitute.Received.InOrder(Function() [|substitute.Nested.Bar()|])
            NSubstitute.Received.InOrder(Sub() [|substitute.Nested.Bar()|])
            NSubstitute.Received.InOrder(Async Function() Await [|otherSubstitute.Bar()|])
            NSubstitute.Received.InOrder(Async Sub() Await [|otherSubstitute.Bar()|])
            NSubstitute.Received.InOrder(Async Function() Await [|otherSubstitute.Nested.Bar()|])
            NSubstitute.Received.InOrder(Async Sub() Await [|otherSubstitute.Nested.Bar()|])
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithNonUsedAssignment()
    {
        var source = @"Imports NSubstitute
Imports System.Threading.Tasks
Namespace MyNamespace
    Public Class Foo
        Public Function Bar() As Integer
            Return 2
        End Function

        Public Function Bar(ByVal x As Integer) As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooBar
        Public Function Bar() As Task(Of Integer)
            Return Task.FromResult(1)
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of FooBar)()
            NSubstitute.Received.InOrder(Function()
                                 [|substitute.Bar()|]
                                 Dim y = [|substitute.Bar()|]
                                 Dim z = CInt([|substitute.Bar()|])
                                 Dim zz = TryCast([|substitute.Bar()|], Object)
                             End Function)
            NSubstitute.Received.InOrder(Async Function()
                                 Await [|otherSubstitute.Bar()|]
                                 Dim y = Await [|otherSubstitute.Bar()|]
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithoutAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property Nested As Foo

        Public Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute.Bar|]
                                         End Function)
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute.Nested.Bar|]
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithNonUsedAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute.Bar|]
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithoutAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property Nested As Foo

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute(1)|]
                                         End Function)
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute.Nested(1)|]
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithNonUsedAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = [|substitute(1)|]
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenSubscribingToEvent()
    {
        var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class Foo
        Public Event SomeEvent As Action

        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                AddHandler substitute.SomeEvent, Arg.Any(Of Action)()
            End Function)
        End Sub

    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualMethodWithUsedAssignment()
    {
        var source = @"Imports NSubstitute
Imports System.Threading.Tasks
Namespace MyNamespace
    Public Class Foo
        Public Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooBar
        Public Function Bar() As Task(Of Integer)
            Return Task.FromResult(1)
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of FooBar)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar()
                                             Dim y = x
                                         End Function)
            NSubstitute.Received.InOrder(Async Function()
                                             Dim x = Await otherSubstitute.Bar()
                                             Dim y = x
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualPropertyWithUsedAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar
                                             Dim y = x
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingNonVirtualIndexerWithUsedAssignment()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim a = substitute(1)
                                             Dim b = TryCast(substitute(1), Object)
                                             Dim c = CInt(substitute(1))
                                             Dim d = CType(substitute(1), Integer)
                                             Dim e = DirectCast(substitute(1), Integer)
                                             Dim aa = a
                                             Dim bb = b
                                             Dim cc = c
                                             Dim dd = d
                                             Dim ee = e
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenNonVirtualMethodIsCalledAsArgument()
    {
        var source = @"Imports NSubstitute
Imports System.Threading.Tasks
Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal x As Object) As Integer
            Return 2
        End Function

        Public Function FooBar() As Object
            Return 1
        End Function
    End Class

    Public Class FooBar
        Public Function Bar() As Task(Of Integer)
            Return Task.FromResult(1)
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of FooBar)()
            NSubstitute.Received.InOrder(Function()
                                             Dim local = 1
                                             Dim x = substitute.Bar(substitute.FooBar())
                                             substitute.Bar(substitute.FooBar())
                                             substitute.Bar(CInt(substitute.FooBar()))
                                             substitute.Bar(TryCast(substitute.FooBar(), Object))
                                             substitute.Bar(DirectCast(substitute.FooBar(), Integer))
                                             substitute.Bar(CType(substitute.FooBar(), Integer))
                                             substitute.Bar(local)
                                             substitute.Bar(1)
                                         End Function)
            NSubstitute.Received.InOrder(Async Function()
                                             Dim x = substitute.Bar(Await otherSubstitute.Bar())
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenNonVirtualPropertyIsCalledAsArgument()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function

        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar(substitute.FooBar)
                                             substitute.Bar(substitute.FooBar)
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenNonVirtualIndexerIsCalledAsArgument()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 2
        End Function

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             Dim x = substitute.Bar(substitute(1))
                                             substitute.Bar(substitute(1))
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingProtectedInternalVirtualMember()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Protected Friend Overridable ReadOnly Property Bar As Integer

        Protected Friend Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Protected Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             substitute.FooBar()
                                             Dim x = substitute.Bar
                                             Dim y = substitute(1)
                                         End Function)
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingVirtualMember()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Overridable ReadOnly Property Bar As Integer

        Overridable Function FooBar() As Integer
            Return 1
        End Function

        Default Protected Friend Overridable ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 1
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                             substitute.FooBar()
                                             Dim x = substitute.Bar
                                             Dim y = substitute(1)
                                         End Function)
            NSubstitute.Received.InOrder(Function() substitute.FooBar())
            NSubstitute.Received.InOrder(Sub() substitute.FooBar())
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

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
            NSubstitute.Received.InOrder(Function()
                                             [|substitute.FooBar()|]
                                             Dim x = [|substitute.Bar|]
                                             Dim y = [|substitute(1)|]
                                         End Function)
        End Sub
    End Class
End Namespace
";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Friend member FooBar can not be intercepted without InternalsVisibleToAttribute.",
            "Friend member Bar can not be intercepted without InternalsVisibleToAttribute.",
            "Friend member Item can not be intercepted without InternalsVisibleToAttribute."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(_internalSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToApplied()
    {
        var source = @"Imports System
Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: InternalsVisibleTo(""OtherSecondAssembly"")>
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
            NSubstitute.Received.InOrder(Function()
                                             substitute.FooBar()
                                             Dim x = substitute.Bar
                                             Dim y = substitute(1)
                                         End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", _nonVirtualReceivedInOrderSetupSpecificationDescriptor.Id);

        var source = @"Imports System
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
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                 substitute.Bar(1, 1)
                                 [|substitute.Bar(1)|]
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsDiagnostics_WhenNonVirtualMethodUsedAsPartOfExpression_WithoutAssignment()
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
            NSubstitute.Received.InOrder(Function()
                                 Dim a = [|substitute.Bar()|] + [|substitute.Bar()|]
                                 Dim b = [|substitute.Bar()|] - [|substitute.Bar()|]
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, _nonVirtualReceivedInOrderSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.");
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenNonVirtualMethodUsedAsPartOfExpression_WithAssignment()
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
            NSubstitute.Received.InOrder(Function()
                                 Dim a = substitute.Bar() + substitute.Bar()
                                 Dim b = substitute.Bar() - substitute.Bar()
                                 Dim aa = a
                                 Dim bb = b
                             End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostics_WhenAccessingVirtualMemberViaNonVirtualAccessor()
    {
        var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property NestedFoo As Foo

        Public Function NestedFooMethod() As Foo
            Return Nothing
        End Function

        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Public Overridable Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                                     substitute.NestedFoo.Bar(1)
                                     substitute.NestedFooMethod().Bar(1)
                                     substitute(1).Bar(1)
                                 End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}