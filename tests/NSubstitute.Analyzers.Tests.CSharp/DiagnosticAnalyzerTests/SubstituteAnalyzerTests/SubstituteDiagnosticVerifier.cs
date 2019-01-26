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

        [Fact]
        public abstract Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2();

        [Fact]
        public abstract Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly();

        public abstract Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues);

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