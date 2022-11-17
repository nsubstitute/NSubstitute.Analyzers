using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests;

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
            {method}(value: substitute.Bar(), returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|]);
            {method}(returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|], value: substitute.Bar());
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
        }.Repeat(3).ToList();

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

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
            {method}(value: substitute.Bar(), returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|]);
            {method}(returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|], value: substitute.Bar());
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
        }.Repeat(3).ToList();

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

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
            {method}(value: substitute.Bar(), returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|]);
            {method}(returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|], value: substitute.Bar());
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
        }.Repeat(3).ToList();

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

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
            {method}(value: substitute.Bar(), returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|]);
            {method}(returnThis: [|ReturnThis()|], returnThese: [|OtherReturn()|], value: substitute.Bar());
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
            $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
            $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn()).",
            $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).",
            $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => OtherReturn()).",
            $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => NestedReturnThis())."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

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
            {method}(value: substitute.Bar(), returnThis: x => ReturnThis());
            {method}(returnThis: x => ReturnThis(), value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: bar);
            {method}(returnThis: bar, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: {rootCall});
            {method}(returnThis: {rootCall}, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: {rootCall});
            {method}(returnThis: {rootCall}, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: {firstReturn}, returnThese: {secondReturn});
            {method}(returnThis: {firstReturn}, returnThese: {secondReturn}, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: [|FooBar.ReturnThis()|]);
            {method}(returnThis: [|FooBar.ReturnThis()|], value: substitute.Bar());
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

    public override async Task ReportsDiagnostic_WhenUsingReEntrantReturns_InAsyncMethod(string method)
    {
        var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

        var source = $@"using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public class FooTests
    {{
        public async Task Test()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), [|await ReturnThis()|]);
            {method}(value: substitute.Bar(), returnThis: [|await ReturnThis()|]);
            {method}(returnThis: [|await ReturnThis()|], value: substitute.Bar());
        }}

        private async Task<int> ReturnThis()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), 1);
            return await Task.FromResult(1);
        }}
    }}
}}";

        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => await ReturnThis()).");
    }

    public override async Task ReportsDiagnostic_WhenUsingReEntrantReturnsIn_InParamsArray(string method, string reEntrantArrayCall)
    {
        var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

        var source = $@"using System.Threading.Tasks;
using NSubstitute;

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
            {method}(substitute.Bar(), 1, {reEntrantArrayCall});
            {method}(value: substitute.Bar(), returnThis: 1, returnThese: {reEntrantArrayCall});
            {method}(returnThis: 1, returnThese: {reEntrantArrayCall}, value: substitute.Bar());
        }}

        private int ReturnThis()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), 1);
            return 1;
        }}

        private int CreateDefaultValue()
        {{
            return 1;
        }}
    }}
}}";

        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls {plainMethodName}. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => ReturnThis()).");
    }

    public override async Task ReportsNoDiagnostic_WhenUsingReEntrantReturnsIn_AndParamArrayIsNotCreatedInline(string method)
    {
        var source = $@"using System.Threading.Tasks;
using NSubstitute;

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
            var array = new[] {{ ReturnThis() }};
            {method}(substitute.Bar(), 1, array);
            {method}(value: substitute.Bar(), returnThis: 1, returnThese: array);
            {method}(returnThis: 1, returnThese: array, value: substitute.Bar());
        }}

        private int ReturnThis()
        {{
            var substitute = Substitute.For<IFoo>();
            {method}(substitute.Bar(), 1);
            return 1;
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
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
            {method}(value: substitute.FooBar(), returnThis: typeof({type}));
            {method}(returnThis: typeof({type}), value: substitute.FooBar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenReturnsValueIsSet_InForEachLoop(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar();
    }}

    public class FooBar
    {{
        public int Value {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            foreach (var fooBar in new FooBar[0])
            {{
                {method}(substitute.Bar(), fooBar.Value);
                {method}(value: substitute.Bar(), returnThis: fooBar.Value);
                {method}(returnThis: fooBar.Value, value: substitute.Bar());
            }}
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenElementUsedTwice_InForEachLoop(string method)
    {
        var source = $@"using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace MyNamespace
{{
    public class FooTests
    {{
        private IEnumerator<int> firstEnumerator = Substitute.For<IEnumerator<int>>();
        private IEnumerator<int> secondEnumerator = Substitute.For<IEnumerator<int>>();
        
        public void Test()
        {{
            var thirdEnumerator = Substitute.For<IEnumerator<int>>();
            var fourthEnumerator = Substitute.For<IEnumerator<int>>();
            foreach (var value in Enumerable.Empty<int>())
            {{
                {method}(firstEnumerator.Current, value + 1);
                {method}(value: firstEnumerator.Current, returnThis: value + 1);
                {method}(returnThis: value + 1, value: firstEnumerator.Current);
                {method}(firstEnumerator.Current, value + 1);
                {method}(value: firstEnumerator.Current, returnThis: value + 1);
                {method}(returnThis: value + 1, value: firstEnumerator.Current);
                {method}(secondEnumerator.Current, value + 1);
                {method}(value: secondEnumerator.Current, returnThis: value + 1);
                {method}(returnThis: value + 1, value: secondEnumerator.Current);
                {method}(thirdEnumerator.Current, value + 1);
                {method}(value: thirdEnumerator.Current, returnThis: value + 1);
                {method}(returnThis: value + 1, value: thirdEnumerator.Current);
                {method}(fourthEnumerator.Current, value + 1);
                {method}(value: fourthEnumerator.Current, returnThis: value + 1);
                {method}(returnThis: value + 1, value: fourthEnumerator.Current);
            }}
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenReturnValueIsCalledWhileBeingConfigured(string method)
    {
        var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var firstSubstitute = Substitute.For<IFoo>();
            firstSubstitute.Id.Returns(45);

            var secondSubstitute = Substitute.For<IFoo>();
            {method}(secondSubstitute.Id, [|firstSubstitute.Id|]);
            {method}(value: secondSubstitute.Id, returnThis: [|firstSubstitute.Id|]);
            {method}(returnThis: [|firstSubstitute.Id|], value: secondSubstitute.Id);
        }}
    }} 
}}";
        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls Id. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => firstSubstitute.Id).");
    }

    public override async Task ReportsDiagnostics_WhenReturnValueIsCalledWhileBeingConfiguredInConstructorBody(string method)
    {
        var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public FooTests()
        {{
            firstSubstitute.Id.Returns(45);
        }}
        
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var secondSubstitute = Substitute.For<IFoo>();
            {method}(secondSubstitute.Id, [|firstSubstitute.Id|]);
            {method}(value: secondSubstitute.Id, returnThis: [|firstSubstitute.Id|]);
            {method}(returnThis: [|firstSubstitute.Id|], value: secondSubstitute.Id);
        }}
    }} 
}}";
        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls Id. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => firstSubstitute.Id).");
    }

    public override async Task ReportsDiagnostics_WhenReturnValueIsCalledWhileBeingConfiguredInConstructorExpressionBody(string method)
    {
        var plainMethodName = method.Replace("SubstituteExtensions.", string.Empty).Replace("<int>", string.Empty);

        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public FooTests() => firstSubstitute.Id.Returns(45);
        
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var secondSubstitute = Substitute.For<IFoo>();
            {method}(secondSubstitute.Id, [|firstSubstitute.Id|]);
            {method}(value: secondSubstitute.Id, returnThis: [|firstSubstitute.Id|]);
            {method}(returnThis: [|firstSubstitute.Id|], value: secondSubstitute.Id);
        }}
    }} 
}}";
        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName}() is set with a method that itself calls Id. This can cause problems with NSubstitute. Consider replacing with a lambda: {plainMethodName}(x => firstSubstitute.Id).");
    }

    public override async Task ReportsNoDiagnostics_WhenReturnValueIsCalledAfterIsConfigured(string method)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public interface IFoo
        {{
            int Id {{ get; }}

            int OtherId {{ get; }}
        }}
        
        public void Test()
        {{
            var firstSubstitute = Substitute.For<IFoo>();
            var secondSubstitute = Substitute.For<IFoo>();
            var thirdSubstitute = Substitute.For<IFoo>();
            var fourthSubstitute = Substitute.For<IFoo>();

            firstSubstitute.OtherId.Returns(45);
            thirdSubstitute.Id.Returns(45);
            fourthSubstitute.Id.Returns(45);
            var value = fourthSubstitute.Id;

            {method}(secondSubstitute.Id, firstSubstitute.Id);
            {method}(value: secondSubstitute.Id, returnThis: firstSubstitute.Id);
            {method}(returnThis: firstSubstitute.Id, value: secondSubstitute.Id);
            {method}(secondSubstitute.Id, value);
            {method}(value: secondSubstitute.Id, returnThis: value);
            {method}(returnThis: value, value: secondSubstitute.Id);
        }}
    }} 
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegateInArrayParams_AndReEntrantReturnsForAnyArgsCallExists(string method)
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
            {method}(substitute.Bar(), _ => 1, new Func<CallInfo, int>[] {{ _ => OtherReturn() }});
            {method}(value: substitute.Bar(), returnThis: _ => 1, returnThese: new Func<CallInfo, int>[] {{ _ => OtherReturn() }});
            {method}(returnThis: _ => 1, returnThese: new Func<CallInfo, int>[] {{ _ => OtherReturn() }}, value: substitute.Bar());
        }}

        private int OtherReturn()
        {{
            var substitute = Substitute.For<IBar>();
            substitute.Foo().Returns(1);
            return 1;
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}