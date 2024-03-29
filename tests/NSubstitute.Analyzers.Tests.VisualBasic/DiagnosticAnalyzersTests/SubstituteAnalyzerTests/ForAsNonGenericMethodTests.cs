﻿using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests;

public class ForAsNonGenericMethodTests : SubstituteDiagnosticVerifier
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(IFoo)})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenUsedForInterface_WhenEmptyArrayPassed()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo)}, New Object() {})
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)}, constructorArguments:= New Object() {})
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= New Object() {}, typesToProxy:= {GetType(IFoo)})
        End Sub
    End Class
End Namespace
";

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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(IFoo)}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(IFoo)})|]
        End Sub
    End Class
End Namespace
";
        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.[For]({GetType(IFoo)},Nothing) instead.",
            "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)},constructorArguments:= Nothing) instead.",
            "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.[For](constructorArguments:= Nothing,typesToProxy:= {GetType(IFoo)}) instead."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(SubstituteConstructorArgumentsForInterfaceDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();
        await VerifyDiagnostic(textParserResult.Text, diagnostics);
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Func(Of Integer))}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(Func(Of Integer))}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Func(Of Integer))})
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Func(Of Integer))}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Func(Of Integer))}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(Func(Of Integer))})|]
        End Sub
    End Class
End Namespace
";
        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.[For]({GetType(Func(Of Integer))},Nothing) instead.",
            "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.[For](typesToProxy:= {GetType(Func(Of Integer))},constructorArguments:= Nothing) instead.",
            "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.[For](constructorArguments:= Nothing,typesToProxy:= {GetType(Func(Of Integer))}) instead."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(SubstituteConstructorArgumentsForDelegateDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();
        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    [Theory]
    [InlineData("{ GetType(Bar), New Foo().[GetType]() }")]
    [InlineData("{ GetType(Bar), New Foo().[GetType]() }.ToArray()")]
    public async Task ReportsNoDiagnostic_WhenProxyTypeCannotBeInfered(string proxyExpression)
    {
        var source = $@"Imports NSubstitute
Imports System.Linq

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim bar = New Bar()
            Dim substitute = NSubstitute.Substitute.[For]({proxyExpression}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {proxyExpression}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {proxyExpression})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleTypeParameters_ContainMultipleClasses()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo), GetType(Bar)}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo), GetType(Bar)}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo), GetType(Bar)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteMultipleClassesDescriptor);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleSameClasses()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo), GetType(Foo)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo), GetType(Foo)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo), GetType(Foo)})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleInterfaces()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Interface IBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo), GetType(IBar)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo), GetType(IBar)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(IFoo), GetType(IBar)})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsNoDiagnostic_WhenMultipleTypeParameters_ContainsInterfaceNotImplementedByClass()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo), GetType(Bar)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo), GetType(Bar)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(IFoo), GetType(Bar)})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [Fact]
    public async Task ReportsDiagnostic_WhenMultipleTypeParameters_ContainsClassWithoutMatchingConstructor()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(IFoo), GetType(Bar)}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo), GetType(Bar)}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(IFoo), GetType(Bar)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.");
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})|]
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})|]
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})|]
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1, 2, 3})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= New Object() {1, 2, 3})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1, 2, 3}, typesToProxy:= {GetType(Foo)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(Foo)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(Foo)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }

    public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
    {
        var source = @"Imports NSubstitute

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(Foo)})|]
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
            Dim substitute = NSubstitute.Substitute.[For]({{GetType(Foo)}}, Nothing)
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {{GetType(Foo)}}, constructorArguments:= Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {{GetType(Foo)}})
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
            Dim substitute = [|NSubstitute.Substitute.[For]({{GetType(Foo)}}, Nothing)|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {{GetType(Foo)}}, constructorArguments:= Nothing)|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {{GetType(Foo)}})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
    }

    [Theory]
    [InlineData("ByVal x As Decimal", "New Object() { 1 }")] // valid c# but doesnt work in NSubstitute
    [InlineData("ByVal x As Integer", "New Object() { 1D }")]
    [InlineData("ByVal x As Integer", "New Object() { 1R }")]
    [InlineData("ByVal x As List(Of Integer)", "New Object() { New List(Of Integer)().AsReadOnly() }")]
    [InlineData("ParamArray z As Integer()", "New Object() { 1D }")]
    [InlineData("ParamArray z As Integer()", "New Object() { 1, 1D }")]
    [InlineData("ParamArray z As Integer()", "{ 1D }")]
    [InlineData("ParamArray z As Integer()", "{ 1, 1D }")]

    // [InlineData("ByVal x As Integer", "New Object()")] This gives runtime error on VB level, not even NSubstitute level (but compiles just fine)
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
            Dim substitute = [|NSubstitute.Substitute.[For]({{GetType(Foo)}}, {invocationValues})|]
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Substitute.[For] do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
    }

    [Theory]
    [InlineData("ByVal x As Integer", "New Object() {1}")]
    [InlineData("ByVal x As Single", "New Object() {\"c\"c}")]
    [InlineData("ByVal x As Integer", "New Object() {\"c\"c}")]
    [InlineData("ByVal x As IList(Of Integer)", "New Object() {New List(Of Integer)()}")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)()}")]
    [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)().AsReadOnly()}")]
    [InlineData("ByVal x As IEnumerable(Of Char)", @"New Object() {""value""}")]
    [InlineData("", @"New Object() {}")]
    [InlineData("", "New Object() {1, 2}.ToArray()")] // actual values known at runtime only so constructor analysys skipped
    [InlineData("ByVal x As Integer, ParamArray z As String()", "New Object() { 1, \"foo\", \"foo\" }")]
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
            Dim substitute = NSubstitute.Substitute.[For]({{GetType(Foo)}}, {invocationValues})
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
            Dim substitute = CType(NSubstitute.Substitute.[For]({GetType(T)}, Nothing), T)
            Dim otherSubstitute = CType(NSubstitute.Substitute.[For](typesToProxy:= {GetType(T)}, constructorArguments:= Nothing), T)
            Dim yetAnotherSubstitute = CType(NSubstitute.Substitute.[For](constructorArguments:= Nothing, typesToProxy:= {GetType(T)}), T)

            Return substitute
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1, 2, 3})
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= New Object() {1, 2, 3})
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= New Object() {1, 2, 3}, typesToProxy:= {GetType(Foo)})
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
            Dim substitute = [|NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})|]
            Dim otherSubstitute = [|NSubstitute.Substitute.[For](typesToProxy:= {GetType(Foo)}, constructorArguments:= New Object() {1})|]
            Dim yetAnotherSubstitute = [|NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(Foo)})|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
    }
}