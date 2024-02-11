using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests;

[CombinatoryData("Throws", "ThrowsForAnyArgs", "ThrowsAsync", "ThrowsAsyncForAnyArgs")]
public class ThrowsAsExtensionMethodTests : CallInfoDiagnosticVerifier
{
    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess)
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
            var returnedValue = {call};
            returnedValue.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess)
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
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, int y);

        Task<int> Barr {{ get; }}

        Task<int> this[int x, int y] {{ get; }}
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
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, int y = 1);

        Task<int> Barr {{ get; }}

        Task<int> this[int x, int y = 1] {{ get; }}
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
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar(int x);
    }}

    public interface IFooBar
    {{
        Task<int> FooBaz(int x, int y);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            substitute.FooBaz(Arg.Any<int>(), Arg.Any<int>())
                .{method}(outerCallInfo =>
                {{
                    var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                    otherSubstitute.Bar(Arg.Any<int>()).{method}(innerCallInfo =>
                    {{
                        var x = outerCallInfo.ArgAt<int>(1);
                        var y = outerCallInfo[1];

                        var xx = innerCallInfo.ArgAt<int>(0);
                        var yy = innerCallInfo[0];

                        return new Exception();
                    }});

                    return new Exception();
                }});

        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, Bar y);

        Task<int> Bar(int x, object y);

        Task<int> this[int x, Bar y] {{ get; }}

        Task<int> this[int x, object y] {{ get; }}
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
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, double y);
    
        Task<int> Bar(string x, object y);

        Task<int> Foo(int x, FooBar bar);

        Task<int> this[int x, double y] {{ get; }}

        Task<int> this[string x, object y] {{ get; }}

        Task<int> this[int x, FooBar bar] {{ get; }}
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
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, "Couldn't convert parameter at position 1 to type MyNamespace.Bar.");
    }

    public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, Bar y);

        Task<int> Bar(decimal x, object y, int z = 1);

        Task<int> this[int x, Bar y] {{ get; }}

        Task<int> this[decimal x, object y] {{ get; }}
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
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, double y);

        Task<int> Bar(object x, object y);

        Task<int> Foo(int x, FooBar bar);

        Task<int> this[int x, double y] {{ get; }}

        Task<int> this[object x, object y] {{ get; }}

        Task<int> this[int x, FooBar bar] {{ get; }}
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
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string callInfo, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(Bar x);

        Task<int> this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {callInfo}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(Bar x);

        Task<int> this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
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
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message)
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
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, message);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar(int x);
    }}

    public interface IFooBar
    {{
        Task<int> FooBaz(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            substitute.FooBaz(Arg.Any<int>())
                      .{method}(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).{method}(innerCallInfo =>
                {{
                     var x = [|outerCallInfo.Arg<string>()|];
                     var y = [|innerCallInfo.Arg<string>()|];

                     return new Exception();
                }});

                return new Exception();
            }});

        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, "Can not find an argument of type string to this call.");
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar(int x);
    }}

    public interface IFooBar
    {{
        Task<int> FooBaz(int x, int y);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            substitute.FooBaz(Arg.Any<int>(), Arg.Any<int>())
                      .{method}(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).{method}(innerCallInfo =>
                {{
                     var x = [|outerCallInfo.ArgAt<int>(2)|];
                     var y = [|outerCallInfo[2]|];
                     var z = outerCallInfo[1];

                     var xx = [|innerCallInfo.ArgAt<int>(1)|];
                     var yy = [|innerCallInfo[1]|];
                     var zz = innerCallInfo[0];

                     return new Exception();
                }});

                return new Exception();
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
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar(int x);

        Task<int> Bar(Foo x);

        Task<int> Bar(int x, object y);

        Task<int> this[int x] {{ get; }}

        Task<int> this[Foo x] {{ get; }}

        Task<int> this[int x, object y] {{ get; }}
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
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return new Exception();
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar(int x);
    }}

    public interface IFooBar
    {{
        Task<int> FooBaz(string x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFooBar>();
            substitute.FooBaz(Arg.Any<string>())
                      .{method}(outerCallInfo =>
            {{
                var otherSubstitute = NSubstitute.Substitute.For<IFoo>();
                otherSubstitute.Bar(Arg.Any<int>()).{method}(innerCallInfo =>
                {{
                     var x = outerCallInfo.Arg<string>();
                     var y = innerCallInfo.Arg<int>();
                     return new Exception();
                }});

                return new Exception();
            }});

        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, int y);

        Task<int> Bar(object x, object y);
        
        Task<int> this[int x, int y] {{ get; }}

        Task<int> this[object x, object y] {{ get; }}
    }}

    public class FooBar
    {{
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
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, CallInfoMoreThanOneArgumentOfTypeDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, double y);

        Task<int> Bar(object x, FooBar y);

        Task<int> this[int x, double y] {{ get; }}

        Task<int> this[object x, FooBar y] {{ get; }}
    }}

    public class FooBar
    {{
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
            }});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(int x, double y);

        Task<int> this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                [|callInfo[1]|] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoArgumentIsNotOutOrRefDescriptor, "Could not set argument 1 (double) as it is not an out or ref argument.");
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(ref int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(ref value).{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(out int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).{method}(callInfo =>
            {{
                [|callInfo[1]|] = 1;
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).{method}(callInfo =>
            {{
                [|callInfo[0]|] = {right};
                return new Exception();
            }});
        }}
    }}
}}";

        await VerifyDiagnostic(source, CallInfoArgumentSetWithIncompatibleValueDescriptor, expectedMessage);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using System.Collections.Generic;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task<int> Bar(out {left} x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {left} value = default({left});
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).{method}(callInfo =>
            {{
                callInfo[0] = {right};
                return new Exception();
            }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}