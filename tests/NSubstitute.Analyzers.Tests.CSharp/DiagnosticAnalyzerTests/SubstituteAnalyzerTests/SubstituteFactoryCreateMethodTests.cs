using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public class SubstituteFactoryCreateMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForInterface()
        {
            var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, null);
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
        {
            var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForDelegate()
        {
            var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Func<int>)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
        {
            var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Func<int>)}, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainsMultipleClasses()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo), typeof(Bar)}, null);
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleSameClasses()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo), typeof(Foo)}, null);
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleGenericTypeParameters_ContainsMultipleInterfaces()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public interface IBar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(IBar)}, null);

        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsInterfaceNotImplementedByClass()
        {
            var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(Bar) }, null);
        }
    }
}";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainsClassWithoutMatchingConstructor()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class Bar
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(IFoo), typeof(Bar) }, new object[] { 1 });
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
        private Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1, 2, 3});
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1 });
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public class Foo
    {
        public Foo(int x, int y = 1)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, new object[]{ 1 });
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"using NSubstitute.Core;

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
        }
    }
}
";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task
            ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var source = @"using System.Runtime.CompilerServices;
using NSubstitute.Core;

[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public override async Task
            ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
        {
            var source = @"using System.Runtime.CompilerServices;
using NSubstitute.Core;

[assembly: InternalsVisibleTo(""SomeValue"")]

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Foo)}, null);
        }
    }
}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("decimal x", "1")] // valid c# but doesnt work in NSubstitute
        [InlineData("int x", "1m")]
        [InlineData("int x", "1D")]
        [InlineData("List<int> x", "new List<int>().AsReadOnly()")]
        [InlineData("int x", "new object()")]
        public override async Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"using System.Collections.Generic;
using NSubstitute.Core;

namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{

        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, new object[] {{{invocationValues}}});
        }}
    }}
}}";
            var expectedDiagnostic = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.CallInfoArgumentSetWithIncompatibleValue;
            expectedDiagnostic.OverrideMessage(expectedMessage);

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("int x", "new object [] { 1 }")]
        [InlineData("float x", "new object [] { 'c' }")]
        [InlineData("int x", "new object [] { 'c' }")]
        [InlineData("IList<int> x", "new object [] { new List<int>() }")]
        [InlineData("IEnumerable<int> x", "new object [] { new List<int>() }")]
        [InlineData("IEnumerable<int> x", "new object [] { new List<int>().AsReadOnly() }")]
        [InlineData("IEnumerable<char> x", @"new object [] { ""value"" }")]
        [InlineData("", @"new object[] { }")]
        [InlineData("", "new object[] { 1, 2 }.ToArray()")] // actual values known at runtime only so constructor analysys skipped
        public override async Task ReturnsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace MyNamespace
{{
    public class Foo
    {{
        public Foo({ctorValues})
        {{

        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {{typeof(Foo)}}, {invocationValues});
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument()
        {
            var source = @"using System;
using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{

    public class FooTests
    {
        public T Foo<T>() where T : class
        {
            return (T) SubstitutionContext.Current.SubstituteFactory.Create(new Type[] {typeof(T)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }
    }
}