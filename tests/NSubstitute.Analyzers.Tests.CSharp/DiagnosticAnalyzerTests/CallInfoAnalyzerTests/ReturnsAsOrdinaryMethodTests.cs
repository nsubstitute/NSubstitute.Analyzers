using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    public class ReturnsAsOrdinaryMethodTests : CallInfoDiagnosticVerifier
    {
        [Theory]
        [InlineData("callInfo.ArgAt<int>(1);", 18, 17)]
        [InlineData("var x = callInfo[1];", 18, 25)]
        [InlineData("callInfo[1] = 1;", 18, 17)]
        [InlineData("var x = callInfo.Args()[1];", 18, 25)]
        [InlineData("callInfo.Args()[1] = 1;", 18, 17)]
        [InlineData("callInfo.ArgTypes()[1] = typeof(int);", 18, 17)]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>()), callInfo =>
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
        [InlineData("callInfo.ArgAt<int>(0);")]
        [InlineData("callInfo.ArgAt<int>(1);")]
        [InlineData("var x = callInfo[0];")]
        [InlineData("var x = callInfo[1];")]
        [InlineData("var x = callInfo.Args()[0];")]
        [InlineData("var x = callInfo.Args()[1];")]
        [InlineData("var x = callInfo.ArgTypes()[0];")]
        [InlineData("var x = callInfo.ArgTypes()[1];")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, int y);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>(), Arg.Any<int>()), callInfo =>
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
        [InlineData("callInfo.ArgAt<Bar>(1);", 22, 17)]
        [InlineData("var x = (Bar)callInfo[1];", 22, 30)]
        [InlineData("var x = callInfo[1] as Bar;", 22, 25)]
        [InlineData("var x = (Bar)callInfo.Args()[1];", 22, 30)]
        [InlineData("var x = callInfo.Args()[1] as Bar;", 22, 25)]
        public override async Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x, double y);
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>(), Arg.Any<double>()), callInfo =>
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
        [InlineData("callInfo.ArgAt<Bar>(0);")]
        [InlineData("var x = (Bar)callInfo[0];")]
        [InlineData("var x = callInfo[0] as Bar;")]
        [InlineData("var x = (Bar)callInfo.Args()[0];")]
        [InlineData("var x = callInfo.Args()[0] as Bar;")]
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<Bar>()), callInfo =>
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
        [InlineData("var x = callInfo.ArgTypes() as object;")]
        [InlineData("var x = (object)callInfo.ArgTypes();")]
        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<Bar>()), callInfo =>
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
        [InlineData("callInfo.ArgTypes()[0] = typeof(object);")]
        [InlineData("callInfo.Args()[0] = 1m;")]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(Bar x);
    }}

    public class Bar
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<Bar>()), callInfo =>
            {{
                {argAccess}
                return 1;
            }});
        }}
    }}
}}";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation()
        {
            var source = @"using System;
using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>()), callInfo =>
            {
                callInfo.Arg<double>();
                return 1;
            });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoCouldNotFindArgumentToThisCall,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not find an argument of type double to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(18, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation()
        {
            var source = @"using System;
using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(int x);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>()), callInfo =>
            {
                callInfo.Arg<int>();
                return 1;
            });
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(int x, int y);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>(), Arg.Any<int>()), callInfo =>
            {
                callInfo.Arg<int>();
                return 1;
            });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoMoreThanOneArgumentOfType,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is more than one argument of type int to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 17)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(int x, double y);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>(), Arg.Any<double>()), callInfo =>
            {
                callInfo.Arg<int>();
                return 1;
            });
        }
    }
}";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface Foo
    {
        int Bar(int x, double y);
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            SubstituteExtensions.Returns(substitute.Bar(Arg.Any<int>(), Arg.Any<double>()), callInfo =>
            {
                callInfo[1] = 1;
                return 1;
            });
        }
    }
}";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentIsNotOutOrRef,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not set argument 1 (double) as it is not an out or ref argument.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 17)
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
            SubstituteExtensions.Returns(substitute.Bar(ref value), callInfo =>
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
            SubstituteExtensions.Returns(substitute.Bar(out value), callInfo =>
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
            SubstituteExtensions.Returns(substitute.Bar(out value), callInfo =>
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
            SubstituteExtensions.Returns(substitute.Bar(out value), callInfo =>
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
            SubstituteExtensions.Returns(substitute.Bar(out value), callInfo =>
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