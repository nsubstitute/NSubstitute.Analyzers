using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstitutionNodeFinder : ISubstitutionNodeFinder
{
    public IEnumerable<SyntaxNode> Find(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IInvocationOperation invocationOperation,
        IMethodSymbol invocationExpressionSymbol = null)
    {
        if (invocationOperation == null)
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        var invocationExpression = invocationOperation.Syntax;
        invocationExpressionSymbol ??=
            syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

        if (invocationExpressionSymbol == null ||
            invocationExpressionSymbol.ContainingAssembly.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == false)
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteDoMethod, StringComparison.Ordinal))
        {
            var operation = invocationOperation.GetSubstituteOperation();
            return FindForWhenExpression(syntaxNodeContext, operation as IInvocationOperation);
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenMethod, StringComparison.Ordinal) ||
            invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenForAnyArgsMethod, StringComparison.Ordinal))
        {
            return FindForWhenExpression(syntaxNodeContext, invocationOperation, invocationExpressionSymbol);
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteAndDoesMethod, StringComparison.Ordinal))
        {
            var substitution =
                FindForAndDoesExpression(syntaxNodeContext, invocationOperation, invocationExpressionSymbol);
            return substitution != null ? new[] { substitution } : Enumerable.Empty<SyntaxNode>();
        }

        var standardSubstitution = FindForStandardExpression(invocationOperation);

        return standardSubstitution != null ? new[] { standardSubstitution } : Enumerable.Empty<SyntaxNode>();
    }

    public IEnumerable<SyntaxNode> FindForWhenExpression(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IInvocationOperation invocationOperation,
        IMethodSymbol whenInvocationSymbol = null)
    {
        if (invocationOperation == null)
        {
            yield break;
        }

        var invocationExpression = invocationOperation.Syntax;
        whenInvocationSymbol ??=
            syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

        if (whenInvocationSymbol == null)
        {
            yield break;
        }

        var typeSymbol = whenInvocationSymbol.TypeArguments.FirstOrDefault() ?? whenInvocationSymbol.ReceiverType;
        var argumentOperation = invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument().First();

        foreach (var syntaxNode in FindInvocations(syntaxNodeContext, argumentOperation.Value.Syntax))
        {
            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
            if (symbol != null && typeSymbol != null && ContainsSymbol(typeSymbol, symbol))
            {
                yield return GetSubstitutionActualNode(syntaxNodeContext, syntaxNode);
            }
        }
    }

    public IEnumerable<IOperation> FindForWhenExpression(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation)
    {
        var whenVisitor = new WhenVisitor(operationAnalysisContext, invocationOperation);
        whenVisitor.Visit();

        var typeSymbol = invocationOperation.TargetMethod.TypeArguments.FirstOrDefault() ??
                         invocationOperation.TargetMethod.ReceiverType;

        foreach (var operation in whenVisitor.Operations)
        {
            var symbol = ExtractSymbol(operation);

            if (symbol != null && ContainsSymbol(typeSymbol, symbol))
            {
                yield return operation;
            }
        }
    }

    public SyntaxNode FindForAndDoesExpression(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IInvocationOperation invocationOperation,
        IMethodSymbol invocationExpressionSymbol)
    {
        if (invocationOperation.GetSubstituteOperation() is not IInvocationOperation parentInvocationExpression)
        {
            return null;
        }

        return FindForStandardExpression(parentInvocationExpression);
    }

    public SyntaxNode FindForStandardExpression(IInvocationOperation invocationOperation)
    {
        var substituteOperation = invocationOperation.GetSubstituteOperation();
        return substituteOperation.Syntax;
    }

    public IOperation FindOperationForStandardExpression(IInvocationOperation invocationOperation)
    {
        return invocationOperation.GetSubstituteOperation();
    }

    public IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(
        SyntaxNodeAnalysisContext syntaxNodeContext,
        IInvocationOperation invocationOperation,
        IMethodSymbol receivedInOrderInvocationSymbol = null)
    {
        var argumentOperation = invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument().First();

        return FindInvocations(syntaxNodeContext, argumentOperation.Value.Syntax)
            .Select(syntax => GetSubstitutionActualNode(syntaxNodeContext, syntax));
    }

    protected abstract IEnumerable<SyntaxNode> FindInvocations(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode argumentSyntax);

    protected abstract SyntaxNode GetSubstitutionActualNode(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntaxNode);

    private bool ContainsSymbol(ITypeSymbol containerSymbol, ISymbol symbol)
    {
        return GetBaseTypesAndThis(containerSymbol).Any(typeSymbol => typeSymbol.Equals(symbol.ContainingType));
    }

    private static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    private static ISymbol ExtractSymbol(IOperation operation)
    {
        var symbol = operation switch
        {
            IInvocationOperation invocationOperation => invocationOperation.TargetMethod,
            IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Property,
            IConversionOperation conversionOperation => ExtractSymbol(conversionOperation.Operand),
            _ => null
        };
        return symbol;
    }

    private class WhenVisitor : OperationWalker
    {
        private readonly OperationAnalysisContext _operationAnalysisContext;
        private readonly IInvocationOperation _whenInvocationOperation;
        private readonly List<IOperation> _operations = new List<IOperation>();
        private readonly IDictionary<SyntaxTree, SemanticModel>
            _semanticModelCache = new Dictionary<SyntaxTree, SemanticModel>(1);

        public WhenVisitor(
            OperationAnalysisContext operationAnalysisContext,
            IInvocationOperation whenInvocationOperation)
        {
            _operationAnalysisContext = operationAnalysisContext;
            _whenInvocationOperation = whenInvocationOperation;
        }

        public IReadOnlyList<IOperation> Operations => _operations.AsReadOnly();

        public void Visit() => Visit(_whenInvocationOperation);

        public override void VisitInvocation(IInvocationOperation operation)
        {
            if (operation != _whenInvocationOperation)
            {
                _operations.Add(operation);
            }

            base.VisitInvocation(operation);
        }

        public override void VisitMethodReference(IMethodReferenceOperation operation)
        {
            foreach (var methodDeclaringSyntaxReference in operation.Method.DeclaringSyntaxReferences)
            {
                // TODO async?
                var syntaxNode = methodDeclaringSyntaxReference.GetSyntax();
                var semanticModel = GetSemanticModel(syntaxNode.Parent);
                var referencedOperation = semanticModel.GetOperation(syntaxNode) ??
                                          semanticModel.GetOperation(syntaxNode.Parent);
                Visit(referencedOperation);
            }

            base.VisitMethodReference(operation);
        }

        public override void VisitPropertyReference(IPropertyReferenceOperation operation)
        {
            _operations.Add(operation);
            base.VisitPropertyReference(operation);
        }

        private SemanticModel GetSemanticModel(SyntaxNode syntaxNode)
        {
            var syntaxTree = syntaxNode.SyntaxTree;
            if (_semanticModelCache.TryGetValue(syntaxTree, out var semanticModel))
            {
                return semanticModel;
            }

            semanticModel = _operationAnalysisContext.Compilation.GetSemanticModel(syntaxTree);
            _semanticModelCache[syntaxTree] = semanticModel;

            return semanticModel;
        }
    }
}