using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public class SubstituteFactoryCreatePartialMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface()
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(IFoo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(new Type[] { typeof(IFoo)}, null) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(IFoo)}, null) here.");
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate()
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Func<int>)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(new Type[] { typeof(Func<int>)}, null) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Func<int>)}, null) here.");
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1, 2, 3})|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"using System;
using NSubstitute.Core;

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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1 })|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"using System;
using NSubstitute;
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, new object[] { 1 })|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"using System;
using NSubstitute.Core;

namespace MyNamespace
{
    internal class Foo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
        }

        [Fact]
        public override async Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var source = @"using System;
using System.Runtime.CompilerServices;
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
        {
            var source = @"using System;
using System.Runtime.CompilerServices;
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] { typeof(Foo)}, null)|];
        }
    }
}";
            await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
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
            var substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {{typeof(Foo)}}, new object[] {{{invocationValues}}})|];
        }}
    }}
}}";
            await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
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
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {{typeof(Foo)}}, {invocationValues});
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
            return (T) SubstitutionContext.Current.SubstituteFactory.CreatePartial(new Type[] {typeof(T)}, null);
        }
    }
}";
            await VerifyNoDiagnostic(source);
        }
    }
}