﻿using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberWhenAnalyzerTests;

[CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Foo>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Foo>")]
public class WhenAsOrdinaryMethodTests : NonSubstitutableMemberWhenDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMemberFromBaseClass(string method, string whenAction)
    {
        var source = $@"using NSubstitute;
namespace MyNamespace
{{
    public abstract class FooBar
    {{
        public int Bar(int x)
        {{
            return 1;
        }}
    }}

    public class Foo : FooBar
    {{
    }}
    
    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = Substitute.For<Foo>();

            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooBar
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class Foo : FooBar
    {{
        public override int Bar(int x) => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Func<int>>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Func<int>>")]
    public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction)
    {
        var source = $@"using NSubstitute;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = Substitute.For<Func<int>>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooBar
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class Foo : FooBar
    {{
        public sealed override int Bar(int x) => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

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
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Foo<int>>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Foo<int>>")]
    public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
   public interface Foo<T>
    {{
        int Bar<T>(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int this[int i] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction)
    {
        var source = $@"

namespace NSubstitute
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 1;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T When<T>(this T substitute, System.Action<T> substituteCall, int x)
        {{
            return default(T);
        }}

        public static T WhenForAnyArgs<T>(this T substitute, System.Action<T> substituteCall, int x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            {method}(substitute, {whenAction}, 1);
            {method}(substituteCall: {whenAction}, substitute: substitute, x: 1);
            {method}(substitute: substitute, substituteCall: {whenAction}, x: 1);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar {{get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar {{get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int this[int x] => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {whenAction}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {whenAction}).Do(callInfo => i++);
            {method}(substituteCall: {whenAction}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            Task SubstituteCall(Foo sub)
            {{
                [|sub.Bar(Arg.Any<int>())|];
                return Task.CompletedTask;
            }}

            Task OtherSubstituteCall(Foo sub)
            {{
                [|sub.Bar(Arg.Any<int>())|];
                return Task.CompletedTask;
            }}

            Task YetAnotherSubstituteCall(Foo sub)
            {{
                [|sub.Bar(Arg.Any<int>())|];
                return Task.CompletedTask;
            }}

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: OtherSubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: YetAnotherSubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}
    }}
}}";
        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            Task SubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);
            Task OtherSubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);
            Task YetAnotherSubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: OtherSubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: YetAnotherSubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: OtherSubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: YetAnotherSubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}

        private Task SubstituteCall(Foo sub)
        {{
            var objBarr = [|sub.Bar(Arg.Any<int>())|];
            return Task.CompletedTask;
        }}

        private Task OtherSubstituteCall(Foo sub)
        {{
            var objBarr = [|sub.Bar(Arg.Any<int>())|];
            return Task.CompletedTask;
        }}

        private Task YetAnotherSubstituteCall(Foo sub)
        {{
            var objBarr = [|sub.Bar(Arg.Any<int>())|];
            return Task.CompletedTask;
        }}
    }}
}}";
        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: OtherSubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: YetAnotherSubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}

        private Task SubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);

        private Task OtherSubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);

        private Task YetAnotherSubstituteCall(Foo sub) => Task.FromResult([|sub.Bar(Arg.Any<int>())|]);
    }}
}}";
        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            Task SubstituteCall(Foo sub)
            {{
                sub.Bar(Arg.Any<int>());
                return Task.CompletedTask;
            }}

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: SubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: SubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            Task SubstituteCall(Foo sub) => Task.FromResult(sub.Bar(Arg.Any<int>()));

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: SubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: SubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: SubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: SubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}

        private Task SubstituteCall(Foo sub)
        {{
            var objBarr = sub.Bar(Arg.Any<int>());
            return Task.CompletedTask;
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction(string method)
    {
        var source = $@"using NSubstitute;
using System.Threading.Tasks;
namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar(int x)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            {method}(substitute, SubstituteCall).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: SubstituteCall).Do(callInfo => i++);
            {method}(substituteCall: SubstituteCall, substitute: substitute).Do(callInfo => i++);
            substitute.Bar(1);
        }}

        private Task SubstituteCall(Foo sub) => Task.FromResult(sub.Bar(Arg.Any<int>()));
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar(int x)
        {{
            return 1;
        }}

        internal virtual int this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {call}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {call}).Do(callInfo => i++);
            {method}(substituteCall: {call}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar(int x)
        {{
            return 1;
        }}

        internal virtual int this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {call}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {call}).Do(callInfo => i++);
            {method}(substituteCall: {call}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar(int x)
        {{
            return 1;
        }}

        internal virtual int this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {call}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {call}).Do(callInfo => i++);
            {method}(substituteCall: {call}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
    {
        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual int Bar {{ get; }}

        protected internal virtual int FooBar(int x)
        {{
            return 1;
        }}

        protected internal virtual int this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, {call}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: {call}).Do(callInfo => i++);
            {method}(substituteCall: {call}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}