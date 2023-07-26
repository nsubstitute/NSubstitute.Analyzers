using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests;

[CombinatoryData(
    "ReceivedExtensions.Received(substitute, Quantity.None())",
    "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
    "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
    "ReceivedExtensions.Received<Foo>(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.Received<Foo>(requiredQuantity: Quantity.None(), substitute: substitute)",
    "SubstituteExtensions.Received(substitute)",
    "SubstituteExtensions.Received(substitute: substitute)",
    "SubstituteExtensions.Received<Foo>(substitute)",
    "SubstituteExtensions.Received<Foo>(substitute: substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, requiredQuantity: Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(requiredQuantity: Quantity.None(), substitute: substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute)",
    "SubstituteExtensions.DidNotReceive(substitute)",
    "SubstituteExtensions.DidNotReceive(substitute: substitute)",
    "SubstituteExtensions.DidNotReceive<Foo>(substitute)",
    "SubstituteExtensions.DidNotReceive<Foo>(substitute: substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute: substitute)")]
public class ReceivedAsOrdinaryMethodTests : NonSubstitutableMemberReceivedDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method)
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
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.Received<Func<Foo>>(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Func<Foo>>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received<Func<Foo>>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)",
        "SubstituteExtensions.Received<Func<Foo>>(substitute)",
        "SubstituteExtensions.Received<Func<Foo>>(substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Func<Foo>>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Func<Foo>>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive<Func<Foo>>(substitute)",
        "SubstituteExtensions.DidNotReceive<Func<Foo>>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<Foo>>(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<Foo>>(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method)
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
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.Received<Foo<int>>(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo<int>>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received<Foo<int>>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)",
        "SubstituteExtensions.Received<Foo<int>>(substitute)",
        "SubstituteExtensions.Received<Foo<int>>(substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo<int>>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo<int>>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo<int>>(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo<int>>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive<Foo<int>>(substitute)",
        "SubstituteExtensions.DidNotReceive<Foo<int>>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo<int>>(substitute: substitute)")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method)
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
            [|{method}.Event|] += () => {{ }};
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
            {method}.Event += () => {{ }};
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
            {method}.Event += () => {{ }};
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
            {method}.Event += () => {{ }};
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.Received(x: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo>(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.Received<Foo>(x: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute, 1, 1)",
        "SubstituteExtensions.Received(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.Received(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.Received<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.Received<Foo>(x: 1, y: 1, substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(x: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(x: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceive(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(x: 1, y: 1, substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)")]
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call)
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