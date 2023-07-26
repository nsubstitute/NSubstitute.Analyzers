using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberAnalyzerTests;

[CombinatoryData(
    "ExceptionExtensions.Throws",
    "ExceptionExtensions.ThrowsAsync",
    "ExceptionExtensions.ThrowsForAnyArgs",
    "ExceptionExtensions.ThrowsAsyncForAnyArgs")]
public class ThrowsAsOrdinaryMethodTests : NonSubstitutableMemberDiagnosticVerifier
{
    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute.Bar()|], new Exception());
            {method}(value: [|substitute.Bar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type)
    {
        if (method.Contains("Async"))
        {
            // ThrowsAsync like methods do not extend literals
            // TODO replace with Assert.Skip once xUnit v3 released
            // https://github.com/xunit/xunit/issues/2073
            return;
        }

        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            {method}([|{literal}|], new Exception());
            {method}(value: [|{literal}|], ex: new Exception());
            {method}(ex: new Exception(), value: [|{literal}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, $"Member {literal} can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public static Task<int> Bar()
        {{
            return Task.FromResult(2);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            {method}([|Foo.Bar()|], new Exception());
            {method}(value: [|Foo.Bar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|Foo.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> Bar()
        {{
            return Task.FromResult(2);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(), new Exception());
            {method}(value: substitute.Bar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> Bar()
        {{
            return Task.FromResult(2);
        }}
    }}

    public class Foo2 : Foo
    {{
        public override Task<int> Bar() => Task.FromResult(1);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}(substitute.Bar(), new Exception());
            {method}(value: substitute.Bar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> Bar()
        {{
            return Task.FromResult(2);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var returnValue = substitute.Bar();
            {method}(returnValue, new Exception());
            {method}(value: returnValue, ex: new Exception());
            {method}(ex: new Exception(), value: returnValue);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Func<Task<int>>>();
            {method}(substitute(), new Exception());
            {method}(value: substitute(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> Bar()
        {{
            return Task.FromResult(2);
        }}
    }}

    public class Foo2 : Foo
    {{
        public sealed override Task<int> Bar() => Task.FromResult(1);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo2>();
            {method}([|substitute.Bar()|], new Exception());
            {method}(value: [|substitute.Bar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.Bar()|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract Task<int> Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(), new Exception());
            {method}(value: substitute.Bar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar(), new Exception());
            {method}(value: substitute.Bar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo<T>
    {{
        Task<int> Bar<T>();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            {method}(substitute.Bar<int>(), new Exception());
            {method}(value: substitute.Bar<int>(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar<int>());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract Task<int> Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<int> this[int i] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            {method}(substitute[1], new Exception());
            {method}(value: substitute[1], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute.Bar|], new Exception());
            {method}(value: [|substitute.Bar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.Bar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual Task<int> this[int x] => Task.FromResult(0);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1], new Exception());
            {method}(value: substitute[1], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1]);
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> this[int x] => Task.FromResult(0);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}([|substitute[1]|], new Exception());
            {method}(value: [|substitute[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
    {
        var source = $@"
using System;
using System.Threading.Tasks;
namespace NSubstitute
{{
    public class Foo
    {{
        public Task<int> Bar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public static class ExceptionExtensions
    {{
        public static T Throws<T>(this object value, T ex) where T: Exception
        {{
            return default(T);
        }}

        public static T ThrowsAsync<T>(this Task value, T ex) where T: Exception
        {{
            return default(T);
        }}

        public static T ThrowsForAnyArgs<T>(this object value, T ex) where T: Exception
        {{
            return default(T);
        }}

        public static T ThrowsAsyncForAnyArgs<T>(this Task value, T ex) where T: Exception
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            {method}(substitute.Bar(), new Exception());
            {method}(value: substitute.Bar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar {{ get; }}

        public Task<int> FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
            {method}([|substitute.FooBar|], new Exception());
            {method}(value: [|substitute.FooBar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T> where T : Task
    {{
        public T Bar {{ get; }}

        public Task<int> FooBar {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<Task<int>>>();
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
            {method}([|substitute.FooBar|], new Exception());
            {method}(value: [|substitute.FooBar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.FooBar|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar(int x)
        {{
            return Task.FromResult(1);
        }}

        public Task<int> Bar(int x, int y)
        {{
            return Task.FromResult(2);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar(1, 2), new Exception());
            {method}(value: substitute.Bar(1, 2), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar(1, 2));
            {method}([|substitute.Bar(1)|], new Exception());
            {method}(value: [|substitute.Bar(1)|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar(int x)
        {{
            return Task.FromResult(1);
        }}

        public Task<int> Bar<T>(T x, T y)
        {{
            return Task.FromResult(2);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute.Bar<int>(1, 2), new Exception());
            {method}(value: substitute.Bar<int>(1, 2), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar<int>(1, 2));
            {method}([|substitute.Bar(1)|], new Exception());
            {method}(value: [|substitute.Bar(1)|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.Bar(1)|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> this[int x, int y] => Task.FromResult(0);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1,2], new Exception());
            {method}(value: substitute[1,2], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1,2]);
            {method}([|substitute[1]|], new Exception());
            {method}(value: [|substitute[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public Task<int> this[T x] => Task.FromResult(0);
        public Task<int> this[T x, T y] => Task.FromResult(0);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}(substitute[1, 2], new Exception());
            {method}(value: substitute[1, 2], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1, 2]);
            {method}([|substitute[1]|], new Exception());
            {method}(value: [|substitute[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute[1]|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method)
    {
        Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        public Task<int> Bar {{ get; set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooBarBar
    {{
        public Task<int> Bar {{ get;set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1], new Exception());
            {method}(value: substitute[1], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1]);
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
            {method}(substitute.FooBar(), new Exception());
            {method}(value: substitute.FooBar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], new Exception());
            {method}(value: [|substituteFooBarBar[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], new Exception());
            {method}(value: [|substituteFooBarBar.Bar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], new Exception());
            {method}(value: [|substituteFooBarBar.FooBar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.FooBar()|]);
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

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo<T>
    {{
        public Task<int> Bar {{ get; set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooBarBar<T>
    {{
        public Task<int> Bar {{ get;set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            {method}(substitute[1], new Exception());
            {method}(value: substitute[1], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1]);
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
            {method}(substitute.FooBar(), new Exception());
            {method}(value: substitute.FooBar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            {method}([|substituteFooBarBar[1]|], new Exception());
            {method}(value: [|substituteFooBarBar[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], new Exception());
            {method}(value: [|substituteFooBarBar.Bar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], new Exception());
            {method}(value: [|substituteFooBarBar.FooBar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.FooBar()|]);
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

        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyOtherNamespace
{{
    public class FooBarBar
    {{
        public Task<int> Bar {{ get; set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}
}}

namespace MyNamespace
{{
    using MyOtherNamespace;
    public class Foo
    {{
        public Task<int> Bar {{ get; set; }}
        public Task<int> this[int x] => Task.FromResult(0);
        public Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute[1], new Exception());
            {method}(value: substitute[1], ex: new Exception());
            {method}(ex: new Exception(), value: substitute[1]);
            {method}(substitute.Bar, new Exception());
            {method}(value: substitute.Bar, ex: new Exception());
            {method}(ex: new Exception(), value: substitute.Bar);
            {method}(substitute.FooBar(), new Exception());
            {method}(value: substitute.FooBar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.FooBar());

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            {method}([|substituteFooBarBar[1]|], new Exception());
            {method}(value: [|substituteFooBarBar[1]|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar[1]|]);
            {method}([|substituteFooBarBar.Bar|], new Exception());
            {method}(value: [|substituteFooBarBar.Bar|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.Bar|]);
            {method}([|substituteFooBarBar.FooBar()|], new Exception());
            {method}(value: [|substituteFooBarBar.FooBar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substituteFooBarBar.FooBar()|]);
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
        Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Threading.Tasks.Task{System.Int32}", NonVirtualSetupSpecificationDescriptor.Id);

        var source = $@"using System;
using System.Threading.Tasks;
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
            {method}(substitute.GetBar(), new Exception());
            {method}(value: substitute.GetBar(), ex: new Exception());
            {method}(ex: new Exception(), value: substitute.GetBar());
            {method}([|substitute.GetFooBar()|], new Exception());
            {method}(value: [|substitute.GetFooBar()|], ex: new Exception());
            {method}(ex: new Exception(), value: [|substitute.GetFooBar()|]);
        }}
    }}

    public static class MyExtensions
    {{
        public static IBar Bar {{ get; set; }}

        public static Task<int> GetBar(this object @object)
        {{
            return Bar.Foo(@object);
        }}

        public static Task<int> GetFooBar(this object @object)
        {{
            return Task.FromResult(1);
        }}
    }}

    public interface IBar
    {{
        Task<int> Foo(object @obj);
    }}
}}";

        await VerifyDiagnostic(source, NonVirtualSetupSpecificationDescriptor, "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.");
    }

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual Task<int> Bar {{ get; }}

        internal virtual Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}

        internal virtual Task<int> this[int x]
        {{
            get {{ return Task.FromResult(1); }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}([|substitute{call}|], new Exception());
            var y = {method}(value: [|substitute{call}|], ex: new Exception());
            var z = {method}(ex: new Exception(), value: [|substitute{call}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call)
    {
        var source = $@"using System;
using System.Threading.Tasks;
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
        internal virtual Task<int> Bar {{ get; }}

        internal virtual Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}

        internal virtual Task<int> this[int x]
        {{
            get {{ return Task.FromResult(1); }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute{call}, new Exception());
            var y = {method}(value: substitute{call}, ex: new Exception());
            var z = {method}(ex: new Exception(), value: substitute{call});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual Task<int> Bar {{ get; }}

        internal virtual Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}

        internal virtual Task<int> this[int x]
        {{
            get {{ return Task.FromResult(1); }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}([|substitute{call}|], new Exception());
            var y = {method}(value: [|substitute{call}|], ex: new Exception());
            var z = {method}(ex: new Exception(), value: [|substitute{call}|]);
        }}
    }}
}}";

        await VerifyDiagnostic(source, InternalSetupSpecificationDescriptor, message);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual Task<int> Bar {{ get; }}

        protected internal virtual Task<int> FooBar()
        {{
            return Task.FromResult(1);
        }}

        protected internal virtual Task<int> this[int x]
        {{
            get {{ return Task.FromResult(1); }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute{call}, new Exception());
            var y = {method}(value: substitute{call}, ex: new Exception());
            var z = {method}(ex: new Exception(), value: substitute{call});
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}