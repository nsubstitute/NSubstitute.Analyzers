using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupWhenAnalyzerTests
{
    public class WhenAsOrdinaryMethodWithGenericTypeSpecifiedTests : NonVirtualSetupWhenDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedLine, int expectedColumn)
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
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction)
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
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction)
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

    public class Foo2 : Foo
    {{
        public override int Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo2>();
            SubstituteExtensions.When<Foo2>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction)
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
            SubstituteExtensions.When<Func<int>>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedLine, int expectedColumn)
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

    public class Foo2 : Foo
    {{
        public sealed override int Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo2>();
            SubstituteExtensions.When<Foo2>(substitute, {whenAction}).Do(callInfo => i++);

        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction)
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
            int i = 1;
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction)
        {
            var source = $@"using NSubstitute;

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
            int i = 1;
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.When<IFoo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.When<IFoo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
   public interface IFoo<T>
    {{
        int Bar<T>();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            SubstituteExtensions.When<IFoo<int>>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction)
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
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int this[int i] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            int i = 1;
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.When<IFoo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction)
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
        public static T When<T>(this T substitute, System.Action<T> substituteCall, int x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            SubstituteExtensions.When<Foo>(substitute, {whenAction}, 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedLine, int expectedColumn)
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
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction)
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
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedLine, int expectedColumn)
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
            SubstituteExtensions.When<Foo>(substitute, {whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            void SubstituteCall(Foo sub)
            {
                sub.Bar();
            }

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            void SubstituteCall(Foo sub) => sub.Bar();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }

        private void SubstituteCall(Foo sub)
        {
            var objBarr = sub.Bar();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }

        private void SubstituteCall(Foo sub) => sub.Bar();
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;
            expectedDiagnostic.OverrideMessage("Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            void SubstituteCall(Foo sub)
            {
                sub.Bar();
            }

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            void SubstituteCall(Foo sub) => sub.Bar();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }

        private void SubstituteCall(Foo sub)
        {
            var objBarr = sub.Bar();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            int i = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();

            SubstituteExtensions.When<Foo>(substitute, SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }

        private void SubstituteCall(Foo sub) => sub.Bar();
    }
}";
            await VerifyNoDiagnostic(source);
        }
    }
}