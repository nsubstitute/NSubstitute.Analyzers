using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public class ThrowsForAnyArgsAsExtensionMethodWithGenericTypeSpecifiedTests : NonVirtualSetupDiagnosticVerifier
    {
        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            {literal}.ThrowsForAnyArgs<Exception>();
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForStaticMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            Foo.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
        public override int Bar() => 1;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo2>();
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var returnValue = substitute.Bar();
            returnValue.ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForDelegate()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = Substitute.For<Func<int>>();
            substitute().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public interface IFoo<T>
    {
        int Bar<T>();
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo<int>>();
            substitute.Bar<int>().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar.ThrowsForAnyArgs<Exception>();
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public interface IFoo
    {
        int this[int i] { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute[1].ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar.ThrowsForAnyArgs<Exception>();
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute.Bar.ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute[1].ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            substitute[1].ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod()
        {
            var source = @"
using System;
namespace NSubstitute
{
    public class Foo
    {
        public int Bar()
        {
            return 1;
        }
    }

    public static class ExceptionExtensions
    {
        public static T ThrowsForAnyArgs<T>(this object value) where T: Exception
        {
            return default(T);
        }
    }

    public class FooTests
    {
        public void Test()
        {
            Foo substitute = null;
            substitute.Bar().ThrowsForAnyArgs<Exception>();
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }

        public int FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
            substitute.FooBar.ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Bar", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo<T>
    {
        public T Bar { get; }

        public int FooBar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
            substitute.FooBar.ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int x)
        {
            return 1;
        }

        public int Bar(int x, int y)
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(1, 2).ThrowsForAnyArgs<Exception>();
            substitute.Bar(1).ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.Foo.Bar``1(``0,``0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int x)
        {
            return 1;
        }

        public int Bar<T>(T x, T y)
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar<int>(1, 2).ThrowsForAnyArgs<Exception>();
            substitute.Bar(1).ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo.Item(System.Int32,System.Int32)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
        public int this[int x, int y] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1, 2].ThrowsForAnyArgs<Exception>();
            substitute[1].ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("P:MyNamespace.Foo`1.Item(`0,`0)", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo<T>
    {
        public int this[T x] => 0;
        public int this[T x, T y] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute[1, 2].ThrowsForAnyArgs<Exception>();
            substitute[1].ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType()
        {
             Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo", DiagnosticIdentifiers.NonVirtualSetupSpecification);

             var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooBarBar
    {
        public int Bar { get;set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].ThrowsForAnyArgs<Exception>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
            substitute.FooBar().ThrowsForAnyArgs<Exception>();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            substituteFooBarBar[1].ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.Bar.ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.FooBar().ThrowsForAnyArgs<Exception>();
        }
    }
}";

             var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(38, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(39, 13)
                    }
                }
            };

             await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("T:MyNamespace.Foo`1", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class Foo<T>
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooBarBar<T>
    {
        public int Bar { get;set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo<int>>();
            substitute[1].ThrowsForAnyArgs<Exception>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
            substitute.FooBar().ThrowsForAnyArgs<Exception>();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar<int>>();
            substituteFooBarBar[1].ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.Bar.ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.FooBar().ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(37, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(38, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(39, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("N:MyNamespace", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyOtherNamespace
{
    public class FooBarBar
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }
}

namespace MyNamespace
{
    using MyOtherNamespace;
    public class Foo
    {
        public int Bar { get; set; }
        public int this[int x] => 0;
        public int FooBar()
        {
            return 1;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].ThrowsForAnyArgs<Exception>();
            substitute.Bar.ThrowsForAnyArgs<Exception>();
            substitute.FooBar().ThrowsForAnyArgs<Exception>();

            var substituteFooBarBar = NSubstitute.Substitute.For<FooBarBar>();
            substituteFooBarBar[1].ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.Bar.ThrowsForAnyArgs<Exception>();
            substituteFooBarBar.FooBar().ThrowsForAnyArgs<Exception>();
        }
    }
}";

            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member this[] can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(41, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member Bar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(42, 13)
                    }
                },
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member FooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(43, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod()
        {
            Settings = AnalyzersSettings.CreateWithSuppressions("M:MyNamespace.MyExtensions.GetBar(System.Object)~System.Int32", DiagnosticIdentifiers.NonVirtualSetupSpecification);

            var source = @"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            MyExtensions.Bar = Substitute.For<IBar>();
            var substitute = Substitute.For<object>();
            substitute.GetBar().ThrowsForAnyArgs<Exception>();
            substitute.GetFooBar().ThrowsForAnyArgs<Exception>();
        }
    }

    public static class MyExtensions
    {
        public static IBar Bar { get; set; }

        public static int GetBar(this object @object)
        {
            return Bar.Foo(@object);
        }

        public static int GetFooBar(this object @object)
        {
            return 1;
        }
    }

    public interface IBar
    {
        int Foo(object @obj);
    }
}";
            var expectedDiagnostic = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticIdentifiers.NonVirtualSetupSpecification,
                    Severity = DiagnosticSeverity.Warning,
                    Message = "Member GetFooBar can not be intercepted. Only interface members and virtual, overriding, and abstract members can be intercepted.",
                    Locations = new[]
                    {
                        new DiagnosticResultLocation(14, 13)
                    }
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }
    }
}