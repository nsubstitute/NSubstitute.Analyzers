using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupReceivedAnalyzerTests
{
    [CombinatoryData("Received", "Received<Foo>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Foo>", "DidNotReceive", "DidNotReceive<Foo>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Foo>")]
    public class ReceivedAsExtensionMethodTests : NonVirtualSetupReceivedDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            [|substitute.{method}()|].Bar();
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            substitute.{method}().Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            substitute.{method}().Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData("Received", "Received<Func<Foo>>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Func<Foo>>", "DidNotReceive", "DidNotReceive<Func<Foo>>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Func<Foo>>")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method)
        {
            var source = $@"using NSubstitute;
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
            substitute.{method}()();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            [|substitute.{method}()|].Bar();
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            substitute.{method}().Bar();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            substitute.{method}().Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method)
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.{method}().Bar;
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData("Received", "Received<Foo<int>>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Foo<int>>", "DidNotReceive", "DidNotReceive<Foo<int>>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Foo<int>>")]
        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            substitute.{method}().Bar<int>();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method)
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.{method}().Bar;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method)
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.{method}()[1];
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method)
        {
            var source = $@"using NSubstitute;

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
            var x = substitute.{method}().Bar;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method)
        {
            var source = $@"using NSubstitute;

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
            var x = [|substitute.{method}()|].Bar;
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method)
        {
            var source = $@"using NSubstitute;

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
            var x = substitute.{method}()[1];
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method)
        {
            var source = $@"using NSubstitute;

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
            var x = [|substitute.{method}()|][1];
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
        {
            var source = $@"

namespace NSubstitute
{{
    public class Foo
    {{
        public int Bar()
        {{
            return 1;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            substitute.{method}(1m).Bar();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }
    }
}