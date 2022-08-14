using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReceivedAsOrdinaryMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Action<int> Foo {{ get; set; }}
        Action<int> this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}.Foo = {arg};
            {method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Action<int> Foo {{ get; set; }}
        Action<int> this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}.Foo = {arg};
            {method}[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        void Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            {method}.Bar({arg});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        void Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            {method}.Bar({arg});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        void Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            {method}.Bar({arg});
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}
