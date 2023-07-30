﻿using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberAnalyzerTests;

[CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<int>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<int>")]
public class ReturnsAsOrdinaryMethodTests : NonSubstitutableMemberDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
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
            {method}(value: [|substitute.Bar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    [CombinatoryData("SubstituteExtensions.Returns", "SubstituteExtensions.Returns<>", "SubstituteExtensions.ReturnsForAnyArgs", "SubstituteExtensions.ReturnsForAnyArgs<>")]
    public override async Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type)
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
            {method}(value: [|{literal}|], returnThis: {literal});
            {method}(returnThis: {literal}, value: [|{literal}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, $"Member {literal} can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method)
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
            {method}(value: [|Foo.Bar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|Foo.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
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
            {method}(value: substitute.Bar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar());
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
            {method}(value: returnValue, returnThis: 1);
            {method}(returnThis: 1, value: returnValue);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

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
            var substitute = Substitute.For<Func<int>>();
            {method}(substitute(), 1);
            {method}(value: substitute(), returnThis: 1);
            {method}(returnThis: 1, value: substitute());
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
            {method}(value: [|substitute.Bar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
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
            {method}(value: substitute.Bar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar());
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
            {method}(value: substitute.Bar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar());
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
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
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
            {method}(value: substitute.Bar<int>(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar<int>());
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar, 1);
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
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
            {method}(value: substitute[1], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
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
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
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
        public int Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute.Bar|], 1);
            {method}(value: [|substitute.Bar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.Bar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
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
            {method}(value: substitute[1], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1]);
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
        public int this[int x] => 0;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute[1]|], 1);
            {method}(value: [|substitute[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute[1]|]);
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
        public int Bar()
        {{
            return 1;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T Returns<T>(this T value, T returnThis)
        {{
            return default(T);
        }}

        public static T ReturnsForAnyArgs<T>(this T value, T returnThis)
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
            {method}(value: substitute.Bar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
            {method}([|substitute.FooBar|], 1);
            {method}(value: [|substitute.FooBar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
            {method}([|substitute.FooBar|], 1);
            {method}(value: [|substitute.FooBar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute.Bar(1, 2), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar(1, 2));
            {method}([|substitute.Bar(1)|], 1);
            {method}(value: [|substitute.Bar(1)|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute.Bar<int>(1, 2), returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar<int>(1, 2));
            {method}([|substitute.Bar(1)|], 1);
            {method}(value: [|substitute.Bar(1)|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute[1,2], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1,2]);
            {method}([|substitute[1]|], 1);
            {method}(value: [|substitute[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute[1, 2], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1, 2]);
            {method}([|substitute[1]|], 1);
            {method}(value: [|substitute[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute[1], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1]);
            {method}(substitute.Bar, 1);
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
            {method}(substitute.FooBar(), 1);
            {method}(value: substitute.FooBar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}(value: [|substituteFooBarBar[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}(value: [|substituteFooBarBar.Bar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], 1);
            {method}(value: [|substituteFooBarBar.FooBar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.FooBar()|]);
        }}
    }}
}}";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute[1], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1]);
            {method}(substitute.Bar, 1);
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
            {method}(substitute.FooBar(), 1);
            {method}(value: substitute.FooBar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}(value: [|substituteFooBarBar[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}(value: [|substituteFooBarBar.Bar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], 1);
            {method}(value: [|substituteFooBarBar.FooBar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.FooBar()|]);
        }}
    }}
}}";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute[1], returnThis: 1);
            {method}(returnThis: 1, value: substitute[1]);
            {method}(substitute.Bar, 1);
            {method}(value: substitute.Bar, returnThis: 1);
            {method}(returnThis: 1, value: substitute.Bar);
            {method}(substitute.FooBar(), 1);
            {method}(value: substitute.FooBar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], 1);
            {method}(value: [|substituteFooBarBar[1]|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], 1);
            {method}(value: [|substituteFooBarBar.Bar|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], 1);
            {method}(value: [|substituteFooBarBar.FooBar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substituteFooBarBar.FooBar()|]);
        }}
    }}
}}";

        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
            "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted."
        };

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(NonVirtualSetupSpecificationDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();

        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", NonVirtualSetupSpecificationDescriptor.Id);

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
            {method}(value: substitute.GetBar(), returnThis: 1);
            {method}(returnThis: 1, value: substitute.GetBar());
            {method}([|substitute.GetFooBar()|], 1);
            {method}(value: [|substitute.GetFooBar()|], returnThis: 1);
            {method}(returnThis: 1, value: [|substitute.GetFooBar()|]);
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

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
    {
        var source = $@"using NSubstitute;

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
            var x = {method}([|substitute{call}|], 1);
            var y = {method}(value: [|substitute{call}|], returnThis: 1);
            var z = {method}(returnThis: 1, value: [|substitute{call}|]);
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
            var x = {method}(substitute{call}, 1);
            var y = {method}(value: substitute{call}, returnThis: 1);
            var z = {method}(returnThis: 1, value: substitute{call});
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
            var x = {method}([|substitute{call}|], 1);
            var y = {method}(value: [|substitute{call}|], returnThis: 1);
            var u = {method}(returnThis: 1, value: [|substitute{call}|]);
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
            var x = {method}(substitute{call}, 1);
            var y = {method}(value: substitute{call}, returnThis: 1);
            var z = {method}(returnThis: 1, value: substitute{call});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}