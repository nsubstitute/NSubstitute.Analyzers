using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<int>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<int>")]
    public class ReturnsAsOrdinaryMethodTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method)
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
            {method}([|substitute.Bar()|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<>")]
        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type)
        {
            method = method.Replace("<>", $"<{type}>");

            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            {method}([|{literal}|], {literal});
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, $"Member {literal} can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method)
        {
            var source = $@"using NSubstitute;

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
            {method}([|Foo.Bar()|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method)
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
            {method}(substitute.Bar(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method)
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
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}(substitute.Bar(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
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
            var returnValue = substitute.Bar();
            {method}(returnValue, 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method)
        {
            var source = $@"using NSubstitute;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<int>>();
            {method}(substitute(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method)
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
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}([|substitute.Bar()|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method)
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
            {method}(substitute.Bar(), 1);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method)
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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method)
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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar, 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method)
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
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            {method}(substitute.Bar<int>(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method)
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
            {method}(substitute.Bar, 1);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method)
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
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute[1], 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method)
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
            {method}(substitute.Bar, 1);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method)
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
            {method}([|substitute.Bar|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method)
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
            {method}(substitute[1], 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method)
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
            {method}([|substitute[1]|], 1);
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
        public static T Returns<T>(this T returnValue, T returnThis)
        {{
            return default(T);
        }}

        public static T ReturnsForAnyArgs<T>(this T returnValue, T returnThis)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            {method}(substitute.Bar(), 1);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute.Bar, 1);
            {method}([|substitute.FooBar|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute.Bar, 1);
            {method}([|substitute.FooBar|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute.Bar(1, 2), 1);
            {method}([|substitute.Bar(1)|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute.Bar<int>(1, 2), 1);
            {method}([|substitute.Bar(1)|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute[1,2], 1);
            {method}([|substitute[1]|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute[1, 2], 1);
            {method}([|substitute[1]|], 1);
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute[1], 1);
            {method}(substitute.Bar, 1);
            {method}(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}([|substituteFooBarBar.FooBar()|], 1);
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

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute[1], 1);
            {method}(substitute.Bar, 1);
            {method}(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}([|substituteFooBarBar.FooBar()|], 1);
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

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", Descriptor.Id);

            var source = $@"using NSubstitute;

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
            {method}(substitute[1], 1);
            {method}(substitute.Bar, 1);
            {method}(substitute.FooBar(), 1);

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}([|substituteFooBarBar.FooBar()|], 1);
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

            var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(Descriptor.OverrideMessage(diagnosticMessages[idx]), span.Span, span.LineSpan)).ToArray();

            await VerifyDiagnostic(textParserResult.Text, diagnostics);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", Descriptor.Id);

            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            {method}(substitute.GetBar(), 1);
            {method}([|substitute.GetFooBar()|], 1);
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

            await VerifyDiagnostic(source, Descriptor, "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
        }
    }
}