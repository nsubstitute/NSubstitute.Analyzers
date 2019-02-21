using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests;
using NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberWhenAnalyzerTests;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests
{
    [CombinatoryData("Received")]
    public class ReceivedAsExtensionMethodTests : InternalSetupSpecificationCodeFixProviderVerifier
    {
        public override async Task ChangesInternalToPublic_ForIndexer_WhenUsedWithInternalMember(string method)
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
            var x = substitute.{method}()[0];
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;

namespace MyNamespace
{{
    public class Foo
    {{
        public virtual int this[int x]
        {{
            get {{ return 1; }}
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.{method}()[0];
        }}
    }}
}}";

            await VerifyFix(oldSource, newSource, 1);
        }

        public override async Task ChangesInternalToPublic_ForProperty_WhenUsedWithInternalMember(string method)
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
        public virtual int Bar
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

            await VerifyFix(oldSource, newSource, 1);
        }

        public override async Task ChangesInternalToPublic_ForMethod_WhenUsedWithInternalMember(string method)
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
        public virtual int Bar()
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
            await VerifyFix(oldSource, newSource, 1);
        }

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
            await VerifyFix(oldSource, newSource, 2);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonSubstitutableMemberReceivedAnalyzer();
        }
    }
}