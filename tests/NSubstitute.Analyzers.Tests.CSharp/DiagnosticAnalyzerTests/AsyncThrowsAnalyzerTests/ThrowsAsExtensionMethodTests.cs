using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.AsyncThrowsAnalyzerTests
{
    public class ThrowsAsExtensionMethodTests : AsyncThrowsDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsedWithNonGenericAsyncMethod(string method)
        {
            var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            [|substitute.Bar().{method}(new Exception())|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, AsyncThrowsDescriptor);
        }

        public override async Task ReportsDiagnostic_WhenUsedWithGenericAsyncMethod(string method)
        {
            var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task<object> Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            [|substitute.Bar().{method}(new Exception())|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, AsyncThrowsDescriptor);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedWithSyncMethod(string method)
        {
            var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().{method}(new Exception());
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}