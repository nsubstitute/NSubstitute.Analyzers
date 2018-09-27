using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SubstituteForInternalMemberCodeFixProviderTests
{
    public class SubstituteFactoryCreatePartialMethodTests : SubstituteForInternalMemberCodeFixVerifier
    {
        [Fact]
        public override async Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass()
        {
            var oldSource = @"using NSubstitute.Core;
namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        internal class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
            }
        }
    }
}";
            var newSource = @"using NSubstitute.Core;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    namespace MyInnerNamespace
    {
        internal class Foo
        {
        }

        public class FooTests
        {
            public void Test()
            {
                var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
            }
        }
    }
}";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass()
        {
            var oldSource = @"using NSubstitute.Core;
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            var newSource = @"using NSubstitute.Core;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty()
        {
            var oldSource = @"using System.Reflection;
using NSubstitute.Core;
[assembly: AssemblyVersion(""1.0.0"")]
namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            var newSource = @"using System.Reflection;
using NSubstitute.Core;
[assembly: AssemblyVersion(""1.0.0"")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass()
        {
            var oldSource = @"using NSubstitute.Core;
namespace MyNamespace
{
    internal class Foo
    {
        internal class Bar
        {

        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            var newSource = @"using NSubstitute.Core;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
        internal class Bar
        {

        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public override async Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass()
        {
            var oldSource = @"using NSubstitute.Core;
namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Foo)}, null);
        }
    }
}";
            await VerifyFix(oldSource, oldSource);
        }
    }
}