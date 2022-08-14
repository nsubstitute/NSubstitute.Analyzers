using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface IWithAnyArgsArgumentMatcherDiagnosticVerifier : IForAnyArgsArgumentMatcherDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg);

    Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg);
}