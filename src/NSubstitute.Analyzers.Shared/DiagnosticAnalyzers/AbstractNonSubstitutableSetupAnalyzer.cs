using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableSetupAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly INonSubstitutableMemberAnalysis _nonSubstitutableMemberAnalysis;

    protected abstract DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    private readonly DiagnosticDescriptor _extensionMethodSetupDescriptor;

    private readonly DiagnosticDescriptor _internalSetupSpecificationDescriptor;

    protected AbstractNonSubstitutableSetupAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider)
    {
        _nonSubstitutableMemberAnalysis = nonSubstitutableMemberAnalysis;
        _extensionMethodSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualSetupSpecification;
        _internalSetupSpecificationDescriptor = diagnosticDescriptorsProvider.InternalSetupSpecification;
    }

    protected void Analyze(OperationAnalysisContext operationAnalysisContext, IOperation operation)
    {
        var analysisResult = _nonSubstitutableMemberAnalysis.Analyze(operation);

        if (analysisResult.CanBeSubstituted == false)
        {
            ReportDiagnostics(operationAnalysisContext, in analysisResult);
        }
    }

    private void ReportDiagnostics(
        OperationAnalysisContext context,
        in NonSubstitutableMemberAnalysisResult analysisResult)
    {
        var location = analysisResult.Member.GetLocation();

        if (analysisResult.NonVirtualMemberSubstitution)
        {
            var descriptor = analysisResult.Symbol is IMethodSymbol { IsExtensionMethod: true }
                ? this._extensionMethodSetupDescriptor
                : NonVirtualSetupDescriptor;

            var diagnostic = Diagnostic.Create(
                descriptor,
                location,
                analysisResult.MemberName);
            context.TryReportDiagnostic(diagnostic, analysisResult.Symbol);
        }

        if (analysisResult.InternalMemberSubstitution)
        {
            var diagnostic = Diagnostic.Create(
                _internalSetupSpecificationDescriptor,
                location,
                analysisResult.MemberName);

            context.ReportDiagnostic(diagnostic);
        }
    }
}