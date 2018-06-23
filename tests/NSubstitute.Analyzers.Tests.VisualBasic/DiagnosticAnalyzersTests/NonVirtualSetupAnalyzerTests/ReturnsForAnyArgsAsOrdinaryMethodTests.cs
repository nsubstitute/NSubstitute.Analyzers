using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupAnalyzerTests
{
    public class ReturnsForAnyArgsAsOrdinaryMethodTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace

    Public Class FooTests
        Public Sub Test()
            SubstituteExtensions.ReturnsForAnyArgs({literal}, {literal})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    $"Member {literal} can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(7, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            SubstituteExtensions.ReturnsForAnyArgs(Foo.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnValue = substitute.Bar()
            SubstituteExtensions.ReturnsForAnyArgs(returnValue, 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Integer
            Return 2
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(24, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Interface IFoo

        Function Bar() As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Interface IFoo

       Property Bar As Integer

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(Of Integer), 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Integer
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As Integer
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable ReadOnly Property Bar As Integer
            Get
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace

    Public Class Foo

        Public Overridable Default Property Item(ByVal x As Integer) As Integer
            Set
                Throw New NotImplementedException
            End Set
            Get
                Throw New NotImplementedException
            End Get

        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Integer
            Return 1
        End Function
    End Class

    Module SubstituteExtensions
        <Extension>
        Function ReturnsForAnyArgs(Of T)(ByVal returnValue As T, ByVal returnThis As T) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Integer
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.FooBar, 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 52)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.FooBar, 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 52)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(1, 2), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(1), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 52)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Integer
            Return 1
        End Function

        Public Function Bar(Of T)(ByVal x As T, ByVal y As T) As Integer
            Return 2
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(Of Integer)(1, 2), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(1), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 52)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1 ,2), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(22, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo(Of T)
        Default Public ReadOnly Property Item(ByVal x As T) As Integer
            Get
                Return 0
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As T, ByVal y As T) As Integer
            Get
                Return 0
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1 ,2), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(22, 52)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.FooBar(), 1)
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.FooBar(), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(39, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(40, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 52)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooBarBar(Of T)
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.FooBar(), 1)
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.FooBar(), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(39, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(40, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 52)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports MyOtherNamespace

Namespace MyOtherNamespace
    Public Class FooBarBar
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class
End Namespace

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Integer

        Default Public ReadOnly Property Item(ByVal x As Integer) As Integer
            Get
                Return 0
            End Get
        End Property

        Public Function FooBar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            SubstituteExtensions.ReturnsForAnyArgs(substitute(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substitute.FooBar(), 1)
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar(1), 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.Bar, 1)
            SubstituteExtensions.ReturnsForAnyArgs(substituteFooBarBar.FooBar(), 1)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(43, 52)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(44, 52)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}