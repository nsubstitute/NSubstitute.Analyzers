using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<int>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<int>")]
    public class ReturnsAsOrdinaryMethodTests : ReEntrantReturnsSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string method, string reEntrantCall)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|ReturnThis()|], [|OtherReturn()|]);
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{plainMethodName}() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
                $"{plainMethodName}() is set with a method that itself calls Returns. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string method, string reEntrantCall)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|ReturnThis()|], [|OtherReturn()|]);
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{plainMethodName}() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
                $"{plainMethodName}() is set with a method that itself calls ReturnsForAnyArgs. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string method, string reEntrantCall)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|ReturnThis()|], [|OtherReturn()|]);
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}
    }}
}}";
            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{plainMethodName}() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
                $"{plainMethodName}() is set with a method that itself calls Do. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_ForNestedReEntrantCall(string method)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|ReturnThis()|], [|OtherReturn()|]);
        }}

        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {method}(substitute.Foo(), [|NestedReturnThis()|]);
            return 1;
        }}

        private int NestedReturnThis()
        {{
            return OtherNestedReturnThis();
        }}

        private int OtherNestedReturnThis()
        {{
            var sub = Substitute.For<IBar>();
            {method}(sub.Foo(), 1);
            return 1;
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
                $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn()).",
                $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => NestedReturnThis())."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsDiagnostic_ForSpecificNestedReEntrantCall(string method)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), x => ReturnThis());
        }}

        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {method}(substitute.Foo(), [|NestedReturnThis()|]);
            return 1;
        }}

        private int NestedReturnThis()
        {{
            return OtherNestedReturnThis();
        }}

        private int OtherNestedReturnThis()
        {{
            var sub = Substitute.For<IBar>();
            {method}(sub.Foo(), 1);
            return 1;
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => NestedReturnThis()).");
        }

        [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<IBar>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<IBar>")]
        public override async Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string method, string localVariable)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        IBar Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {localVariable}
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), bar);
        }}

        public IBar Bar()
        {{
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(1);
            return substitute;
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string method, string rootCall, string reEntrantCall)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), {rootCall});
        }}

        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}

        private int ReturnThisWithCallInfo(CallInfo info)
        {{
            return OtherReturn();
        }}

        Func<CallInfo, int> MyMethod()
        {{
            return ReturnThisWithCallInfo;
        }}

        Func<CallInfo, int> MyProperty
        {{
            get {{ return ReturnThisWithCallInfo; }}
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string method, string rootCall, string reEntrantCall)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), {rootCall});
        }}

        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            {reEntrantCall}
            return 1;
        }}

        private int ReturnThisWithCallInfo(CallInfo info)
        {{
            return OtherReturn();
        }}

        Func<CallInfo, int> MyMethod()
        {{
            return ReturnThisWithCallInfo;
        }}

        Func<CallInfo, int> MyProperty
        {{
            get {{ return ReturnThisWithCallInfo; }}
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string method, string firstReturn, string secondReturn)
        {
            var source = $@"using NSubstitute;
using NSubstitute.Core;

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
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), {firstReturn}, {secondReturn});
        }}


        private int ReturnThis()
        {{
            return OtherReturn();
        }}

        private int OtherReturn()
        {{
           return 1;
        }}

        private int ReturnThis(CallInfo info)
        {{
            return OtherReturn(info);
        }}

        private int OtherReturn(CallInfo info)
        {{
           return 1;
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles(string method)
        {
            var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public interface IBar
    {{
        int Foo();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|FooBar.ReturnThis()|]);
        }}
    }}
}}";

            var secondSource = $@"
using NSubstitute;

namespace MyNamespace
{{
    public class FooBar
    {{
        public static int ReturnThis()
        {{
            var substitute = Substitute.For<IBar>();
            {method}(substitute.Foo(), 1);
            return 1;
        }}
    }}
}}";

            await VerifyDiagnostics(new[] { source, secondSource }, Descriptor, $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => FooBar.ReturnThis()).");
        }

        [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<object>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<object>")]
        public override async Task ReportsNoDiagnostic_WhenUsed_WithTypeofExpression(string method, string type)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public object FooBar()
        {{
            return null;
        }}

        public object Bar()
        {{
            var sub = Substitute.For<Foo>();
            sub.FooBar().Returns(1);
            return null;
        }}
    }}

    public struct FooBar
    {{
        public object Bar()
        {{
            var sub = Substitute.For<Foo>();
            sub.FooBar().Returns(1);
            return null;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.FooBar(), typeof({type}));
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }
    }
}