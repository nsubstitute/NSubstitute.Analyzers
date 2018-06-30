using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupWhenAnalyzerTests
{
    public class WhenAsExtensionMethodTests : CSharpDiagnosticVerifier
    {
        [Theory]
        [InlineData("sub => sub.Bar()", 36)]
        [InlineData("delegate(Foo sub) { sub.Bar(); }", 49)]
        [InlineData("sub => { sub.Bar(); }", 38)]
        public async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedColumn)
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
            substitute.When({whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("sub => sub.Bar()")]
        [InlineData("delegate(Foo sub) { sub.Bar(); }")]
        [InlineData("sub => { sub.Bar(); }")]
        public async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction)
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
            substitute.When({whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("sub => { var x = sub.Bar; }", 46)]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }", 57)]
        public async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedColumn)
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
            substitute.When({whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("sub => { var x = sub.Bar; }")]
        [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
        public async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction)
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
            substitute.When({whenAction}).Do(callInfo => i++);
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("sub => { var x = sub[1]; }", 46)]
        [InlineData("delegate(Foo sub) { var x = sub[1]; }", 57)]
        public async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedColumn)
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
            substitute.When({whenAction}).Do(callInfo => i++);
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InSeparateFunction()
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

            substitute.When(SubstituteCall).Do(callInfo => i++);
            substitute.Bar();
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.NonVirtualWhenSetupSpecification,
                Severity = DiagnosticSeverity.Warning,
                Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(16, 15)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupWhenAnalyzer();
        }
    }
}