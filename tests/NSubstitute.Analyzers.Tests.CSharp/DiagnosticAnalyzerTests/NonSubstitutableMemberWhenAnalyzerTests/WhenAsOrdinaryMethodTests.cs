using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberWhenAnalyzerTests;

[CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Foo>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Foo>")]
public class WhenAsOrdinaryMethodTests : NonSubstitutableMemberWhenDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
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
            {method}(substitute, sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substituteCall: sub => [|sub.Bar(Arg.Any<int>())|], substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method)
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

            {method}(substitute, sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substituteCall: sub => [|sub.Bar(Arg.Any<int>())|], substitute: substitute).Do(callInfo => i++);


            {method}(substitute, sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}, substitute: substitute).Do(callInfo => i++);


            {method}(substitute, delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
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
            {method}(substitute, sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substituteCall: sub => sub.Bar(Arg.Any<int>()), substitute: substitute).Do(callInfo => i++);


            {method}(substitute, sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method)
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
            {method}(substitute, sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substituteCall: sub => sub.Bar(Arg.Any<int>()), substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Func<int>>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Func<int>>")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method)
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
            {method}(substitute, sub => sub()).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub()).Do(callInfo => i++);
            {method}(substituteCall: sub => sub(), substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ sub(); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub(); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub(); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Func<int> sub) {{ sub(); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Func<int> sub) {{ sub(); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Func<int> sub) {{ sub(); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method)
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
            {method}(substitute, sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => [|sub.Bar(Arg.Any<int>())|]).Do(callInfo => i++);
            {method}(substituteCall: sub => [|sub.Bar(Arg.Any<int>())|], substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ [|sub.Bar(Arg.Any<int>())|]; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ [|sub.Bar(Arg.Any<int>())|]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
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
            {method}(substitute, sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substituteCall: sub => sub.Bar(Arg.Any<int>()), substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method)
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
            {method}(substitute, sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substituteCall: sub => sub.Bar(Arg.Any<int>()), substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method)
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
            {method}(substitute, sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ int x; x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Foo<int>>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Foo<int>>")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
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
            {method}(substitute, sub => sub.Bar<int>(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar<int>(Arg.Any<int>())).Do(callInfo => i++);
            {method}(substituteCall: sub => sub.Bar<int>(Arg.Any<int>()), substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ sub.Bar<int>(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar<int>(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ sub.Bar<int>(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo<int> sub) {{ sub.Bar<int>(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo<int> sub) {{ sub.Bar<int>(Arg.Any<int>()); }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo<int> sub) {{ sub.Bar<int>(Arg.Any<int>()); }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method)
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
            {method}(substitute, sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ int x; x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method)
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
            {method}(substitute, delegate(Foo sub) {{ var x = sub[Arg.Any<int>()]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = sub[Arg.Any<int>()]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = sub[Arg.Any<int>()]; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ var x = sub[Arg.Any<int>()]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = sub[Arg.Any<int>()]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = sub[Arg.Any<int>()]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
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
            {method}(substitute, sub => sub.Bar(Arg.Any<int>()), 1);
            {method}(substituteCall: sub => sub.Bar(Arg.Any<int>()), substitute: substitute, x: 1);
            {method}(substitute: substitute, substituteCall: sub => sub.Bar(Arg.Any<int>()), x: 1);

            {method}(substitute, sub => {{ sub.Bar(Arg.Any<int>()); }}, 1);
            {method}(substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute, x: 1);
            {method}(substitute: substitute, substituteCall: sub => {{ sub.Bar(Arg.Any<int>()); }}, x: 1);

            {method}(substitute, delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, 1);
            {method}(substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, substitute: substitute, x: 1);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ sub.Bar(Arg.Any<int>()); }}, x: 1);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method)
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
            {method}(substitute, sub => {{ var x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = [|sub.Bar|]; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ int x; x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ int x; x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ int x; x = [|sub.Bar|]; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ var x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = [|sub.Bar|]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = [|sub.Bar|]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
    {
        throw new System.NotImplementedException();
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
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
            {method}(substitute, sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ int x; x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ int x; x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = sub.Bar; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method)
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
            {method}(substitute, sub => {{ var x = [|sub[Arg.Any<int>()]|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: sub => {{ var x = [|sub[Arg.Any<int>()]|]; }}).Do(callInfo => i++);
            {method}(substituteCall: sub => {{ var x = [|sub[Arg.Any<int>()]|]; }}, substitute: substitute).Do(callInfo => i++);

            {method}(substitute, delegate(Foo sub) {{ var x = [|sub[Arg.Any<int>()]|]; }}).Do(callInfo => i++);
            {method}(substitute: substitute, substituteCall: delegate(Foo sub) {{ var x = [|sub[Arg.Any<int>()]|]; }}).Do(callInfo => i++);
            {method}(substituteCall: delegate(Foo sub) {{ var x = [|sub[Arg.Any<int>()]|]; }}, substitute: substitute).Do(callInfo => i++);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualWhenSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InLocalFunction(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InExpressionBodiedLocalFunction(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularFunction(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularExpressionBodiedFunction(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InLocalFunction(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InExpressionBodiedLocalFunction(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularFunction(string method)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularExpressionBodiedFunction(string method)
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
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

    public override async Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call)
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