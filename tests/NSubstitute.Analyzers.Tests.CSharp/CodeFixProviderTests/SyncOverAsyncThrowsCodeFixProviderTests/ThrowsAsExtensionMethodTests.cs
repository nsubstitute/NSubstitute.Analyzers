using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsExtensionMethodTests : SyncOverAsyncThrowsCodeFixVerifier
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
            substitute.Bar().{method}(new Exception());
            substitute.Bar().{method}(ex: new Exception());
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
            substitute.Bar().{updatedMethod}(Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
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
            substitute.Bar.{method}(new Exception());
            substitute.Bar.{method}(ex: new Exception());
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
            substitute.Bar.{updatedMethod}(Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
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
            substitute[0].{method}(new Exception());
            substitute[0].{method}(ex: new Exception());
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
            substitute[0].{updatedMethod}(Task.FromException(new Exception()));
        }}
    }}
}}";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInMethod(string method, string updatedMethod)
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
            substitute.Bar().{method}(new Exception());
            substitute.Bar().{method}(ex: new Exception());
            substitute.Bar().{method}(callInfo => new Exception());
            substitute.Bar().{method}(createException: callInfo => new Exception());
            substitute.Bar().{method}(callInfo => {{ return new Exception(); }});
            substitute.Bar().{method}(createException: callInfo => {{ return new Exception(); }});
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
            substitute.Bar().{updatedMethod}(new Exception());
            substitute.Bar().{updatedMethod}(ex: new Exception());
            substitute.Bar().{updatedMethod}(callInfo => new Exception());
            substitute.Bar().{updatedMethod}(createException: callInfo => new Exception());
            substitute.Bar().{updatedMethod}(callInfo => {{ return new Exception(); }});
            substitute.Bar().{updatedMethod}(createException: callInfo => {{ return new Exception(); }});
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInProperty(string method, string updatedMethod)
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
            substitute.Bar.{method}(new Exception());
            substitute.Bar.{method}(ex: new Exception());
            substitute.Bar.{method}(callInfo => new Exception());
            substitute.Bar.{method}(createException: callInfo => new Exception());
            substitute.Bar.{method}(callInfo => {{ return new Exception(); }});
            substitute.Bar.{method}(createException: callInfo => {{ return new Exception(); }});
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
            substitute.Bar.{updatedMethod}(new Exception());
            substitute.Bar.{updatedMethod}(ex: new Exception());
            substitute.Bar.{updatedMethod}(callInfo => new Exception());
            substitute.Bar.{updatedMethod}(createException: callInfo => new Exception());
            substitute.Bar.{updatedMethod}(callInfo => {{ return new Exception(); }});
            substitute.Bar.{updatedMethod}(createException: callInfo => {{ return new Exception(); }});
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInIndexer(string method, string updatedMethod)
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
            substitute[0].{method}(new Exception());
            substitute[0].{method}(ex: new Exception());
            substitute[0].{method}(callInfo => new Exception());
            substitute[0].{method}(createException: callInfo => new Exception());
            substitute[0].{method}(callInfo => {{ return new Exception(); }});
            substitute[0].{method}(createException: callInfo => {{ return new Exception(); }});
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
            substitute[0].{updatedMethod}(new Exception());
            substitute[0].{updatedMethod}(ex: new Exception());
            substitute[0].{updatedMethod}(callInfo => new Exception());
            substitute[0].{updatedMethod}(createException: callInfo => new Exception());
            substitute[0].{updatedMethod}(callInfo => {{ return new Exception(); }});
            substitute[0].{updatedMethod}(createException: callInfo => {{ return new Exception(); }});
        }}
    }}
}}";

        await VerifyFix(source, newSource);
    }
}