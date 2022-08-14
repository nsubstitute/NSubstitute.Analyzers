using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractWithAnyArgsArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax> : AbstractDiagnosticAnalyzer
    where TInvocationExpressionSyntax : SyntaxNode
    where TSyntaxKind : struct, Enum
{
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    protected abstract ImmutableHashSet<int> MaybeAllowedArgMatcherAncestors { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

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
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (methodSymbol.IsWithAnyArgsIncompatibleArgMatcherLikeMethod())
        {
            AnalyzeArgLikeMethodForReceivedWithAnyArgs(syntaxNodeContext, invocationExpression, methodSymbol);
            return;
        }

        if (methodSymbol.IsReturnForAnyArgsLikeMethod() || methodSymbol.IsThrowForAnyArgsLikeMethod())
        {
            AnalyzeReturnsLikeMethod(syntaxNodeContext, invocationExpression);
            return;
        }

        if (methodSymbol.IsWhenForAnyArgsLikeMethod())
        {
           AnalyzeWhenLikeMethod(syntaxNodeContext, invocationExpression);
        }
    }

    private void AnalyzeWhenLikeMethod(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        TInvocationExpressionSyntax whenMethod)
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

        if (syntaxNodeContext.SemanticModel.GetOperation(whenMethod) is not IInvocationOperation invocationOperation)
        {
           return;
        }

        foreach (var syntaxNode in _substitutionNodeFinder.FindForWhenExpression(syntaxNodeContext, invocationOperation))
        {
            var substitutedOperation = syntaxNodeContext.SemanticModel.GetOperation(syntaxNode);

            IReadOnlyList<IOperation> arguments = substitutedOperation switch
            {
                IInvocationOperation substituteInvocationOperation => substituteInvocationOperation.Arguments,
                IPropertyReferenceOperation propertyReferenceOperation => Arguments(propertyReferenceOperation),
                _ => ImmutableArray<IArgumentOperation>.Empty
            };

            foreach (var operation in arguments)
            {
                AnalyzeArgument(syntaxNodeContext, operation);
            }
        }
    }

    private void AnalyzeReturnsLikeMethod(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        TInvocationExpressionSyntax returnsInvocationExpression)
    {
        if (syntaxNodeContext.SemanticModel.GetOperation(returnsInvocationExpression) is not IInvocationOperation
            invocationOperation)
        {
            return;
        }

        var substitutedOperation =
            _substitutionNodeFinder.FindOperationForStandardExpression(invocationOperation);

        var arguments = GetArguments(substitutedOperation);

        foreach (var argumentOperation in arguments)
        {
            AnalyzeArgument(syntaxNodeContext, argumentOperation);
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

    private void AnalyzeArgument(SyntaxNodeAnalysisContext syntaxNodeContext, IOperation operation)
    {
        if (operation is IConversionOperation conversionOperation)
        {
            AnalyzeArgument(syntaxNodeContext, conversionOperation.Operand);
            return;
        }

        if (operation is IArgumentOperation argumentOperation)
        {
           AnalyzeArgument(syntaxNodeContext, argumentOperation.Value);
           return;
        }

        if (operation is IInvocationOperation argInvocationOperation &&
            argInvocationOperation.TargetMethod.IsWithAnyArgsIncompatibleArgMatcherLikeMethod())
        {
            syntaxNodeContext.TryReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                    argInvocationOperation.Syntax.GetLocation()),
                argInvocationOperation.TargetMethod);
        }
    }

    private void AnalyzeArgLikeMethodForReceivedWithAnyArgs(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        TInvocationExpressionSyntax argInvocationExpression,
        IMethodSymbol invocationExpressionSymbol)
    {
        var enclosingExpression = FindMaybeAllowedEnclosingExpression(argInvocationExpression);

        if (enclosingExpression == null)
        {
            return;
        }

        var operation = syntaxNodeContext.SemanticModel.GetOperation(enclosingExpression);

        if (operation is IInvocationOperation enclosingInvocationOperation &&
            enclosingInvocationOperation.Instance is IInvocationOperation enclosingInvocationOperationInstance &&
            enclosingInvocationOperationInstance.TargetMethod.IsReceivedWithAnyArgsLikeMethod())
        {
            syntaxNodeContext.TryReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                    argInvocationExpression.GetLocation()),
                invocationExpressionSymbol);
            return;
        }

        var memberReferenceOperation = GetMemberReferenceOperation(operation);

        if (memberReferenceOperation is not { Instance: IInvocationOperation invocationOperation } ||
            !invocationOperation.TargetMethod.IsReceivedWithAnyArgsLikeMethod())
        {
            return;
        }

        syntaxNodeContext.TryReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptorsProvider.WithAnyArgsArgumentMatcherUsage,
                argInvocationExpression.GetLocation()),
            invocationExpressionSymbol);
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

    private SyntaxNode FindMaybeAllowedEnclosingExpression(TInvocationExpressionSyntax invocationExpression) =>
        FindEnclosingExpression(invocationExpression, MaybeAllowedArgMatcherAncestors);

    private static SyntaxNode FindEnclosingExpression(TInvocationExpressionSyntax invocationExpression, ImmutableHashSet<int> ancestors)
    {
        return invocationExpression.Ancestors()
            .FirstOrDefault(ancestor => ancestors.Contains(ancestor.RawKind));
    }
}