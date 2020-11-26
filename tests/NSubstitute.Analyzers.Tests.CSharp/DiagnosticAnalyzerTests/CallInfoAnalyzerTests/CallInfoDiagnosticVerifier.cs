using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    public abstract class CallInfoDiagnosticVerifier : CSharpDiagnosticVerifier, ICallInfoDiagnosticVerifier
    {
        protected DiagnosticDescriptor CallInfoArgumentOutOfRangeDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentOutOfRange;

        protected DiagnosticDescriptor CallInfoCouldNotConvertParameterAtPositionDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoCouldNotConvertParameterAtPosition;

        protected DiagnosticDescriptor CallInfoCouldNotFindArgumentToThisCallDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoCouldNotFindArgumentToThisCall;

        protected DiagnosticDescriptor CallInfoMoreThanOneArgumentOfTypeDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoMoreThanOneArgumentOfType;

        protected DiagnosticDescriptor CallInfoArgumentIsNotOutOrRefDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentIsNotOutOrRef;

        protected DiagnosticDescriptor CallInfoArgumentSetWithIncompatibleValueDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new CallInfoAnalyzer();

        [CombinatoryTheory]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.Args()[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgTypes()[1] = typeof(int);")]
        [InlineData("substitute.Barr", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute.Barr", "var x = callInfo[1];")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;")]
        [InlineData("substitute.Barr", "var x = callInfo.Args()[1];")]
        [InlineData("substitute.Barr", "callInfo.Args()[1] = 1;")]
        [InlineData("substitute.Barr", "callInfo.ArgTypes()[1] = typeof(int);")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.Args()[1] = 1;")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgTypes()[1] = typeof(int);")]
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo.ArgAt<int>(1)|];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = [|callInfo[1]|];")]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = [|callInfo.Args()[1]|];")]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo.Args()[1]|] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo.ArgTypes()[1]|] = typeof(int);")]
        [InlineData("substitute.Barr", "[|callInfo.ArgAt<int>(1)|];")]
        [InlineData("substitute.Barr", "var x = [|callInfo[1]|];")]
        [InlineData("substitute.Barr", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute.Barr", "var x = [|callInfo.Args()[1]|];")]
        [InlineData("substitute.Barr", "[|callInfo.Args()[1]|] = 1;")]
        [InlineData("substitute.Barr", "[|callInfo.ArgTypes()[1]|] = typeof(int);")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo.ArgAt<int>(1)|];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = [|callInfo[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = [|callInfo.Args()[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo.Args()[1]|] = 1;")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo.ArgTypes()[1]|] = typeof(int);")]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", @"var x = 2; callInfo.ArgAt<int>(x);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", @"var x = 2; var y = callInfo[x];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", @"var x = 2; var y = callInfo.Args()[x];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", @"var x = 2; var y = callInfo.ArgTypes()[x];")]
        [InlineData("substitute.Barr", @"var x = 2; callInfo.ArgAt<int>(x);")]
        [InlineData("substitute.Barr", @"var x = 2; var y = callInfo[x];")]
        [InlineData("substitute.Barr", @"var x = 2; var y = callInfo.Args()[x];")]
        [InlineData("substitute.Barr", @"var x = 2; var y = callInfo.ArgTypes()[x];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", @"var x = 2; callInfo.ArgAt<int>(x);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", @"var x = 2; var y = callInfo[x];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", @"var x = 2; var y = callInfo.Args()[x];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", @"var x = 2; var y = callInfo.ArgTypes()[x];")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.Args()[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.ArgTypes()[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[0];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.Args()[0];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.ArgTypes()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.Args()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.ArgTypes()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.Args()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.ArgTypes()[1];")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (BarBase)callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (object)callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (int)callInfo[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (BarBase)callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (object)callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo[1] as BarBase;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo[1] as object;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo.Args()[1] as BarBase;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo.Args()[1] as object;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (int)callInfo.Args()[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (BarBase)callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (object)callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (BarBase)callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (object)callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo[1] as BarBase;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo[1] as object;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (int)callInfo[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo.Args()[1] as BarBase;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo.Args()[1] as object;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (int)callInfo.Args()[0];")]
        public abstract Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = (Bar)[|callInfo[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = [|callInfo[1]|] as Bar;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = (Bar)[|callInfo.Args()[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = [|callInfo.Args()[1]|] as Bar;")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<int>())", "var x = (Bar)[|callInfo[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<int>())", "var x = [|callInfo[1]|] as Bar;")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<int>())", "var x = (Bar)[|callInfo.Args()[1]|];")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<int>())", "var x = [|callInfo.Args()[1]|] as Bar;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = (Bar)[|callInfo[1]|];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = [|callInfo[1]|] as Bar;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = (Bar)[|callInfo.Args()[1]|];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = [|callInfo.Args()[1]|] as Bar;")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<int>()]", "var x = (Bar)[|callInfo[1]|];")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<int>()]", "var x = [|callInfo[1]|] as Bar;")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<int>()]", "var x = (Bar)[|callInfo.Args()[1]|];")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<int>()]", "var x = [|callInfo.Args()[1]|] as Bar;")]
        public abstract Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "callInfo.ArgAt<BarBase>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "callInfo.ArgAt<object>(1);")]
        [InlineData("substitute.Bar(Arg.Any<decimal>(), Arg.Any<Bar>())", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute.Bar(y: Arg.Any<Bar>(), x: Arg.Any<decimal>())", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute.Bar(Arg.Any<decimal>(), Arg.Any<Bar>())", "callInfo.ArgAt<BarBase>(1);")]
        [InlineData("substitute.Bar(Arg.Any<decimal>(), Arg.Any<Bar>())", "callInfo.ArgAt<object>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "callInfo.ArgAt<object>(0);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "callInfo.ArgAt<BarBase>(1);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "callInfo.ArgAt<object>(1);")]
        [InlineData("substitute[Arg.Any<decimal>(), Arg.Any<Bar>()]", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute[y: Arg.Any<Bar>(), x: Arg.Any<decimal>()]", "callInfo.ArgAt<Bar>(1);")]
        [InlineData("substitute[Arg.Any<decimal>(), Arg.Any<Bar>()]", "callInfo.ArgAt<BarBase>(1);")]
        [InlineData("substitute[Arg.Any<decimal>(), Arg.Any<Bar>()]", "callInfo.ArgAt<object>(1);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "callInfo.ArgAt<object>(0);")]
        public abstract Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "[|callInfo.ArgAt<Bar>(1)|];", "Couldn't convert parameter at position 1 to type MyNamespace.Bar.")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "[|callInfo.ArgAt<int>(1)|];", "Couldn't convert parameter at position 1 to type int.")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "[|callInfo.ArgAt<double>(0)|];", "Couldn't convert parameter at position 0 to type double.")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "[|callInfo.ArgAt<decimal>(0)|];", "Couldn't convert parameter at position 0 to type decimal.")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<double>())", "[|callInfo.ArgAt<Bar>(1)|];", "Couldn't convert parameter at position 1 to type MyNamespace.Bar.")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<double>())", "[|callInfo.ArgAt<int>(1)|];", "Couldn't convert parameter at position 1 to type int.")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<double>())", "[|callInfo.ArgAt<double>(0)|];", "Couldn't convert parameter at position 0 to type double.")]
        [InlineData("substitute.Bar(Arg.Any<string>(), Arg.Any<double>())", "[|callInfo.ArgAt<decimal>(0)|];", "Couldn't convert parameter at position 0 to type decimal.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "[|callInfo.ArgAt<Bar>(1)|];", "Couldn't convert parameter at position 1 to type MyNamespace.Bar.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "[|callInfo.ArgAt<int>(1)|];", "Couldn't convert parameter at position 1 to type int.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "[|callInfo.ArgAt<double>(0)|];", "Couldn't convert parameter at position 0 to type double.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "[|callInfo.ArgAt<decimal>(0)|];", "Couldn't convert parameter at position 0 to type decimal.")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<double>()]", "[|callInfo.ArgAt<Bar>(1)|];", "Couldn't convert parameter at position 1 to type MyNamespace.Bar.")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<double>()]", "[|callInfo.ArgAt<int>(1)|];", "Couldn't convert parameter at position 1 to type int.")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<double>()]", "[|callInfo.ArgAt<double>(0)|];", "Couldn't convert parameter at position 0 to type double.")]
        [InlineData("substitute[Arg.Any<string>(), Arg.Any<double>()]", "[|callInfo.ArgAt<decimal>(0)|];", "Couldn't convert parameter at position 0 to type decimal.")]
        public abstract Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo.ArgTypes() as object;")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo.ArgTypes()[0] as object;")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (object)callInfo.ArgTypes();")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (object)callInfo.ArgTypes()[0];")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo.ArgTypes() as object;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo.ArgTypes()[0] as object;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (object)callInfo.ArgTypes();")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (object)callInfo.ArgTypes()[0];")]
        public abstract Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "callInfo.ArgTypes()[0] = typeof(object);")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "callInfo.Args()[0] = 1m;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "callInfo.ArgTypes()[0] = typeof(object);")]
        [InlineData("substitute[Arg.Any<Bar>()]", "callInfo.Args()[0] = 1m;")]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Barr", "[|callInfo.Arg<double>()|];", "Can not find an argument of type double to this call.")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo.Arg<double>()|];", "Can not find an argument of type double to this call.")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "[|callInfo.Arg<long>()|];", "Can not find an argument of type long to this call.")]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo.Arg<double>()|];", "Can not find an argument of type double to this call.")]
        [InlineData("substitute[Arg.Any<int>()]", "[|callInfo.Arg<long>()|];", "Can not find an argument of type long to this call.")]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.Arg<int>();")]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.Arg<object>();")]
        [InlineData("substitute.Bar(Arg.Any<Foo>())", "callInfo.Arg<FooBase>();")]
        [InlineData("substitute.Bar(Arg.Any<Foo>())", "callInfo.Arg<object>();")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Foo>())", "callInfo.Arg<FooBase>();")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Foo>())", "callInfo.Arg<Foo>();")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Foo>())", "callInfo.Arg<object>();")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.Arg<int>();")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.Arg<object>();")]
        [InlineData("substitute[Arg.Any<Foo>()]", "callInfo.Arg<FooBase>();")]
        [InlineData("substitute[Arg.Any<Foo>()]", "callInfo.Arg<object>();")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Foo>()]", "callInfo.Arg<FooBase>();")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Foo>()]", "callInfo.Arg<Foo>();")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Foo>()]", "callInfo.Arg<object>();")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "[|callInfo.Arg<int>()|];", "There is more than one argument of type int to this call.")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        [InlineData("substitute.Bar(Arg.Any<FooBar>(), Arg.Any<FooBar>())", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        [InlineData("substitute.Bar(Arg.Any<FooBar>(), Arg.Any<FooBar>())", "[|callInfo.Arg<FooBar>()|];", "There is more than one argument of type MyNamespace.FooBar to this call.")]
        [InlineData("substitute.Bar(Arg.Any<object>(), Arg.Any<FooBar>())", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "[|callInfo.Arg<int>()|];", "There is more than one argument of type int to this call.")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        [InlineData("substitute[Arg.Any<FooBar>(), Arg.Any<FooBar>()]", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        [InlineData("substitute[Arg.Any<FooBar>(), Arg.Any<FooBar>()]", "[|callInfo.Arg<FooBar>()|];", "There is more than one argument of type MyNamespace.FooBar to this call.")]
        [InlineData("substitute[Arg.Any<object>(), Arg.Any<FooBar>()]", "[|callInfo.Arg<object>()|];", "There is more than one argument of type object to this call.")]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "callInfo.Arg<int>();")]
        [InlineData("substitute.Bar(Arg.Any<object>(), Arg.Any<FooBar>())", "callInfo.Arg<object>();")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "callInfo.Arg<int>();")]
        [InlineData("substitute[Arg.Any<object>(), Arg.Any<FooBar>()]", "callInfo.Arg<object>();")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]")]
        public abstract Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method);

        [CombinatoryTheory]
        [InlineData("decimal", "1", "Could not set value of type int to argument 0 (decimal) because the types are incompatible.")]
        [InlineData("string", "new object()", "Could not set value of type object to argument 0 (string) because the types are incompatible.")]
        [InlineData("int", "'1'", "Could not set value of type char to argument 0 (int) because the types are incompatible.")]
        [InlineData("int", "1M", "Could not set value of type decimal to argument 0 (int) because the types are incompatible.")]
        [InlineData("char", "1", "Could not set value of type int to argument 0 (char) because the types are incompatible.")]
        [InlineData("List<object>", "new List<object>().AsReadOnly()", "Could not set value of type System.Collections.ObjectModel.ReadOnlyCollection<object> to argument 0 (System.Collections.Generic.List<object>) because the types are incompatible.")]
        [InlineData("List<object>", "new object[] { new object() }", "Could not set value of type object[] to argument 0 (System.Collections.Generic.List<object>) because the types are incompatible.")]
        public abstract Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage);

        [CombinatoryTheory]
        [InlineData("object", @"""string""")]
        [InlineData("int", "1")]
        [InlineData("int?", "1")]
        [InlineData("decimal", "1M")]
        [InlineData("double", "1D")]
        [InlineData("IEnumerable<object>", "new List<object>()")]
        [InlineData("IDictionary<string, object>", "new Dictionary<string, object>()")]
        [InlineData("IReadOnlyDictionary <string, object>", "new Dictionary<string, object>()")]
        public abstract Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right);
    }
}