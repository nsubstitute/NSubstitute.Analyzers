using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests
{
    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received<Foo>(substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute)")]
    public class ReceivedAsOrdinaryMethodTests : NonSubstitutableMemberReceivedDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar()
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            [|{method}.Bar()|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar()
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}.Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar()
        {{
            return 2;
        }}
    }}

    public class Foo2 : Foo
    {{
        public override int Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}.Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
            "ReceivedExtensions.Received(substitute, Quantity.None())",
            "ReceivedExtensions.Received<Func<Foo>>(substitute, Quantity.None())",
            "SubstituteExtensions.Received(substitute)",
            "SubstituteExtensions.Received<Func<Foo>>(substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute, Quantity.None())",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute)",
            "SubstituteExtensions.DidNotReceive(substitute)",
            "SubstituteExtensions.DidNotReceive<Func<Foo>>(substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<Foo>>(substitute)")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

public class Foo
{{
}}

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<Foo>>();
            {method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class FooBar
    {{
        public virtual int Bar()
        {{
            return 2;
        }}
    }}

    public class Foo : FooBar
    {{
        public sealed override int Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            [|{method}.Bar()|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}.Bar();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}.Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}.Bar;
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
                "ReceivedExtensions.Received(substitute, Quantity.None())",
                "ReceivedExtensions.Received<Foo<int>>(substitute, Quantity.None())",
                "SubstituteExtensions.Received(substitute)",
                "SubstituteExtensions.Received<Foo<int>>(substitute)",
                "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
                "ReceivedExtensions.ReceivedWithAnyArgs<Foo<int>>(substitute, Quantity.None())",
                "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
                "SubstituteExtensions.ReceivedWithAnyArgs<Foo<int>>(substitute)",
                "SubstituteExtensions.DidNotReceive(substitute)",
                "SubstituteExtensions.DidNotReceive<Foo<int>>(substitute)",
                "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
                "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo<int>>(substitute)")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
   public interface Foo<T>
    {{
        int Bar<T>();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}.Bar<int>();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}.Bar;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}[1];
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}.Bar;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = [|{method}.Bar|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int this[int x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}[1];
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int this[int x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = [|{method}[1]|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [CombinatoryData(
            "ReceivedExtensions.Received(substitute, Quantity.None())",
            "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
            "SubstituteExtensions.Received(substitute, 1, 1)",
            "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)")]
        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
        {
            var source = $@"

namespace NSubstitute
{{
    public class Quantity
    {{
        public static Quantity None() => null;
    }}

    public class Foo
    {{
        public int Bar()
        {{
            return 1;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}
    }}
    
    public static class ReceivedExtensions
    {{
        public static T Received<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            var x = {method}.Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = [|{method}{call}|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}{call};
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar {{ get; }}

        internal virtual int FooBar()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = [|{method}{call}|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual int Bar {{ get; }}

        protected internal virtual int FooBar()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}{call};
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}