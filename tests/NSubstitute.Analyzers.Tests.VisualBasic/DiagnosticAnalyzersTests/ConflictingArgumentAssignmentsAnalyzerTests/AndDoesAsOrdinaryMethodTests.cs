using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests
{
    [CombinatoryData("AndDoes")]
    public class AndDoesAsOrdinaryMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
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
            {call}.Returns(1).{method}(callInfo =>
            {{
                {previousCallArgAccess}
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor);
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
            {call}.Returns(1).{method}(callInfo =>
            {{
                callInfo[0] = 1;
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
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
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
            substitute.Bar(Arg.Any<int>()).Returns(1).{method}(callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
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
            {call}.Returns(1).{method}(_ => {{}}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}