using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Analyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.AnalyzerTests.ReturnValueAnalyzerTests
{
    public class ReturnByReturnsForAnyArgsMethodTests : ReturnValueAnalyzerTest
    {
        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualMethod()
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
            substitute.Bar().ReturnsForAnyArgs(1);
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ReturnValueAnalyzer.DiagnosticId,
                Severity = DiagnosticSeverity.Warning,
                Message = "Type name '{0}' contains lowercase letters",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 30)
                }
            };

            await VerifyDiagnostics(source, expectedDiagnostic);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualMethod()
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

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar().ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract int Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar().ReturnsForAnyArgs(1);
        }
    }
}";

            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
    }

    public class Foo : IFoo
    {
        public int Bar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar().ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar { get; }
    }

    public class Foo : IFoo
    {
        public int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo<T>
    {
        int Bar();
    }

    public class Foo<T> : IFoo<T>
    {
        public int Bar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute.Bar().ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }

        [Fact(Skip = "Finding interface implementations for generic methods not supported yet")]
        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForGenericInterfaceGenericMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo<T>
    {
        int Bar<TT>();
    }

    public class Foo<T> : IFoo<T>
    {
        public int Bar<TT>()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute.Bar<int>().ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForAbstractProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public abstract class Foo
    {
        public abstract int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.ReturnsForAnyArgs(1);
        }
    }
}";

            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsNoDiagnostic_WhenSettingValueForVirtualProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.ReturnsForAnyArgs(1);
        }
    }
}";

            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsDiagnostic_WhenSettingValueForNonVirtualProperty()
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
            substitute.Bar.ReturnsForAnyArgs(1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ReturnValueAnalyzer.DiagnosticId,
                Severity = DiagnosticSeverity.Warning,
                Message = "Type name '{0}' contains lowercase letters",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 28)
                }
            };

            await VerifyDiagnostics(source, expectedDiagnostic);
        }


        public override async Task AnalyzerReturnsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public virtual int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].ReturnsForAnyArgs(1);
        }
    }
}";
            await VerifyDiagnostics(source);
        }


        public override async Task AnalyzerReturnsDiagnostics_WhenSettingValueForNonVirtualIndexer()
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
            substitute[1].ReturnsForAnyArgs(1);
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = ReturnValueAnalyzer.DiagnosticId,
                Severity = DiagnosticSeverity.Warning,
                Message = "Type name '{0}' contains lowercase letters",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 27)
                }
            };

            await VerifyDiagnostics(source, expectedDiagnostic);
        }
    }
}