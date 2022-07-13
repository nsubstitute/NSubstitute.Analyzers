using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractReEntrantSetupAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TArgumentSyntax> : AbstractDiagnosticAnalyzer
    where TSyntaxKind : struct
    where TInvocationExpressionSyntax : SyntaxNode
    where TArgumentSyntax : SyntaxNode
{
    private readonly IReEntrantCallFinder _reEntrantCallFinder;
    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected AbstractReEntrantSetupAnalyzer(
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider,
        IReEntrantCallFinder reEntrantCallFinder)
        : base(diagnosticDescriptorsProvider)
    {
        _reEntrantCallFinder = reEntrantCallFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        SupportedDiagnostics = ImmutableArray.Create(DiagnosticDescriptorsProvider.ReEntrantSubstituteCall);
    }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    protected abstract IEnumerable<SyntaxNode> GetExpressionsFromArrayInitializer(TArgumentSyntax syntaxNode);

    protected abstract IEnumerable<TArgumentSyntax> GetArguments(TInvocationExpressionSyntax invocationExpressionSyntax);

    protected abstract SyntaxNode GetArgumentExpression(TArgumentSyntax argumentSyntax);

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        var invocationExpression = (TInvocationExpressionSyntax)syntaxNodeContext.Node;
        var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

        if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
        {
            return;
        }

        var methodSymbol = (IMethodSymbol)methodSymbolInfo.Symbol;

        if (methodSymbol.IsInitialReEntryLikeMethod() == false)
        {
            return;
        }

        var allArguments = GetArguments(invocationExpression);
        var argumentsForAnalysis = methodSymbol.MethodKind == MethodKind.ReducedExtension ? allArguments : allArguments.Skip(1);

        foreach (var argument in argumentsForAnalysis)
        {
            var operation = syntaxNodeContext.SemanticModel.GetOperation(argument) as IArgumentOperation;

            if (IsPassedByParamsArrayOfCallInfoFunc(syntaxNodeContext.SemanticModel.Compilation, operation))
            {
                continue;
            }

            if (IsPassedByParamsArray(operation))
            {
                AnalyzeParamsArgument(syntaxNodeContext, argument, invocationExpression, methodSymbol);
            }
            else
            {
                AnalyzeExpression(syntaxNodeContext, GetArgumentExpression(argument), invocationExpression, methodSymbol);
            }
        }
    }

    private void AnalyzeParamsArgument(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        TArgumentSyntax argument,
        TInvocationExpressionSyntax invocationExpression,
        IMethodSymbol methodSymbol)
    {
        var arrayInitializersExpressions = GetExpressionsFromArrayInitializer(argument);

        // if array elements can't be extracted, analyze argument itself
        foreach (var argumentExpression in arrayInitializersExpressions ?? new[] { GetArgumentExpression(argument) })
        {
            AnalyzeExpression(syntaxNodeContext, argumentExpression, invocationExpression, methodSymbol);
        }
    }

    private void AnalyzeExpression(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        SyntaxNode argumentExpression,
        TInvocationExpressionSyntax invocationExpression,
        IMethodSymbol methodSymbol)
    {
        var reentrantSymbol = _reEntrantCallFinder.GetReEntrantCalls(
            syntaxNodeContext.Compilation,
            syntaxNodeContext.SemanticModel,
            invocationExpression,
            argumentExpression).FirstOrDefault();

        if (reentrantSymbol != null)
        {
            var diagnostic = Diagnostic.Create(
                DiagnosticDescriptorsProvider.ReEntrantSubstituteCall,
                argumentExpression.GetLocation(),
                methodSymbol.Name,
                reentrantSymbol.Name,
                argumentExpression.ToString());

            syntaxNodeContext.ReportDiagnostic(diagnostic);
        }
    }

    private bool IsPassedByParamsArray(IArgumentOperation argumentOperation)
    {
        return argumentOperation != null &&
               argumentOperation.Parameter.IsParams &&
               argumentOperation.Parameter.Type is IArrayTypeSymbol;
    }

    private bool IsPassedByParamsArrayOfCallInfoFunc(Compilation compilation, IArgumentOperation argumentOperation)
    {
        return IsPassedByParamsArray(argumentOperation) &&
               argumentOperation.Parameter.Type is IArrayTypeSymbol arrayTypeSymbol &&
               arrayTypeSymbol.ElementType is INamedTypeSymbol namedTypeSymbol &&
               namedTypeSymbol.IsCallInfoDelegate(compilation);
    }
}