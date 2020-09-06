﻿using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface ISubstituteAnalyzerVerifier
    {
        Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor();

        Task ReturnsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied();

        Task ReturnsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied();

        Task ReturnsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied();

        Task ReturnsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied();

        Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount();

        Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount();

        // even though parameters are optional, NSubstitute requires to pass all of them
        Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters();

        Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied();

        Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes);

        Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes);

        Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues);

        Task ReturnsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues);

        Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument();
    }
}