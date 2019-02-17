using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests
{
    [CombinatoryData("Returns")]
    public class ReturnsAsExtensionMethodTests : InternalSetupSpecificationCodeFixProviderVerifier
    {
        public override async Task AppendsProtectedInternal_ToIndexer_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
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
            var x = substitute[0].{method}(1);
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
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
            var x = substitute[0].{method}(1);
        }}
    }}
}}";

            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsProtectedInternal_ToProperty_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar.{method}(1);
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual int Bar
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar.{method}(1);
        }}
    }}
}}";

            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsProtectedInternal_ToMethod_WhenUsedWithInternalMember(string method)
        {
            var oldSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int Bar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar().{method}(1);
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
        protected internal virtual int Bar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar().{method}(1);
        }}
    }}
}}";
            await VerifyFix(oldSource, newSource, 0);
        }

        public override async Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalMember(string method, string call)
        {
            var oldSource = $@"using NSubstitute;
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
            var x = substitute{call}.{method}(1);
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

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
            var x = substitute{call}.{method}(1);
        }}
    }}
}}";
            await VerifyFix(oldSource, newSource, 1);
        }
    }
}