using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class WhenAsExtensionMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x.Foo = {arg});
            substitute.{method}(x => {{ x.Foo = {arg}; }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x.Foo = {arg});
            substitute.{method}(x => {{ x.Foo = {arg}; }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? Foo {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x.Foo = {arg});
            substitute.{method}(x => {{ x.Foo = {arg}; }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Action<int> Foo {{ get; set; }}
        Action<int> this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x.Foo = {arg});
            substitute.{method}(x => {{ x.Foo = {arg}; }});
            substitute.{method}(x => x[1] = {arg});
            substitute.{method}(x => {{ x[1] = {arg}; }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x[{arg}] = {arg});
            substitute.{method}(x => {{ x[{arg}] = {arg}; }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x[{arg}] = {arg});
            substitute.{method}(x => {{ x[{arg}] = {arg}; }});
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        int? this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x[{arg}] = {arg});
            substitute.{method}(x => {{ x[{arg}] = {arg}; }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Action<int> Foo {{ get; set; }}
        Action<int> this[int? x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.{method}(x => x.Foo = {arg});
            substitute.{method}(x => {{ x.Foo = {arg}; }});
            substitute.{method}(x => x[1] = {arg});
            substitute.{method}(x => {{ x[1] = {arg}; }});
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string receivedMethod, string arg)
    {
        var source = $@"using System;
    using NSubstitute;
    using NSubstitute.ReceivedExtensions;
    
    namespace MyNamespace
    {{
        public interface Foo
        {{
            void Bar(int? x);
        }}
    
        public class FooTests
        {{
            public void Test()
            {{
                var substitute = Substitute.For<Foo>();
                substitute.{receivedMethod}(x => x.Bar({arg}));
                substitute.{receivedMethod}(x => {{ x.Bar({arg}); }});
            }}
        }}
    }}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("When")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
    using NSubstitute;
    using NSubstitute.ReceivedExtensions;
    
    namespace MyNamespace
    {{
        public interface Foo
        {{
            void Bar(int? x);
        }}
    
        public class FooTests
        {{
            public void Test()
            {{
                var substitute = Substitute.For<Foo>();
                substitute.{method}(x => x.Bar({arg}));
                substitute.{method}(x => {{ x.Bar({arg}); }});
            }}
        }}
    }}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("WhenForAnyArgs")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
    using NSubstitute;
    using NSubstitute.ReceivedExtensions;
    
    namespace MyNamespace
    {{
        public interface Foo
        {{
            void Bar(int? x);
        }}
    
        public class FooTests
        {{
            public void Test()
            {{
                var substitute = Substitute.For<Foo>();
                substitute.{method}(x => x.Bar({arg}));
                substitute.{method}(x => {{ x.Bar({arg}); }});
            }}
        }}
    }}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}