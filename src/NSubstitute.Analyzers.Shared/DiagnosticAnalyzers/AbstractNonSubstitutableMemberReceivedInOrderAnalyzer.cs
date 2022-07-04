using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractNonSubstitutableMemberReceivedInOrderAnalyzer<TSyntaxKind,
    TMemberAccessExpressionSyntax, TBlockStatementSyntax> : AbstractNonSubstitutableSetupAnalyzer
    where TSyntaxKind : struct
    where TMemberAccessExpressionSyntax : SyntaxNode
    where TBlockStatementSyntax : SyntaxNode
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    protected abstract TSyntaxKind InvocationExpressionKind { get; }

    protected abstract ImmutableArray<int> IgnoredAncestorPaths { get; }

    private readonly Action<SyntaxNodeAnalysisContext> _analyzeInvocationAction;
    private readonly ISubstitutionNodeFinder _substitutionNodeFinder;

    protected AbstractNonSubstitutableMemberReceivedInOrderAnalyzer(
        ISubstitutionNodeFinder substitutionNodeFinder,
        INonSubstitutableMemberAnalysis nonSubstitutableMemberAnalysis,
        IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        : base(diagnosticDescriptorsProvider, nonSubstitutableMemberAnalysis)
    {
        _substitutionNodeFinder = substitutionNodeFinder;
        _analyzeInvocationAction = AnalyzeInvocation;
        NonVirtualSetupDescriptor = diagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification;
        SupportedDiagnostics = ImmutableArray.Create(
            DiagnosticDescriptorsProvider.InternalSetupSpecification,
            DiagnosticDescriptorsProvider.NonVirtualReceivedInOrderSetupSpecification);
    }

    protected override DiagnosticDescriptor NonVirtualSetupDescriptor { get; }

    protected sealed override void InitializeAnalyzer(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(_analyzeInvocationAction, InvocationExpressionKind);
    }

    protected override Location GetSubstitutionNodeActualLocation(
        in NonSubstitutableMemberAnalysisResult analysisResult)
    {
        return analysisResult.Member.GetSubstitutionNodeActualLocation<TMemberAccessExpressionSyntax>(analysisResult
            .Symbol);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
    {
        if (!(syntaxNodeContext.SemanticModel.GetOperation(syntaxNodeContext.Node) is IInvocationOperation
                invocationOperation))
        {
            return;
        }

        if (invocationOperation.TargetMethod.IsReceivedInOrderMethod() == false)
        {
            return;
        }

        foreach (var syntaxNode in _substitutionNodeFinder
                     .FindForReceivedInOrderExpression(syntaxNodeContext, invocationOperation)
                     .Where(node => ShouldAnalyzeNode(syntaxNodeContext.SemanticModel, node)))
        {
            var symbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode);

            if (symbolInfo.Symbol == null)
            {
                return;
            }

            Analyze(syntaxNodeContext, syntaxNode, symbolInfo.Symbol);
        }
    }

    private bool ShouldAnalyzeNode(SemanticModel semanticModel, SyntaxNode syntaxNode)
    {
        var maybeIgnoredExpression = FindIgnoredEnclosingExpression(syntaxNode);
        if (maybeIgnoredExpression == null)
        {
            return true;
        }

        if (syntaxNode.Parent is TMemberAccessExpressionSyntax ||
            semanticModel.GetOperation(syntaxNode.Parent) is IMemberReferenceOperation)
        {
            return false;
        }

        var operation = semanticModel.GetOperation(maybeIgnoredExpression);

        if (syntaxNode.Parent is TMemberAccessExpressionSyntax ||
            semanticModel.GetOperation(syntaxNode.Parent) is IMemberReferenceOperation)
        {
            return false;
        }

        if (operation is IArgumentOperation &&
            operation.Parent is IInvocationOperation invocationOperation &&
            invocationOperation.TargetMethod.IsReceivedInOrderMethod())
        {
            return true;
        }

        if (operation.IsEventAssignmentOperation())
        {
            return false;
        }

        var symbol = GetVariableDeclaratorSymbol(operation);

        if (symbol == null)
        {
            return false;
        }

        var blockStatementSyntax =
            maybeIgnoredExpression.Ancestors().OfType<TBlockStatementSyntax>().FirstOrDefault();

        if (blockStatementSyntax == null)
        {
            return false;
        }

        var dataFlowAnalysis = semanticModel.AnalyzeDataFlow(blockStatementSyntax);
        return !dataFlowAnalysis.ReadInside.Contains(symbol);
    }

    private static ILocalSymbol GetVariableDeclaratorSymbol(IOperation operation)
    {
        return operation switch
        {
            IVariableDeclaratorOperation declarator => declarator.Symbol,
            IVariableDeclarationOperation declarationOperation => declarationOperation.Declarators.FirstOrDefault()?.Symbol,
            _ => null
        };
    }

    private SyntaxNode FindIgnoredEnclosingExpression(SyntaxNode syntaxNode)
    {
        return syntaxNode.Ancestors().FirstOrDefault(ancestor => IgnoredAncestorPaths.Contains(ancestor.RawKind));
    }
}