﻿using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberAnalyzerTests;

[CombinatoryData("ReturnsExtensions.ReturnsNull", "ReturnsExtensions.ReturnsNull<object>", "ReturnsExtensions.ReturnsNullForAnyArgs", "ReturnsExtensions.ReturnsNullForAnyArgs<object>")]
public class ReturnsNullAsOrdinaryMethodTests : NonSubstitutableMemberDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar()
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type)
    {
        await Task.CompletedTask;
    }

    public override async Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public static object Bar()
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {method}([|Foo.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object Bar()
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object Bar()
        {{
            return new object();
        }}
    }}

    public class Foo2 : Foo
    {{
        public override object Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}(substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object Bar()
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var returnValue = substitute.Bar();
            {method}(returnValue);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<object>>();
            {method}(substitute());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object Bar()
        {{
            return new object();
        }}
    }}

    public class Foo2 : Foo
    {{
        public sealed override object Bar() => 1;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}([|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract object Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar());
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        object Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        object Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public interface IFoo<T>
    {{
        object Bar<T>();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            {method}(substitute.Bar<int>());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract object Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        object this[int i] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute[1]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute.Bar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual object this[int x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object this[int x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
    {
        var source = $@"

namespace NSubstitute
{{
    public class Foo
    {{
        public object Bar()
        {{
            return 1;
        }}
    }}

    public static class ReturnsExtensions
    {{
        public static T ReturnsNull<T>(this T returnValue)
        {{
            return default(T);
        }}

        public static T ReturnsNullForAnyArgs<T>(this T returnValue)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            {method}(substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar {{ get; }}

        public object FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar);
            {method}([|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo<T> where T : class
    {{
        public T Bar {{ get; }}

        public object FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<object>>();
            {method}(substitute.Bar);
            {method}([|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar(int x)
        {{
            return 1;
        }}

        public object Bar(int x, int y)
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(1, 2));
            {method}([|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar(int x)
        {{
            return 1;
        }}

        public object Bar<T>(T x, T y)
        {{
            return new object();
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar<int>(1, 2));
            {method}([|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object this[int x] => 0;
        public object this[int x, int y] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1,2]);
            {method}([|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public object this[T x] => 0;
        public object this[T x, T y] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}(substitute[1, 2]);
            {method}([|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public object Bar {{ get; set; }}
        public object this[int x] => 0;
        public object FooBar()
        {{
            return 1;
        }}
    }}

    public class FooBarBar
    {{
        public object Bar {{ get;set; }}
        public object this[int x] => 0;
        public object FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1]);
            {method}(substitute.Bar);
            {method}(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|]);
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

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public object Bar {{ get; set; }}
        public object this[int x] => 0;
        public object FooBar()
        {{
            return 1;
        }}
    }}

    public class FooBarBar<T>
    {{
        public object Bar {{ get;set; }}
        public object this[int x] => 0;
        public object FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}(substitute[1]);
            {method}(substitute.Bar);
            {method}(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            {method}([|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|]);
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

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyOtherNamespace
{{
    public class FooBarBar
    {{
        public object Bar {{ get; set; }}
        public object this[int x] => 0;
        public object FooBar()
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
        public object Bar {{ get; set; }}
        public object this[int x] => 0;
        public object FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1]);
            {method}(substitute.Bar);
            {method}(substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|]);
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
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Object", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            {method}(substitute.GetBar());
            {method}([|substitute.GetFooBar()|]);
        }}
    }}

    public static class MyExtensions
    {{
        public static IBar Bar {{ get; set; }}

        public static object GetBar(this object @object)
        {{
            return Bar.Foo(@object);
        }}

        public static object GetFooBar(this object @object)
        {{
            return 1;
        }}
    }}

    public interface IBar
    {{
        object Foo(object @obj);
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual object Bar {{ get; }}

        internal virtual object FooBar()
        {{
            return 1;
        }}

        internal virtual object this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}([|substitute{call}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
using NSubstitute.ReturnsExtensions;
[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual object Bar {{ get; }}

        internal virtual object FooBar()
        {{
            return 1;
        }}

        internal virtual object this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute{call});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
    {
        var source = $@"using NSubstitute;
using System.Runtime.CompilerServices;
using NSubstitute.ReturnsExtensions;
[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual object Bar {{ get; }}

        internal virtual object FooBar()
        {{
            return 1;
        }}

        internal virtual object this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}([|substitute{call}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual object Bar {{ get; }}

        protected internal virtual object FooBar()
        {{
            return 1;
        }}

        protected internal virtual object this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute{call});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}