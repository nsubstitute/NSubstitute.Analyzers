using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupAnalyzerTests
{
    public class ReturnsNullAsExtensionMethodTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo
        Public Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().ReturnsNull()
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
                    new DiagnosticResultLocation(16, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            return Task.CompletedTask;
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Shared Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Foo.Bar().ReturnsNull()
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
                    new DiagnosticResultLocation(16, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public Overrides Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar().ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnValue = substitute.Bar()
            returnValue.ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of FooTests))()
            substitute().ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class Foo2
        Inherits Foo

        Public NotOverridable Overrides Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo2)()
            substitute.Bar().ReturnsNull()
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
                    new DiagnosticResultLocation(25, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride Function Bar() As Foo
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar().ReturnsNull()
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Interface IFoo

        Function Bar() As IFoo

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Interface IFoo

       Property Bar As IFoo

    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.ReturnsNull()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Interface IFoo(Of T)

        Function Bar(Of T)() As FooTests
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo(Of Integer))()
            substitute.Bar(Of Integer).ReturnsNull()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public MustInherit Class Foo

        Public MustOverride ReadOnly Property Bar As Foo
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.ReturnsNull()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Interface IFoo

        Default Property Item(ByVal i As Integer) As IFoo
    End Interface

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)
            substitute(1).ReturnsNull()
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable ReadOnly Property Bar As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.ReturnsNull()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute.Bar.ReturnsNull()
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Overridable Default Property Item(ByVal x As Integer) As Foo
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
            substitute(1).ReturnsNull()
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace

    Public Class Foo

        Public Default ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of Foo)
            substitute(1).ReturnsNull()
        End Sub
    End Class
End Namespace";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Item can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(20, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Foo
        Public Function Bar() As Foo
            Return Nothing
        End Function
    End Class

    Module ReturnsExtensions
        <Extension>
        Function ReturnsNull(Of T)(ByVal returnValue As T, ByVal returnThis As Object) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            substitute.Bar().ReturnsNull(Nothing)
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
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public ReadOnly Property Bar As Foo
        Public ReadOnly Property FooBar As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar.ReturnsNull()
            substitute.FooBar.ReturnsNull()
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
                    new DiagnosticResultLocation(14, 13)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public ReadOnly Property Bar As T
        Public ReadOnly Property FooBar As FooTests
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of FooTests))()
            substitute.Bar.ReturnsNull()
            substitute.FooBar.ReturnsNull()
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
                    new DiagnosticResultLocation(14, 13)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Foo
            Return Nothing
        End Function

        Public Function Bar(ByVal x As Integer, ByVal y As Integer) As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(1, 2).ReturnsNull()
            substitute.Bar(1).ReturnsNull()
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
                    new DiagnosticResultLocation(19, 13)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Function Bar(ByVal x As Integer) As Foo
            Return Nothing
        End Function

        Public Function Bar(Of T)(ByVal x As T, ByVal y As T) As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Of Integer)(1, 2).ReturnsNull()
            substitute.Bar(1).ReturnsNull()
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
                    new DiagnosticResultLocation(19, 13)
                }
            };
            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1, 2).ReturnsNull()
            substitute(1).ReturnsNull()
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
                    new DiagnosticResultLocation(23, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Default Public ReadOnly Property Item(ByVal x As T) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(ByVal x As T, ByVal y As T) As FooTests
            Get
                Return Nothing
            End Get
        End Property
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            substitute(1, 2).ReturnsNull()
            substitute(1).ReturnsNull()
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
                    new DiagnosticResultLocation(23, 13)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

             var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As Foo

        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooBarBar
        Public Property Bar As Foo

        Default Public ReadOnly Property Item(ByVal x As Integer) As Foo
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As Foo
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1).ReturnsNull()
            substitute.Bar.ReturnsNull()
            substitute.FooBar().ReturnsNull()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            substituteFooBarBar(1).ReturnsNull()
            substituteFooBarBar.Bar.ReturnsNull()
            substituteFooBarBar.FooBar().ReturnsNull()
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
                        new DiagnosticResultLocation(40, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 13)
                    }
                }
            };

             await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class Foo(Of T)
        Public Property Bar As FooTests

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooTests
            Return Nothing
        End Function
    End Class

    Public Class FooBarBar(Of T)
        Public Property Bar As FooTests

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooTests
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooTests
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo(Of Integer))()
            substitute(1).ReturnsNull()
            substitute.Bar.ReturnsNull()
            substitute.FooBar().ReturnsNull()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar(Of Integer))()
            substituteFooBarBar(1).ReturnsNull()
            substituteFooBarBar.Bar.ReturnsNull()
            substituteFooBarBar.FooBar().ReturnsNull()
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
                        new DiagnosticResultLocation(40, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports MyOtherNamespace

Namespace MyOtherNamespace
    Public Class FooBarBar
        Public Property Bar As FooBarBar

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooBarBar
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooBarBar
            Return Nothing
        End Function
    End Class
End Namespace

Namespace MyNamespace
    Public Class Foo
        Public Property Bar As FooBarBar

        Default Public ReadOnly Property Item(ByVal x As Integer) As FooBarBar
            Get
                Return Nothing
            End Get
        End Property

        Public Function FooBar() As FooBarBar
            Return Nothing
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute(1).ReturnsNull()
            substitute.Bar.ReturnsNull()
            substitute.FooBar().ReturnsNull()
            Dim substituteFooBarBar = NSubstitute.Substitute.[For](Of FooBarBar)()
            substituteFooBarBar(1).ReturnsNull()
            substituteFooBarBar.Bar.ReturnsNull()
            substituteFooBarBar.FooBar().ReturnsNull()
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
                        new DiagnosticResultLocation(43, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(44, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(45, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(MyNamespace.IFoo)~MyNamespace.FooTests", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"Imports System.Runtime.CompilerServices
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Bar = NSubstitute.Substitute.[For](Of IBar)()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.GetBar().ReturnsNull()
            substitute.GetFooBar().ReturnsNull()
        End Sub
    End Class

    Module MyExtensions
        Public Property Bar As IBar

        <Extension()>
        Function GetBar(ByVal foo As IFoo) As FooTests
            Return Bar.Foo()
        End Function

        <Extension()>
        Function GetFooBar(ByVal foo As IFoo) As FooTests
            Return foo.Bar()
        End Function
    End Module

    Interface IBar
        Function Foo() As FooTests
    End Interface

    Interface IFoo
        Function Bar() As FooTests
    End Interface
End Namespace";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member GetFooBar can not be intercepted. Only interface members and overrideable, overriding, and must override members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(10, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}