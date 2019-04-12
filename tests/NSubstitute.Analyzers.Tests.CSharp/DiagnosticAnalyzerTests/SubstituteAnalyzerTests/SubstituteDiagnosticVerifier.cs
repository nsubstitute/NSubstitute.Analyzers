using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzerTests
{
    public abstract class SubstituteDiagnosticVerifier : CSharpDiagnosticVerifier, ISubstituteAnalyzerVerifier
    {
        protected DiagnosticDescriptor SubstituteConstructorArgumentsForInterfaceDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteConstructorArgumentsForInterface;

        protected DiagnosticDescriptor SubstituteConstructorArgumentsForDelegateDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteConstructorArgumentsForDelegate;

        protected DiagnosticDescriptor SubstituteMultipleClassesDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteMultipleClasses;

        protected DiagnosticDescriptor SubstituteForConstructorParametersMismatchDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteForConstructorParametersMismatch;

        protected DiagnosticDescriptor SubstituteForWithoutAccessibleConstructorDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteForWithoutAccessibleConstructor;

        protected DiagnosticDescriptor SubstituteForInternalMemberDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteForInternalMember;

        protected DiagnosticDescriptor SubstituteConstructorMismatchDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SubstituteConstructorMismatch;

        protected DiagnosticDescriptor PartialSubstituteForUnsupportedTypeDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.PartialSubstituteForUnsupportedType;

#pragma warning disable xUnit1013 // Public method should be marked as test
        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied();

        [Theory]
        [InlineData(@"[assembly: InternalsVisibleTo(""OtherFirstAssembly"")]
[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")]
[assembly: InternalsVisibleTo(""OtherSecondAssembly"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"")]")]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes);

        [Theory]
        [InlineData(@"[assembly: InternalsVisibleTo(""SomeValue"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly21"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicproxyGenAssembly2,"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2,"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, PublicKey="")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, ,PublicKey=abcd"")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, Version="")]")]
        [InlineData(@"[assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2, ,Version=1.0.0"")]")]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes);

        [Theory]
        public abstract Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues);

        [Theory]
        public abstract Task ReturnsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues);

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument();
#pragma warning restore xUnit1013 // Public method should be marked as test

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }
    }
}