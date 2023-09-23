using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberArgumentMatcherAnalyzer : AbstractDiagnosticAnalyzer
{
    internal static ImmutableHashSet<OperationKind> MaybeAllowedAncestors { get; } = ImmutableHashSet.Create(
        OperationKind.Invocation,
        OperationKind.BinaryOperator,
        OperationKind.DynamicInvocation,
        OperationKind.PropertyReference,
        OperationKind.EventAssignment,
        OperationKind.ObjectCreation,
        OperationKind.SimpleAssignment);

    private static readonly ImmutableHashSet<OperationKind> IgnoredAncestors =
        ImmutableHashSet.Create(OperationKind.VariableDeclarator, OperationKind.VariableDeclaration, OperationKind.Return);

    private static readonly ImmutableHashSet<OperationKind> DynamicOperations =
        ImmutableHashSet.Create(OperationKind.DynamicInvocation, OperationKind.DynamicIndexerAccess, OperationKind.DynamicMemberReference);

    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    private readonly INonSubstitutableMemberAnalysis _nonSubstitutableMemberAnalysis;

    protected AbstractNonSubstitutableMemberArgumentMatcherAnalyzer(
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider)
    {
        _nonSubstitutableMemberAnalysis = nonSubstitutableMemberAnalysis;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage);
    }

    public sealed override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext operationAnalysisContext)
    {
        var invocationOperation = (IInvocationOperation)operationAnalysisContext.Operation;

        if (invocationOperation.TargetMethod.IsArgMatcherLikeMethod() == false)
        {
            return;
        }

        AnalyzeArgLikeMethod(operationAnalysisContext, invocationOperation);
    }

    private void AnalyzeArgLikeMethod(
        OperationAnalysisContext context,
        IInvocationOperation invocationOperation)
    {
        var enclosingOperation = FindMaybeAllowedEnclosingExpression(invocationOperation);

        if (enclosingOperation != null && DynamicOperations.Contains(enclosingOperation.Kind))
        {
           return;
        }

        // if Arg is used with not allowed expression, find if it is used in ignored ones eg. var x = Arg.Any
        // as variable might be used later on
        if (enclosingOperation == null)
        {
            var ignoredEnclosingExpression = FindIgnoredEnclosingExpression(invocationOperation);

            if (ignoredEnclosingExpression == null)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                    invocationOperation.Syntax.GetLocation());

                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        if (enclosingOperation == null)
        {
            return;
        }

        if (enclosingOperation.IsEventAssignmentOperation())
        {
            return;
        }

        var memberReferenceOperation = GetMemberReferenceOperation(enclosingOperation);

        if (AnalyzeEnclosingExpression(
                context,
                invocationOperation,
                enclosingOperation,
                memberReferenceOperation))
        {
            return;
        }

        if (memberReferenceOperation != null)
        {
            AnalyzeAssignment(
                context,
                invocationOperation,
                memberReferenceOperation);
        }
    }

    private bool AnalyzeEnclosingExpression(
        OperationAnalysisContext context,
        IInvocationOperation argInvocation,
        IOperation enclosingOperation,
        IMemberReferenceOperation? memberReferenceOperation)
    {
        var enclosingExpressionSymbol = memberReferenceOperation?.Member ?? enclosingOperation.ExtractSymbol();

        if (enclosingExpressionSymbol == null)
        {
            TryReportDiagnostic(context, argInvocation, null);
            return true;
        }

        if (_nonSubstitutableMemberAnalysis.Analyze(memberReferenceOperation ?? enclosingOperation).CanBeSubstituted)
        {
            return false;
        }

        TryReportDiagnostic(context, argInvocation, enclosingExpressionSymbol);

        return true;
    }

    private void AnalyzeAssignment(
        OperationAnalysisContext context,
        IInvocationOperation invocationOperation,
        IMemberReferenceOperation memberReferenceOperation)
    {
        if (IsWithinWhenLikeMethod(memberReferenceOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsArgDoLikeMethod())
        {
            return;
        }

        if (IsPrecededByReceivedLikeMethod(memberReferenceOperation))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
            invocationOperation.Syntax.GetLocation());

        context.TryReportDiagnostic(diagnostic, memberReferenceOperation.Member);
    }

    private bool IsPrecededByReceivedLikeMethod(IOperation operation)
    {
        var substituteOperation = operation switch
        {
            IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.GetSubstituteOperation(),
            IInvocationOperation invocationOperation => invocationOperation.GetSubstituteOperation(),
            _=> null
        };

        return substituteOperation.ExtractSymbol()?.IsReceivedLikeMethod() ?? false;
    }

    private bool IsWithinWhenLikeMethod(IOperation operation)
    {
        var invocation = operation.Ancestors().FirstOrDefault(ancestor => ancestor.Kind == OperationKind.Invocation);

        return invocation is IInvocationOperation invocationOperation &&
               invocationOperation.TargetMethod.IsWhenLikeMethod();
    }

    private static IMemberReferenceOperation? GetMemberReferenceOperation(IOperation operation)
    {
        return operation switch
        {
            IAssignmentOperation { Target: IMemberReferenceOperation memberReferenceOperation } =>
                memberReferenceOperation,
            IBinaryOperation { LeftOperand: IMemberReferenceOperation binaryMemberReferenceOperation } =>
                binaryMemberReferenceOperation,
            IBinaryOperation { LeftOperand: IConversionOperation { Operand: IMemberReferenceOperation conversionMemberReference } } =>
                conversionMemberReference,
            IExpressionStatementOperation { Operation: ISimpleAssignmentOperation { Target: IMemberReferenceOperation memberReferenceOperation } } =>
                memberReferenceOperation,
            _ => null
        };
    }

    private IOperation? FindMaybeAllowedEnclosingExpression(IOperation operation) =>
        FindEnclosingExpression(operation, MaybeAllowedAncestors);

    private IOperation? FindIgnoredEnclosingExpression(IOperation operation) =>
        FindEnclosingExpression(operation, IgnoredAncestors);

    private static IOperation? FindEnclosingExpression(IOperation operation, ImmutableHashSet<OperationKind> ancestors)
    {
        return operation.Ancestors()
            .FirstOrDefault(ancestor => ancestors.Contains(ancestor.Kind));
    }

    private void TryReportDiagnostic(
        OperationAnalysisContext context,
        IInvocationOperation argInvocation,
        ISymbol? enclosingExpressionSymbol)
    {
        context.TryReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptorsProvider.NonSubstitutableMemberArgumentMatcherUsage,
                argInvocation.Syntax.GetLocation()),
            enclosingExpressionSymbol);
    }
}