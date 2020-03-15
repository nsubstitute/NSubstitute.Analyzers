using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberAnalyzerTests
{
    [CombinatoryData("Throws<Exception>", "ThrowsForAnyArgs<Exception>")]
    public class ThrowsAsExtensionMethodWithGenericTypeSpecifiedTests : NonSubstitutableMemberDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            [|substitute.Bar()|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            [|{literal}|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, $"Member {literal} can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public static int Bar()
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            [|Foo.Bar()|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var returnValue = substitute.Bar();
            returnValue.{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<int>>();
            substitute().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var substitute = NSubstitute.Substitute.For<Foo2>();
            [|substitute.Bar()|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar.{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            substitute.Bar<int>().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar.{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute[1].{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar.{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            [|substitute.Bar|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute[1].{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            [|substitute[1]|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
        {
            var source = $@"
using System;
namespace NSubstitute
{{
    public class Foo
    {{
        public int Bar()
        {{
            return 1;
        }}
    }}

    public static class ExceptionExtensions
    {{
        public static T Throws<T>(this object value) where T: Exception
        {{
            return default(T);
        }}

        public static T ThrowsForAnyArgs<T>(this object value) where T: Exception
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            substitute.Bar().{method}();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar {{ get; }}

        public int FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.{method}();
            [|substitute.FooBar|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public T Bar {{ get; }}

        public int FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute.Bar.{method}();
            [|substitute.FooBar|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 1;
        }}

        public int Bar(int x, int y)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(1, 2).{method}();
            [|substitute.Bar(1)|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar(int x)
        {{
            return 1;
        }}

        public int Bar<T>(T x, T y)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar<int>(1, 2).{method}();
            [|substitute.Bar(1)|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int this[int x] => 0;
        public int this[int x, int y] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1,2].{method}();
            [|substitute[1]|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public int this[T x] => 0;
        public int this[T x, T y] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute[1, 2].{method}();
            [|substitute[1]|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public int Bar {{ get; set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooBarBar
    {{
        public int Bar {{ get;set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].{method}();
            substitute.Bar.{method}();
            substitute.FooBar().{method}();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            [|substituteFooBarBar[1]|].{method}();
            [|substituteFooBarBar.Bar|].{method}();
            [|substituteFooBarBar.FooBar()|].{method}();
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public int Bar {{ get; set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooBarBar<T>
    {{
        public int Bar {{ get;set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute[1].{method}();
            substitute.Bar.{method}();
            substitute.FooBar().{method}();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            [|substituteFooBarBar[1]|].{method}();
            [|substituteFooBarBar.Bar|].{method}();
            [|substituteFooBarBar.FooBar()|].{method}();
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyOtherNamespace
{{
    public class FooBarBar
    {{
        public int Bar {{ get; set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}
}}

namespace MyNamespace
{{
    using MyOtherNamespace;
    public class Foo
    {{
        public int Bar {{ get; set; }}
        public int this[int x] => 0;
        public int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].{method}();
            substitute.Bar.{method}();
            substitute.FooBar().{method}();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            [|substituteFooBarBar[1]|].{method}();
            [|substituteFooBarBar.Bar|].{method}();
            [|substituteFooBarBar.FooBar()|].{method}();
        }}
    }}
}}";

            var textParserResult = TextParser.GetSpans(source);

            var diagnosticMessages = new[]
            {
                "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
            };

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", NonVirtualSetupSpecificationDescriptor.Id);

            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            substitute.GetBar().{method}();
            [|substitute.GetFooBar()|].{method}();
        }}
    }}

    public static class MyExtensions
    {{
        public static IBar Bar {{ get; set; }}

        public static int GetBar(this object @object)
        {{
            return Bar.Foo(@object);
        }}

        public static int GetFooBar(this object @object)
        {{
            return 1;
        }}
    }}

    public interface IBar
    {{
        int Foo(object @obj);
    }}
}}";

            await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var x = [|substitute{call}|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
            var x = substitute{call}.{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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
            var x = [|substitute{call}|].{method}();
        }}
    }}
}}";

            await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var x = substitute{call}.{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}