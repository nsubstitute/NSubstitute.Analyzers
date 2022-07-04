using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableSetupAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly INonSubstitutableMemberAnalysis _nonSubstitutableMemberAnalysis;

    protected abstract DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    private readonly DiagnosticDescriptor _internalSetupSpecificationDescriptor;

    protected AbstractNonSubstitutableSetupAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis)
        : base(diagnosticDescriptorsProvider)
    {
        _nonSubstitutableMemberAnalysis = nonSubstitutableMemberAnalysis;
        _internalSetupSpecificationDescriptor = diagnosticDescriptorsProvider.InternalSetupSpecification;
    }

    protected void Analyze(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxNode syntaxNode, ISymbol symbol = null)
    {
        var analysisResult = _nonSubstitutableMemberAnalysis.Analyze(syntaxNodeAnalysisContext, syntaxNode, symbol);

        if (analysisResult.CanBeSubstituted == false)
        {
            ReportDiagnostics(syntaxNodeAnalysisContext, in analysisResult);
        }
    }

    protected void Analyze(OperationAnalysisContext operationAnalysisContext, IInvocationOperation operation, ISymbol symbol = null)
    {
        var analysisResult = _nonSubstitutableMemberAnalysis.Analyze(operation, symbol);

        if (analysisResult.CanBeSubstituted == false)
        {
            ReportDiagnostics(operationAnalysisContext, in analysisResult);
        }
    }

    protected void Analyze(OperationAnalysisContext operationAnalysisContext, IOperation operation, ISymbol symbol = null)
    {
        var analysisResult = _nonSubstitutableMemberAnalysis.Analyze(operation);

        if (analysisResult.CanBeSubstituted == false)
        {
            ReportDiagnostics(operationAnalysisContext, in analysisResult);
        }
    }

    protected virtual Location GetSubstitutionNodeActualLocation(in NonSubstitutableMemberAnalysisResult analysisResult)
    {
        return analysisResult.Member.GetLocation();
    }

    private void ReportDiagnostics(
        SyntaxNodeAnalysisContext context,
        in NonSubstitutableMemberAnalysisResult analysisResult)
    {
        var location = GetSubstitutionNodeActualLocation(analysisResult);
        if (analysisResult.NonVirtualMemberSubstitution)
        {
            var diagnostic = Diagnostic.Create(
                NonVirtualSetupDescriptor,
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

    private void ReportDiagnostics(
        OperationAnalysisContext context,
        in NonSubstitutableMemberAnalysisResult analysisResult)
    {
        var location = GetSubstitutionNodeActualLocation(analysisResult);
        if (analysisResult.NonVirtualMemberSubstitution)
        {
            var diagnostic = Diagnostic.Create(
                NonVirtualSetupDescriptor,
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