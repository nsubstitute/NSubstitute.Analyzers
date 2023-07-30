using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests;

[CombinatoryData(
    "Received(Quantity.None())",
    "Received<Foo>(Quantity.None())",
    "Received()",
    "Received<Foo>()",
    "ReceivedWithAnyArgs(Quantity.None())",
    "ReceivedWithAnyArgs<Foo>(Quantity.None())",
    "ReceivedWithAnyArgs()",
    "ReceivedWithAnyArgs<Foo>()",
    "DidNotReceive()",
    "DidNotReceive<Foo>()",
    "DidNotReceiveWithAnyArgs()",
    "DidNotReceiveWithAnyArgs<Foo>()")]
public class ReceivedAsExtensionMethodTests : NonSubstitutableMemberReceivedDiagnosticVerifier
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
            [|substitute.{method}.Bar()|];
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
            substitute.{method}.Bar();
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
    public class FooBar
    {{
        public virtual int Bar()
        {{
            return 2;
        }}
    }}

    public class Foo : FooBar
    {{
        public override int Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}.Bar();
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received<Func<Foo>>(Quantity.None())",
        "Received",
        "Received<Func<Foo>>",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Func<Foo>>(Quantity.None())",
        "ReceivedWithAnyArgs",
        "ReceivedWithAnyArgs<Func<Foo>>",
        "DidNotReceive",
        "DidNotReceive<Func<Foo>>",
        "DidNotReceiveWithAnyArgs",
        "DidNotReceiveWithAnyArgs<Func<Foo>>")]
    public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace MyNamespace
{{
    public class Foo
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<Foo>>();
            substitute.{method}();
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
            [|substitute.{method}.Bar()|];
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
            substitute.{method}.Bar();
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
            substitute.{method}.Bar();
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
            var x = substitute.{method}.Bar;
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received<Foo<int>>(Quantity.None())",
        "Received()",
        "Received<Foo<int>>()",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Foo<int>>(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "ReceivedWithAnyArgs<Foo<int>>()",
        "DidNotReceive()",
        "DidNotReceive<Foo<int>>()",
        "DidNotReceiveWithAnyArgs()",
        "DidNotReceiveWithAnyArgs<Foo<int>>()")]
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
            substitute.{method}.Bar<int>();
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
            var x = substitute.{method}.Bar;
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
            var x = substitute.{method}[1];
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
            var x = substitute.{method}.Bar;
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
            var x = [|substitute.{method}.Bar|];
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
            var x = substitute.{method}[1];
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
            var x = [|substitute.{method}[1]|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualEvent(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace MyNamespace
{{
    public class Foo
    {{
        public event Action Event;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            [|substitute.{method}.Event|] += () => {{ }};
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualReceivedSetupSpecificationDescriptor, "Member Event can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractEvent(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract event Action Event;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}.Event += () => {{ }};
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualEvent(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual event Action Event;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}.Event += () => {{ }};
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceEvent(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;

namespace MyNamespace
{{
    public interface Foo
    {{
        event Action Event;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}.Event += () => {{ }};
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received<Foo>(Quantity.None())",
        "Received(1, 1)",
        "Received<Foo>(1, 1)",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Foo>(Quantity.None())",
        "ReceivedWithAnyArgs(1, 1)",
        "ReceivedWithAnyArgs<Foo>(1, 1)",
        "DidNotReceive(1, 1)",
        "DidNotReceive<Foo>(1, 1)",
        "DidNotReceiveWithAnyArgs(1, 1)",
        "DidNotReceiveWithAnyArgs<Foo>(1, 1)")]
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
            substitute.{method}.Bar();
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
            var x = [|substitute.{method}{call}|];
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
            var x = substitute.{method}{call};
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
            var x = [|substitute.{method}{call}|];
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
            var x = substitute.{method}{call};
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}