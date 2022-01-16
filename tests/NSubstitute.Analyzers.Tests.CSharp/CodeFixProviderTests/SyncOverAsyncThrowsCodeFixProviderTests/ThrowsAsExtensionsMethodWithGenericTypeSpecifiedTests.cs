using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsExtensionsMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsCodeFixVerifier
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
            substitute.Bar().{method}<Exception>();
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
            substitute.Bar().{updatedMethod}(Task.FromException(new Exception()));
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
            substitute.Bar.{method}<Exception>();
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
            substitute.Bar.{updatedMethod}(Task.FromException(new Exception()));
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
            substitute[0].{method}<Exception>();
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
            substitute[0].{updatedMethod}(Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }
}