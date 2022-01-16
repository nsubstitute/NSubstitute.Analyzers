using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsOrdinaryMethodTests : SyncOverAsyncThrowsCodeFixVerifier
{
    public override async Task ReplacesThrowsWithReturns_WhenUsedInMethod(string method, string updatedMethod)
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
            ExceptionExtensions.{method}(substitute.Bar(), new Exception());
            ExceptionExtensions.{method}(value: substitute.Bar(), ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute.Bar());
        }}
    }}
}}";

        var newSource = $@"using System;
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
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithReturns_WhenUsedInProperty(string method, string updatedMethod)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ExceptionExtensions.{method}(substitute.Bar, new Exception());
            ExceptionExtensions.{method}(value: substitute.Bar, ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute.Bar);
        }}
    }}
}}";

        var newSource = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithReturns_WhenUsedInIndexer(string method, string updatedMethod)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task this[int x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ExceptionExtensions.{method}(substitute[0], new Exception());
            ExceptionExtensions.{method}(value: substitute[0], ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute[0]);
        }}
    }}
}}";

        var newSource = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task this[int x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            SubstituteExtensions.{updatedMethod}(substitute[0], Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute[0], Task.FromException(new Exception()));
            SubstituteExtensions.{updatedMethod}(substitute[0], Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }
}