using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    [CombinatoryData("AndDoes")]
    public class AndDoesMethodPrecededByExtensionMethodTests : CallInfoDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess)
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
            var returnedValue = {call}.Returns(1);
            returnedValue.{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess)
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
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int Barr {{ get; }}

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int Barr {{ get; }}

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, Bar y);

        int this[int x, Bar y] {{ get; }}
    }}

    public class BarBase
    {{
    }}

    public class Bar : BarBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[int x, FooBar bar] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooBar : Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, "Couldn't convert parameter at position 1 to type MyNamespace.Bar.");
        }

        public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, Bar y);

        int this[int x, Bar y] {{ get; }}
    }}

    public class BarBase
    {{
    }}

    public class Bar : BarBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[int x, FooBar bar] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooBar : Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, message);
        }

        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string callInfo, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {callInfo}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message)
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
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, message);
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);

        int Bar(Foo x);

        int this[int x] {{ get; }}

        int this[Foo x] {{ get; }}
    }}

    public class FooBase
    {{
    }}

    public class Foo : FooBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoMoreThanOneArgumentOfTypeDescriptor, message);
        }

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                callInfo.Arg<int>();
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.Returns(1).{method}(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoArgumentIsNotOutOrRefDescriptor, "Could not set argument 1 (double) as it is not an out or ref argument.");
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(ref int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(ref value).Returns(1).{method}(callInfo =>
            {{
                callInfo[0] = 1;
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).Returns(1).{method}(callInfo =>
            {{
                callInfo[0] = 1;
            }});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).Returns(1).{method}(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
        }

        public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage)
        {
            var source = $@"using NSubstitute;
using System.Collections.Generic;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).Returns(1).{method}(callInfo =>
            {{
                [|callInfo[0]|] = {right};
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, CallInfoArgumentSetWithIncompatibleValueDescriptor, expectedMessage);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right)
        {
            var source = $@"using NSubstitute;
using System.Collections.Generic;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).Returns(1).{method}(callInfo =>
            {{
                callInfo[0] = {right};
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}