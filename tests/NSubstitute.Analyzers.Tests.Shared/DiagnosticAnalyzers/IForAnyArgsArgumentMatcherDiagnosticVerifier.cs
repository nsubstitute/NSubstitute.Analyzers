using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface IForAnyArgsArgumentMatcherDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg);
}