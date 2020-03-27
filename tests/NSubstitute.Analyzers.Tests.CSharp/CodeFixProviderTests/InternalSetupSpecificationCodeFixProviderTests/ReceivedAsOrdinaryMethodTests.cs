using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.InternalSetupSpecificationCodeFixProviderTests
{
    [CombinatoryData("SubstituteExtensions.Received")]
    public class ReceivedAsOrdinaryMethodTests : InternalSetupSpecificationCodeFixProviderVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedAnalyzer();

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
            var x = {method}(substitute)[0];
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
            var x = {method}(substitute)[0];
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
            var x = {method}(substitute).Bar;
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
            var x = {method}(substitute).Bar;
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
            var x = {method}(substitute).Bar();
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
            var x = {method}(substitute).Bar();
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
            var x = {method}(substitute)[0];
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
            var x = {method}(substitute)[0];
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
            var x = {method}(substitute).Bar;
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
            var x = {method}(substitute).Bar;
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
            var x = {method}(substitute).Bar();
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
            var x = {method}(substitute).Bar();
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
            var x = {method}(substitute){call};
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(""OtherAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

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
            var x = {method}(substitute){call};
        }}
    }}
}}";
            await VerifyFix(oldSource, newSource, 2);
        }

        public override async Task AppendsInternalsVisibleToWithFullyQualifiedName_WhenUsedWithInternalMemberAndCompilerServicesNotImported(string method)
        {
            var oldSource = $@"using NSubstitute;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherAssembly"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute).FooBar();
        }}
    }}
}}";

            var newSource = $@"using NSubstitute;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""OtherAssembly"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{{
    public class Foo
    {{
        internal virtual int FooBar()
        {{
            return 1;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = {method}(substitute).FooBar();
        }}
    }}
}}";
            await VerifyFix(oldSource, newSource, 2);
        }
    }
}