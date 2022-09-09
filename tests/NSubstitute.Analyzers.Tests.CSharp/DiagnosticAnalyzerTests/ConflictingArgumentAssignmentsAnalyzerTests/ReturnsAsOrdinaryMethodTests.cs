using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ConflictingArgumentAssignmentsAnalyzerTests;

[CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<int>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<int>")]
public class ReturnsAsOrdinaryMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo => 1,
            callInfo =>
            {{
                {previousCallArgAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(value: {call}, returnThis: callInfo => 1,
            returnThese: callInfo =>
            {{
                {previousCallArgAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(returnThese: callInfo =>
            {{
                {previousCallArgAccess}
                return 1;
            }}, value: {call},
            returnThis: callInfo => 1).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, Descriptor);
    }

    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var configuredCall = {method}(substitute.Bar(Arg.Any<int>()), callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }});

            var otherCall = {method}(value: substitute.Bar(Arg.Any<int>()), returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }});

            var yetAnotherCall = {method}(returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}, value: substitute.Bar(Arg.Any<int>()));


            configuredCall.AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});

            otherCall.AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});

            yetAnotherCall.AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(Arg.Any<int>()), callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }}, callInfo => {{}});
            {method}(value: substitute.Bar(Arg.Any<int>()), returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }}, callInfo => {{}});
            {method}(returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}, value: substitute.Bar(Arg.Any<int>())).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }}, callInfo => {{}});
        }}
    }}

    public static class ConfiguredCallExtensions
    {{
        public static void AndDoes(this ConfiguredCall call, Action<CallInfo> firstCall, Action<CallInfo> secondCall)
        {{
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(value: {call}, returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(returnThis: callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}, value: {call}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, callInfo =>
            {{
                {argAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {argAccess}
            }});

            {method}(value: {call}, returnThis: callInfo =>
            {{
                {argAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {argAccess}
            }});

            {method}(returnThis: callInfo =>
            {{
                {argAccess}
                return 1;
            }}, value: {call}).AndDoes(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(Arg.Any<int>()), callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});

            {method}(value: substitute.Bar(Arg.Any<int>()), returnThis: callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});

            {method}(returnThis: callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
                return 1;
            }}, value: substitute.Bar(Arg.Any<int>())).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}({call}, 1).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(value: {call}, returnThis: 1).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});

            {method}(returnThis: 1, value: {call}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}