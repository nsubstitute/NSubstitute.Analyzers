using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReceivedAsExtensionMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
            substitute.{method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
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
            substitute.{method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
            substitute.{method}.Foo = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
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
            substitute.{method}.Foo = {arg};
            substitute.{method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
            substitute.{method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
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
            substitute.{method}[1] = {arg};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
            substitute.{method}[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
            substitute.{method}.Foo = {arg};
            substitute.{method}[1] = {arg};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string receivedMethod, string arg)
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
                substitute.{receivedMethod}.Bar({arg});
            }}
        }}
    }}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
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
                substitute.{method}.Bar({arg});
            }}
        }}
    }}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
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
                substitute.{method}.Bar({arg});
            }}
        }}
    }}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}