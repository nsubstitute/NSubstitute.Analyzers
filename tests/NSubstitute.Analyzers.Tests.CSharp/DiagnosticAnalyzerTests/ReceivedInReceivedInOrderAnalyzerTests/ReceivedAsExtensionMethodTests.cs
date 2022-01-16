using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReceivedInReceivedInOrderAnalyzerTests;

[CombinatoryData(
    "Received(Quantity.None())",
    "Received<IFoo>(Quantity.None())",
    "Received()",
    "Received<IFoo>()",
    "ReceivedWithAnyArgs(Quantity.None())",
    "ReceivedWithAnyArgs<IFoo>(Quantity.None())",
    "ReceivedWithAnyArgs()",
    "ReceivedWithAnyArgs<IFoo>()",
    "DidNotReceive()",
    "DidNotReceive<IFoo>()",
    "DidNotReceiveWithAnyArgs()",
    "DidNotReceiveWithAnyArgs<IFoo>()")]
public class ReceivedAsExtensionMethodTests : ReceivedInReceivedInOrderDiagnosticVerifier
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
                [|substitute.{method}|].Bar(); 
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
                _ = [|substitute.{method}|].Bar; 
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
                _ = [|substitute.{method}|][0]; 
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
            substitute.{method}.Bar();
            _ = substitute.{method}[0];
            _ = substitute.{method}.Foo;

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
        "Received(Quantity.None())",
        "Received<Foo>(Quantity.None())",
        "Received(1, 1)",
        "Received<Foo>(1, 1)",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Foo>(Quantity.None())",
        "ReceivedWithAnyArgs(1, 1)",
        "ReceivedWithAnyArgs<Foo>(1, 1)",
        "DidNotReceive(1, 1)",
        "DidNotReceive<Foo>(1, 1)",
        "DidNotReceiveWithAnyArgs(1, 1)",
        "DidNotReceiveWithAnyArgs<Foo>(1, 1)")]
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
                substitute.{method}.Bar();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    private static string GetPlainMethodName(string methodName)
    {
        return methodName.Replace("<IFoo>", string.Empty)
            .Replace("Quantity.None()", string.Empty)
            .Replace("()", string.Empty);
    }

    private async Task VerifyDiagnostic(string source, string methodName)
    {
        var plainMethodName = GetPlainMethodName(methodName);

        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName} method used in Received.InOrder block.");
    }
}