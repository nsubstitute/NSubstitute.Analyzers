using System.Threading.Tasks;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonVirtualSetupAnalyzerSuppressDiagnosticsCodeFixProviderTests
{
    public class ReturnsForAnyArgsAsOrdinaryMethodTests : DefaultSuppressDiagnosticsCodeFixProviderVerifier
    {
        public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualMethod()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1);
        }
    }
}";
            await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
        }

        public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForStaticMethod()
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
            SubstituteExtensions.ReturnsForAnyArgs(Foo.Bar(), 1);
        }
    }
}";
            await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
        }

        public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForSealedOverrideMethod()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar(), 1);
        }
    }
}";

            await VerifySuppressionSettings(source, "M:MyNamespace.Foo2.Bar~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);
        }

        public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualProperty()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute.Bar, 1);
        }
    }
}";

            await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);
        }

        public override async Task SuppressesDiagnosticsInSettings_WhenSettingValueForNonVirtualIndexer()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute[1], 1);
        }
    }
}";

            await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);
        }

        public override async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute[1], 1);
        }
    }
}";

            await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification, 1);
        }

        public override async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
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
            SubstituteExtensions.ReturnsForAnyArgs(substitute[1], 1);
        }
    }
}";

            await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification, 2);
        }
    }
}