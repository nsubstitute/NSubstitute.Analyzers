﻿using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests;

public class ForAsGenericMethodTests : SubstituteDiagnosticVerifier
{
    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForInterface()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of IFoo)(1)|]
            Dim anotherSubstitute = [|NSubstitute.Substitute.[For](Of IFoo)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForInterfaceDescriptor, "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.[For](Of IFoo)() instead.");
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForDelegate()
    {
        var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
    {
        var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Func(Of Integer))(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of Func(Of Integer))(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteConstructorArgumentsForDelegateDescriptor, "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.[For](Of Func(Of Integer))() instead.");
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleClasses()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo, Bar)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteMultipleClassesDescriptor);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleSameClasses()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo, Foo)()
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleInterfaces()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Interface IBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo, IBar)()
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsInterfaceNotImplementedByClass()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo, Bar)()
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleGenericTypeParameters_ContainsClassWithoutMatchingConstructor()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of IFoo, Bar)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of IFoo, Bar)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.IFoo, MyNamespace.Bar) do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Private Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied()
    {
        var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)(1, 2, 3)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of Foo)(New Integer() { 1, 2, 3 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of Foo)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal Optional y As Integer = 1)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of Foo)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

{assemblyAttributes}

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes)
    {
        var source = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices
{assemblyAttributes}
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    [Theory]
    [InlineData("ByVal x As Decimal", "1")] // valid c# but doesnt work in NSubstitute
    [InlineData("ByVal x As Integer", "1D")]
    [InlineData("ByVal x As Integer", "1R")]
    [InlineData("ParamArray z As Integer()", "1D")]
    [InlineData("ParamArray z As Integer()", "1, 1D")]
    [InlineData("ByVal x As List(Of Integer)", "New List(Of Integer)().AsReadOnly()")]
    [InlineData("ByVal x As Integer", "New Object()")]
    [InlineData("ParamArray z As Integer()", "New Object() { 1D }")]
    [InlineData("ParamArray z As Integer()", "New Object() { 1, 1D }")]
    [InlineData("ParamArray z As Integer()", "{ 1D }")]
    [InlineData("ParamArray z As Integer()", "{ 1, 1D }")]
    public override async Task ReportsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"Imports NSubstitute
Imports System.Collections.Generic

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)({invocationValues})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.Foo) do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
    }

    [Theory]
    [InlineData("ByVal x As Integer", "1")]
    [InlineData("ByVal x As Single", @"""c""c")]
    [InlineData("ByVal x As Integer", @"""c""c")]
    [InlineData("ByVal x As IList(Of Integer)", "New List(Of Integer)()")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New List(Of Integer)()")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New List(Of Integer)().AsReadOnly()")]
    [InlineData("ByVal x As IEnumerable(Of Char)", @"""value""")]
    [InlineData("ByVal x As Integer, ParamArray z As String()", "1, \"foo\", \"foo\"")]
    [InlineData("ByVal x As Integer", @"New Object() {1}")]
    [InlineData("ByVal x As Integer()", @"New Integer() {1}")]
    [InlineData("ByVal x As Object(), ByVal y As Integer", @"New Object() {1}, 1")]
    [InlineData("ByVal x As Integer(), ByVal y As Integer", @"New Integer() {1}, 1")]
    [InlineData("", @"New Object() {}")]
    [InlineData("", "New Object() {1, 2}.ToArray()")] // actual values known at runtime only so constructor analysys skipped
    [InlineData("ByVal x As Integer", "New Object() {Nothing}")] // even though we pass null as first arg, this works fine with NSubstitute
    [InlineData("ByVal x As Integer, ByVal y As Integer", "New Object() { Nothing, Nothing }")] // even though we pass null as first arg, this works fine with NSubstitute
    [InlineData("ByVal x As Integer, ByVal y As Integer", "New Object() {1, Nothing}")] // even though we pass null as last arg, this works fine with NSubstitute
    [InlineData("ByVal x As Integer, ParamArray z As String()", "New Object() {1, \"foo\", \"foo\"}")]
    public override async Task ReportsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
    {
        var source = $@"Imports NSubstitute
Imports System.Collections.Generic
Imports System.Linq

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)({invocationValues})
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithGenericArgument()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Function Foo(Of T As Class)() As T
            Return Substitute.[For](Of T)()
        End Function
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersNotProvided()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ParamArray y As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)(1)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenParamsParametersProvided()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ParamArray y As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)(1, 2, 3)
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of Foo)(New Object() { 1, 2, New Integer() { 3 } })
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount_AndParamsParameterDefined()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer, ParamArray z As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For](Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](Of Foo)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For](Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }
}