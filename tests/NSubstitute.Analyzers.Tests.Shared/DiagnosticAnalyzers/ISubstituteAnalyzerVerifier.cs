using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface ISubstituteAnalyzerVerifier
{
    Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor();

    Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied();

    Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied();

    Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied();

    Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied();

    Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount();

    Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount();

    // even though parameters are optional, NSubstitute requires to pass all of them
    Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters();

    Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied();

    Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes);

    Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes);

    Task ReportsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues);

    Task ReportsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues);

    Task ReportsNoDiagnostic_WhenUsedWithGenericArgument();

    Task ReportsNoDiagnostic_WhenParamsParametersNotProvided();

    Task ReportsNoDiagnostic_WhenParamsParametersProvided();

    Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount_AndParamsParameterDefined();
}