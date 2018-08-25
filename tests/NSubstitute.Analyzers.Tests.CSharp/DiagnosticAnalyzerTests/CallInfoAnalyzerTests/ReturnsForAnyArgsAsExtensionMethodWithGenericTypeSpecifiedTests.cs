using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    public class ReturnsForAnyArgsAsExtensionMethodWithGenericTypeSpecifiedTests : CallInfoDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgAt<int>(1);", 22, 17)]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[1];", 22, 25)]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;", 22, 17)]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo.Args()[1];", 22, 25)]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.Args()[1] = 1;", 22, 17)]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo.ArgTypes()[1] = typeof(int);", 22, 17)]
        [InlineData("substitute.Barr", "callInfo.ArgAt<int>(1);", 22, 17)]
        [InlineData("substitute.Barr", "var x = callInfo[1];", 22, 25)]
        [InlineData("substitute.Barr", "callInfo[1] = 1;", 22, 17)]
        [InlineData("substitute.Barr", "var x = callInfo.Args()[1];", 22, 25)]
        [InlineData("substitute.Barr", "callInfo.Args()[1] = 1;", 22, 17)]
        [InlineData("substitute.Barr", "callInfo.ArgTypes()[1] = typeof(int);", 22, 17)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgAt<int>(1);", 22, 17)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[1];", 22, 25)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;", 22, 17)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo.Args()[1];", 22, 25)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.Args()[1] = 1;", 22, 17)]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo.ArgTypes()[1] = typeof(int);", 22, 17)]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentOutOfRange,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is no argument at position 1",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.Args()[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]", "var x = callInfo.ArgTypes()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "callInfo.ArgAt<int>(0);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "callInfo.ArgAt<int>(1);")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.Args()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.ArgTypes()[0];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())", "var x = callInfo.ArgTypes()[1];")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int Barr {{ get; }}

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (BarBase)callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (object)callInfo[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (BarBase)callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = (object)callInfo.Args()[1];")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo[1] as BarBase;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo[1] as object;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo.Args()[1] as BarBase;")]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<Bar>())", "var x = callInfo.Args()[1] as object;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (BarBase)callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (object)callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (BarBase)callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = (object)callInfo.Args()[1];")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo[1] as BarBase;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo[1] as object;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo.Args()[1] as BarBase;")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<Bar>()]", "var x = callInfo.Args()[1] as object;")]
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToAssignableTypeForIndirectCasts(string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, Bar y);

        int this[int x, Bar y] {{ get; }}
    }}

    public class BarBase
    {{
    }}

    public class Bar : BarBase
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "callInfo.ArgAt<Bar>(1);", 32, 17)]
        [InlineData("substitute.Foo(Arg.Any<int>(), Arg.Any<FooBar>())", "callInfo.ArgAt<Bar>(1);", 32, 17)]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = (Bar)callInfo[1];", 32, 30)]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = callInfo[1] as Bar;", 32, 25)]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = (Bar)callInfo.Args()[1];", 32, 30)]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())", "var x = callInfo.Args()[1] as Bar;", 32, 25)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "callInfo.ArgAt<Bar>(1);", 32, 17)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = (Bar)callInfo[1];", 32, 30)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = callInfo[1] as Bar;", 32, 25)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = (Bar)callInfo.Args()[1];", 32, 30)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]", "var x = callInfo.Args()[1] as Bar;", 32, 25)]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<FooBar>()]", "callInfo.ArgAt<Bar>(1);", 32, 17)]
        public override async Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int Foo(int x, FooBar bar);

        int this[int x, double y] {{ get; }}

        int this[int x, FooBar bar] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooBar : Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoCouldNotConvertParameterAtPosition,
                Severity = DiagnosticSeverity.Warning,
                Message = "Couldn't convert parameter at position 1 to type MyNamespace.Bar.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "callInfo.ArgAt<Bar>(0);")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (Bar)callInfo[0];")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo[0] as Bar;")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (Bar)callInfo.Args()[0];")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo.Args()[0] as Bar;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "callInfo.ArgAt<Bar>(0);")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (Bar)callInfo[0];")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo[0] as Bar;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (Bar)callInfo.Args()[0];")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo.Args()[0] as Bar;")]
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo.ArgTypes() as object;")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = callInfo.ArgTypes()[0] as object;")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (object)callInfo.ArgTypes();")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "var x = (object)callInfo.ArgTypes()[0];")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo.ArgTypes() as object;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = callInfo.ArgTypes()[0] as object;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (object)callInfo.ArgTypes();")]
        [InlineData("substitute[Arg.Any<Bar>()]", "var x = (object)callInfo.ArgTypes()[0];")]
        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "callInfo.ArgTypes()[0] = typeof(object);")]
        [InlineData("substitute.Bar(Arg.Any<Bar>())", "callInfo.Args()[0] = 1m;")]
        [InlineData("substitute[Arg.Any<Bar>()]", "callInfo.ArgTypes()[0] = typeof(object);")]
        [InlineData("substitute[Arg.Any<Bar>()]", "callInfo.Args()[0] = 1m;")]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);

        int this[Bar x] {{ get; }}
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>())")]
        [InlineData("substitute.Barr")]
        [InlineData("substitute[Arg.Any<int>()]")]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                callInfo.Arg<double>();
                return 1;
            }});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoCouldNotFindArgumentToThisCall,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not find an argument of type double to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(22, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>())")]
        [InlineData("substitute[Arg.Any<int>()]")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                callInfo.Arg<int>();
                return 1;
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<int>())")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<int>()]")]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);

        int this[int x, int y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                callInfo.Arg<int>();
                return 1;
            }});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoMoreThanOneArgumentOfType,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is more than one argument of type int to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string call)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                callInfo.Arg<int>();
                return 1;
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any<int>(), Arg.Any<double>())")]
        [InlineData("substitute[Arg.Any<int>(), Arg.Any<double>()]")]
        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string call)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);

        int this[int x, double y] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.ReturnsForAnyArgs<int>(callInfo =>
            {{
                callInfo[1] = 1;
                return 1;
            }});
        }}
    }}
}}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentIsNotOutOrRef,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not set argument 1 (double) as it is not an out or ref argument.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(19, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(ref int x);
    }

    public class FooTests
    {
        public void Test()
        {
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(ref value).ReturnsForAnyArgs<int>(callInfo =>
            {
                callInfo[0] = 1;
                return 1;
            });
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(out int x);
    }

    public class FooTests
    {
        public void Test()
        {
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).ReturnsForAnyArgs<int>(callInfo =>
            {
                callInfo[0] = 1;
                return 1;
            });
        }
    }
}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(out int x);
    }

    public class FooTests
    {
        public void Test()
        {
            int value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).ReturnsForAnyArgs<int>(callInfo =>
            {
                callInfo[1] = 1;
                return 1;
            });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentOutOfRange,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is no argument at position 1",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(out decimal x);
    }

    public class FooTests
    {
        public void Test()
        {
            decimal value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).ReturnsForAnyArgs<int>(callInfo =>
            {
                callInfo[0] = 1;
                return 1;
            });
        }
    }
}";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentSetWithIncompatibleValue,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not set value of type int to argument 0 (decimal) because the types are incompatible.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(out decimal x);
    }

    public class FooTests
    {
        public void Test()
        {
            decimal value = 0;
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(out value).ReturnsForAnyArgs<int>(callInfo =>
            {
                callInfo[0] = 1M;
                return 1;
            });
        }
    }
}";

            await VerifyDiagnostic(source);
        }
    }
}