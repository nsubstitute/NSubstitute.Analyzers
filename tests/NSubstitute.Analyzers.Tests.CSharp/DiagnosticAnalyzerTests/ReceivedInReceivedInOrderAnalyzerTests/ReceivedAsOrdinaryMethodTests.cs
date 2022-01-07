using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReceivedInReceivedInOrderAnalyzerTests;

[CombinatoryData(
    "ReceivedExtensions.Received(substitute, Quantity.None())",
    "ReceivedExtensions.Received<IFoo>(substitute, Quantity.None())",
    "SubstituteExtensions.Received(substitute)",
    "SubstituteExtensions.Received<IFoo>(substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs<IFoo>(substitute, Quantity.None())",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<IFoo>(substitute)",
    "SubstituteExtensions.DidNotReceive(substitute)",
    "SubstituteExtensions.DidNotReceive<IFoo>(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<IFoo>(substitute)")]
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
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.Received(substitute, 1, 1)",
        "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)")]
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