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

    public IEnumerable<IOperation> FindForReceivedInOrderExpression(OperationAnalysisContext operationAnalysisContext, IInvocationOperation invocationOperation, bool includeAll = false)
    {
        var visitor = new WhenVisitor(operationAnalysisContext, invocationOperation, includeAll);
        visitor.Visit();

        return visitor.Operations;
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
        private readonly bool _includeAll;
        private readonly HashSet<IOperation> _operations = new ();

        private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModelCache = new (1);

        public WhenVisitor(
            OperationAnalysisContext operationAnalysisContext,
            IInvocationOperation whenInvocationOperation,
            bool includeAll = false)
        {
            _operationAnalysisContext = operationAnalysisContext;
            _whenInvocationOperation = whenInvocationOperation;
            _includeAll = includeAll;
        }

        public IEnumerable<IOperation> Operations => _operations;

        public void Visit() => Visit(_whenInvocationOperation);

        public override void VisitInvocation(IInvocationOperation operation)
        {
            TryAdd(operation);

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
            TryAdd(operation);
            base.VisitPropertyReference(operation);
        }

        private void TryAdd(IOperation operation)
        {
            if (operation == _whenInvocationOperation)
            {
                return;
            }

            if (operation.Parent == null)
            {
                _operations.Add(operation);
                return;
            }

            // For cases like Foo.Nested.Bar(); Foo.Nested().Bar
            // we are only interested in last operation
            // TODO make it smarter, will fail on multiple nested operations
            if (_includeAll)
            {
                _operations.Add(operation);
            }
            else if (_operations.Contains(operation.Parent) == false)
            {
                _operations.Add(operation);
            }
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