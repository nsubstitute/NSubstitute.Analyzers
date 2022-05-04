using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests;

[CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.WhenForAnyArgs")]
public class DoMethodPrecededByOrdinaryMethodTests : CallInfoDiagnosticVerifier
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
            var sub = NSubstitute.Substitute.For<Foo>();
            var returnValue = {method}(sub, substitute => {{var _ = {call}; }});
            var otherValue = {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }});
            var yetAnotherValue = {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub);
            returnValue.Do(callInfo =>
            {{
                {argAccess}
            }});
            otherValue.Do(callInfo =>
            {{
                {argAccess}
            }});
            yetAnotherValue.Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
        int Bar(int x, int y = 1);

        int Barr {{ get; }}

        int this[int x, int y = 1] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);
    }}

    public interface IFooBar
    {{
        int FooBaz(int x, int y);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            {method}(substitute, sub => sub.FooBaz(Arg.Any<int>(), Arg.Any<int>())).Do(outerCallInfo =>
                {{
                    var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                    otherSubstitute.Bar(Arg.Any<int>()).Returns(innerCallInfo =>
                    {{
                        var x = outerCallInfo.ArgAt<int>(1);
                        var y = outerCallInfo[1];

                        var xx = innerCallInfo.ArgAt<int>(0);
                        var yy = innerCallInfo[0];

                        return 1;
                    }});
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
    
        int Bar(string x, object y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[string x, object y] {{ get; }}

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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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

        int Bar(decimal x, object y, int z = 1);

        int this[int x, Bar y] {{ get; }}

        int this[decimal x, object y] {{ get; }}
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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

        int Bar(object x, object y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[object x, object y] {{ get; }}

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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call, string argAccess)
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, message);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);
    }}

    public interface IFooBar
    {{
        void FooBaz(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            {method}(substitute, sub => sub.FooBaz(Arg.Any<int>())).Do(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).Returns(innerCallInfo =>
                {{
                     var x = [|outerCallInfo.Arg<string>()|];
                     var y = [|innerCallInfo.Arg<string>()|];

                     return 1;
                }});
            }});

        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, "Can not find an argument of type string to this call.");
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);
    }}

    public interface IFooBar
    {{
        void FooBaz(int x, int y);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            {method}(substitute, sub => sub.FooBaz(Arg.Any<int>(), Arg.Any<int>())).Do(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).Returns(innerCallInfo =>
                {{
                     var x = [|outerCallInfo.ArgAt<int>(2)|];
                     var y = [|outerCallInfo[2]|];
                     var z = outerCallInfo[1];

                     var xx = [|innerCallInfo.ArgAt<int>(1)|];
                     var yy = [|innerCallInfo[1]|];
                     var zz = innerCallInfo[0];

                     return 1;
                }});
            }});

        }}
    }}
}}";
        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "There is no argument at position 2",
            "There is no argument at position 2",
            "There is no argument at position 1",
            "There is no argument at position 1"
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(CallInfoArgumentOutOfRangeDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();
        await VerifyDiagnostic(textParserResult.Text, diagnostics);
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

        int Bar(int x, object y);

        int this[int x] {{ get; }}

        int this[Foo x] {{ get; }}

        int this[int x, object y] {{ get; }}
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
            var sub = NSubstitute.Substitute.For<IFoo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(
        string method)
    {
        var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar(int x);
    }}

    public interface IFooBar
    {{
        void FooBaz(string x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            {method}(substitute, sub => sub.FooBaz(Arg.Any<string>())).Do(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).Returns(innerCallInfo =>
                {{
                     var x = outerCallInfo.Arg<string>();
                     return innerCallInfo.Arg<int>();
                }});
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

        int Bar(object x, object y);
        
        int this[int x, int y] {{ get; }}

        int this[object x, object y] {{ get; }}
    }}

    public class FooBar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, CallInfoMoreThanOneArgumentOfTypeDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Bar(object x, FooBar y);

        int this[int x, double y] {{ get; }}

        int this[object x, FooBar y] {{ get; }}
    }}

    public class FooBar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                {argAccess}
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
            {{
                {argAccess}
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = {call}; }}).Do(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
            {method}(substituteCall: substitute => {{var _ = {call}; }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = substitute.Bar(ref value); }}).Do(callInfo =>
            {{
                callInfo[0] = 1;
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = substitute.Bar(ref value); }}).Do(callInfo =>
            {{
                callInfo[0] = 1;
            }});
            {method}(substituteCall: substitute => {{var _ = substitute.Bar(ref value); }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                callInfo[0] = 1;
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                callInfo[0] = 1;
            }});
            {method}(substituteCall: substitute => {{var _ = substitute.Bar(out value); }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();
            {method}(sub, substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                [|callInfo[1]|] = 1;
            }});
            {method}(substituteCall: substitute => {{var _ = substitute.Bar(out value); }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();

            {method}(sub, substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                [|callInfo[0]|] = {right};
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                [|callInfo[0]|] = {right};
            }});
            {method}(substituteCall: substitute => {{var _ = substitute.Bar(out value); }}, substitute: sub).Do(callInfo =>
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
            var sub = NSubstitute.Substitute.For<Foo>();

            {method}(sub, substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                callInfo[0] = {right};
            }});
            {method}(substitute: sub, substituteCall: substitute => {{var _ = substitute.Bar(out value); }}).Do(callInfo =>
            {{
                callInfo[0] = {right};
            }});
            {method}(substituteCall: substitute => {{var _ = substitute.Bar(out value); }}, substitute: sub).Do(callInfo =>
            {{
                callInfo[0] = {right};
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}