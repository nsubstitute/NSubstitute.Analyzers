using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReceivedInReceivedInOrderAnalyzerTests;

[CombinatoryData(
    "ReceivedExtensions.Received(substitute, Quantity.None())",
    "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
    "ReceivedExtensions.Received<IFoo>(substitute, Quantity.None())",
    "ReceivedExtensions.Received<IFoo>(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.Received<IFoo>(requiredQuantity: Quantity.None(), substitute: substitute)",
    "SubstituteExtensions.Received(substitute)",
    "SubstituteExtensions.Received(substitute: substitute)",
    "SubstituteExtensions.Received<IFoo>(substitute)",
    "SubstituteExtensions.Received<IFoo>(substitute: substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs<IFoo>(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs<IFoo>(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs<IFoo>(requiredQuantity: Quantity.None(), substitute: substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<IFoo>(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<IFoo>(substitute: substitute)",
    "SubstituteExtensions.DidNotReceive(substitute)",
    "SubstituteExtensions.DidNotReceive(substitute: substitute)",
    "SubstituteExtensions.DidNotReceive<IFoo>(substitute)",
    "SubstituteExtensions.DidNotReceive<IFoo>(substitute: substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<IFoo>(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<IFoo>(substitute: substitute)")]
public class ReceivedAsOrdinaryMethodTests : ReceivedInReceivedInOrderDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            {{ 
                [|{method}|].Bar(); 
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForProperty(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            {{ 
                _ = [|{method}|].Bar; 
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForIndexer(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            Received.InOrder(() =>
            {{ 
                _ = [|{method}|][0]; 
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsNoDiagnostic_WhenUsingReceivedLikeMethodOutsideOfReceivedInOrderBlock(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();

        int Foo {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}.Bar();
            _ = {method}[0];
            _ = {method}.Foo;

            Received.InOrder(() =>
            {{ 
                _ = substitute[0]; 
                _ = substitute.Foo; 
                substitute.Bar();
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.Received(x: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo>(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.Received<Foo>(x: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute, 1, 1)",
        "SubstituteExtensions.Received(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.Received(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.Received<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.Received<Foo>(x: 1, y: 1, substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(x: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(x: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceive(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
    {
        var source = $@"

namespace NSubstitute
{{
    public class Quantity
    {{
        public static Quantity None() => null;
    }}

    public class Foo
    {{
        public int Bar()
        {{
            return 1;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}
    }}
    
    public static class ReceivedExtensions
    {{
        public static T Received<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            Received.InOrder(() => 
            {{
                {method}.Bar();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    private static string GetPlainMethodName(string methodName)
    {
        var plainMethodName = methodName.Replace("<IFoo>", string.Empty)
            .Replace("(substitute, Quantity.None())", string.Empty)
            .Replace("(substitute: substitute, requiredQuantity: Quantity.None())", string.Empty)
            .Replace("(requiredQuantity: Quantity.None(), substitute: substitute)", string.Empty)
            .Replace("(substitute: substitute)", string.Empty)
            .Replace("(substitute)", string.Empty);

        var planMethodNameWithoutNamespace =
            plainMethodName.Replace("ReceivedExtensions.", string.Empty)
                .Replace("SubstituteExtensions.", string.Empty);

        return planMethodNameWithoutNamespace;
    }

    private async Task VerifyDiagnostic(string source, string methodName)
    {
        var plainMethodName = GetPlainMethodName(methodName);

        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName} method used in Received.InOrder block.");
    }
}