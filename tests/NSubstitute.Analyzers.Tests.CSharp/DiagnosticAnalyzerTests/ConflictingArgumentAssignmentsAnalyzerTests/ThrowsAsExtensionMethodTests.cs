using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ConflictingArgumentAssignmentsAnalyzerTests;

[CombinatoryData("Throws", "ThrowsAsync", "ThrowsForAnyArgs", "ThrowsAsyncForAnyArgs")]
public class ThrowsAsExtensionMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);

        Task<int> Barr {{ get; }}

        Task<int> this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                {previousCallArgAccess}
                return new Exception();
            }}).AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var configuredCall = substitute.Bar(Arg.Any<int>()).{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }});

            configuredCall.AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(Arg.Any<int>()).{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }}).AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);

        Task<int> Barr {{ get; }}

        Task<int> this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }}).AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);

        Task<int> Barr {{ get; }}

        Task<int> this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }}).AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(Arg.Any<int>()).{method}(callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
                return new Exception();
            }}).AndDoes(callInfo =>
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x);

        Task<int> Barr {{ get; }}

        Task<int> this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(new Exception()).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}