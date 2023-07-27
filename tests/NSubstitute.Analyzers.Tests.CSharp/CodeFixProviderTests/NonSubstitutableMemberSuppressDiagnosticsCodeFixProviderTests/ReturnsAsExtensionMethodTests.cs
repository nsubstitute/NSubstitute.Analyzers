using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonSubstitutableMemberSuppressDiagnosticsCodeFixProviderTests;

public class ReturnsAsExtensionMethodTests : NonSubstitutableMemberSuppressDiagnosticsCodeFixVerifier
{
    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualMethod()
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
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar().Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithStaticMethod()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public static int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            Foo.Bar().Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithExtensionMethod()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            substitute.GetBar().Returns(1);
        }
    }

    public static class MyExtensions
    {
        public static IBar Bar { get; set; }

        public static int GetBar(this object @object)
        {
            return Bar.Foo(@object);
        }
    }

    public interface IBar
    {
        int Foo(object @obj);
    }
}";

        await VerifySuppressionSettings(source, "M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithSealedOverrideMethod()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar()
        {
            return 2;
        }
    }

    public class Foo2 : Foo
    {
        public sealed override int Bar() => 1;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            substitute.Bar().Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "M:MyNamespace.Foo2.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualProperty()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettings_WhenUsedWithNonVirtualIndexer()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }

    public override async Task SuppressesDiagnosticsInSettingsForClass_WhenUsedWithNonVirtualMember_AndSelectingClassSuppression()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification, 1);
    }

    public override async Task SuppressesDiagnosticsInSettingsForNamespace_WhenUsedWithNonVirtualMember_AndSelectingNamespaceSuppression()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].Returns(1);
        }
    }
}";

        await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification, 2);
    }
}