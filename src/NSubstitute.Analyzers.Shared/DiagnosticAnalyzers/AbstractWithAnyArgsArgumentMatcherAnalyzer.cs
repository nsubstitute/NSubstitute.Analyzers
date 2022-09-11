using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractWithAnyArgsArgumentMatcherAnalyzer : AbstractDiagnosticAnalyzer
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<OperationAnalysisContext> _analyzeInvocationAction;

    private static readonly ImmutableHashSet<OperationKind> MaybeAllowedArgMatcherAncestors =
        ImmutableHashSet.Create(OperationKind.Invocation, OperationKind.ObjectCreation, OperationKind.SimpleAssignment);

    protected AbstractWithAnyArgsArgumentMatcherAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        ISubstitutionNodeFinder substitutionNodeFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterOperationAction(_analyzeInvocationAction, OperationKind.Invocation);
    }

    private void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocationOperation = (IInvocationOperation)context.Operation;

        var methodSymbol = invocationOperation.TargetMethod;
        if (methodSymbol.IsWithAnyArgsIncompatibleArgMatcherLikeMethod())
        {
            AnalyzeArgLikeMethodForReceivedWithAnyArgs(context, invocationOperation);
            return;
        }

        if (methodSymbol.IsReturnForAnyArgsLikeMethod() || methodSymbol.IsThrowForAnyArgsLikeMethod())
        {
            AnalyzeReturnsLikeMethod(context, invocationOperation);
            return;
        }

        if (methodSymbol.IsWhenForAnyArgsLikeMethod())
        {
           AnalyzeWhenLikeMethod(context, invocationOperation);
        }
    }

    private void AnalyzeWhenLikeMethod(
        OperationAnalysisContext context,
        IInvocationOperation invocationOperation)
    {
        ImmutableArray<IOperation> Arguments(IPropertyReferenceOperation propertyReferenceOperation)
        {
            var builder = ImmutableArray.CreateBuilder<IOperation>(propertyReferenceOperation.Arguments.Length);
            builder.AddRange(propertyReferenceOperation.Arguments);

            if (propertyReferenceOperation.Parent is IAssignmentOperation assignmentOperation)
            {
                builder.Add(assignmentOperation.Value);
            }

            return builder.ToImmutable();
        }

        foreach (var substitutedOperation in _substitutionNodeFinder.FindForWhenExpression(context.Compilation, invocationOperation))
        {
            IReadOnlyList<IOperation> arguments = substitutedOperation switch
            {
                IInvocationOperation substituteInvocationOperation => substituteInvocationOperation.Arguments,
                IPropertyReferenceOperation propertyReferenceOperation => Arguments(propertyReferenceOperation),
                _ => ImmutableArray<IArgumentOperation>.Empty
            };

            foreach (var operation in arguments)
            {
                AnalyzeArgument(context, operation);
            }
        }
    }

    private void AnalyzeReturnsLikeMethod(
        OperationAnalysisContext context,
        IInvocationOperation invocationOperation)
    {
        var substitutedOperation =
            _substitutionNodeFinder.FindForStandardExpression(invocationOperation);

        var arguments = GetArguments(substitutedOperation);

        foreach (var argumentOperation in arguments)
        {
            AnalyzeArgument(context, argumentOperation);
        }
    }

    private static IReadOnlyList<IOperation> GetArguments(IOperation substitutedOperation)
    {
        IReadOnlyList<IOperation> arguments = substitutedOperation switch
        {
            IInvocationOperation substituteInvocationOperation => substituteInvocationOperation.Arguments,
            IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Arguments,
            IConversionOperation conversionOperation => GetArguments(conversionOperation.Operand),
            _ => ImmutableArray<IOperation>.Empty
        };
        return arguments;
    }

    private void AnalyzeArgument(OperationAnalysisContext context, IOperation operation)
    {
        if (operation is IConversionOperation conversionOperation)
        {
            AnalyzeArgument(context, conversionOperation.Operand);
            return;
        }

        if (operation is IArgumentOperation argumentOperation)
        {
           AnalyzeArgument(context, argumentOperation.Value);
           return;
        }

        if (operation is IInvocationOperation argInvocationOperation &&
            argInvocationOperation.TargetMethod.IsWithAnyArgsIncompatibleArgMatcherLikeMethod())
        {
            context.TryReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                    argInvocationOperation.Syntax.GetLocation()),
                argInvocationOperation.TargetMethod);
        }
    }

    private void AnalyzeArgLikeMethodForReceivedWithAnyArgs(
        OperationAnalysisContext context,
        IInvocationOperation argInvocationOperation)
    {
        var enclosingOperation = FindMaybeAllowedEnclosingOperation(argInvocationOperation);

        if (enclosingOperation == null)
        {
            return;
        }

        if (enclosingOperation is IInvocationOperation enclosingInvocationOperation &&
            enclosingInvocationOperation.Instance is IInvocationOperation enclosingInvocationOperationInstance &&
            enclosingInvocationOperationInstance.TargetMethod.IsReceivedWithAnyArgsLikeMethod())
        {
            context.TryReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                    argInvocationOperation.Syntax.GetLocation()),
                argInvocationOperation.TargetMethod);
            return;
        }

        var memberReferenceOperation = GetMemberReferenceOperation(enclosingOperation);

        if (memberReferenceOperation is not { Instance: IInvocationOperation invocationOperation } ||
            !invocationOperation.TargetMethod.IsReceivedWithAnyArgsLikeMethod())
        {
            return;
        }

        context.TryReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                argInvocationOperation.Syntax.GetLocation()),
            argInvocationOperation.TargetMethod);
    }

    private static IMemberReferenceOperation GetMemberReferenceOperation(IOperation operation)
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

    private IOperation FindMaybeAllowedEnclosingOperation(IInvocationOperation invocationOperation) =>
        FindEnclosingOperation(invocationOperation, MaybeAllowedArgMatcherAncestors);

    private static IOperation FindEnclosingOperation(IInvocationOperation invocationOperation, ImmutableHashSet<OperationKind> ancestors)
    {
        return invocationOperation.Ancestors()
            .FirstOrDefault(ancestor => ancestors.Contains(ancestor.Kind));
    }
}