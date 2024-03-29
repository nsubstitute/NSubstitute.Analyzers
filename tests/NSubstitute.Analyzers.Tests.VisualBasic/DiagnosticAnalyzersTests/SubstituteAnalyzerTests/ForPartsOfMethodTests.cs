﻿using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests;

public class ForPartsOfMethodTests : SubstituteDiagnosticVerifier
{
    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForInterface()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of IFoo)()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use NSubstitute.Substitute.For(Of IFoo)() instead of NSubstitute.Substitute.ForPartsOf(Of IFoo)() here.");
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenUsedForDelegate()
    {
        var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Func(Of Integer))()|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use NSubstitute.Substitute.For(Of Func(Of Integer))() instead of NSubstitute.Substitute.ForPartsOf(Of Func(Of Integer))() here.");
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)()|]
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)()|]
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)()|]
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
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
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
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(1, 2, 3)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(New Integer() { 1, 2, 3 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(New Object() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)(New Object() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)()|]
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
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)()|]
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
            Dim substitute = [|NSubstitute.Substitute.ForPartsOf(Of Foo)({invocationValues})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
    }

    [Theory]
    [InlineData("ByVal x As Integer", "New Object() {1}")]
    [InlineData("ByVal x As Single", "New Object() {\"c\"c}")]
    [InlineData("ByVal x As Integer", "New Object() {\"c\"c}")]
    [InlineData("ByVal x As IList(Of Integer)", "New Object() {New List(Of Integer)()}")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)()}")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)().AsReadOnly()}")]
    [InlineData("ByVal x As IEnumerable(Of Char)", @"New Object() {""value""}")]
    [InlineData("ByVal x As Integer, ParamArray z As String()", "1, \"foo\", \"foo\"")]
    [InlineData("", @"New Object() {}")]
    [InlineData("", "New Object() {1, 2}.ToArray()")] // actual values known at runtime only so constructor analysys skipped
    [InlineData("ByVal x As Integer, ParamArray z As String()", "New Object() {1, \"foo\", \"foo\" }")]
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
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)({invocationValues})
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
        Public Function FooPartsOf(Of T As Class)() As T
            Return Substitute.ForPartsOf(Of T)()
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
            Dim substitute = NSubstitute.Substitute.[ForPartsOf](Of Foo)(1)
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
            Dim substitute = NSubstitute.Substitute.[ForPartsOf](Of Foo)(1, 2, 3)
            Dim otherSubstitute = NSubstitute.Substitute.[ForPartsOf](Of Foo)(New Object() { 1, 2, New Integer() { 3 } })
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
            Dim substitute = [|NSubstitute.Substitute.[ForPartsOf](Of Foo)(1)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[ForPartsOf](Of Foo)(New Integer() { 1 })|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }
}