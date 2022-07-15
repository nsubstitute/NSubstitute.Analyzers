using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class SubstitutionNodeFinder : ISubstitutionNodeFinder
{
    public static SubstitutionNodeFinder Instance { get; } = new ();

    public IEnumerable<IOperation> Find(
        Compilation compilation,
        IInvocationOperation invocationOperation)
    {
        if (invocationOperation == null)
        {
            return Enumerable.Empty<IOperation>();
        }

        var invocationExpressionSymbol = invocationOperation.TargetMethod;

        if (invocationExpressionSymbol == null ||
            invocationExpressionSymbol.ContainingAssembly.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == false)
        {
            return Enumerable.Empty<IOperation>();
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteDoMethod, StringComparison.Ordinal))
        {
            var operation = invocationOperation.GetSubstituteOperation();
            return FindForWhenExpression(compilation, operation as IInvocationOperation);
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenMethod, StringComparison.Ordinal) ||
            invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenForAnyArgsMethod, StringComparison.Ordinal))
        {
            return FindForWhenExpression(compilation, invocationOperation);
        }

        if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteAndDoesMethod, StringComparison.Ordinal))
        {
            var substitution = FindForAndDoesExpression(invocationOperation);
            return substitution != null ? new[] { substitution } : Enumerable.Empty<IOperation>();
        }

        var standardSubstitution = FindForStandardExpression(invocationOperation);

        return standardSubstitution != null ? new[] { standardSubstitution } : Enumerable.Empty<IOperation>();
    }

    public IEnumerable<IOperation> FindForWhenExpression(Compilation compilation, IInvocationOperation invocationOperation)
    {
        if (invocationOperation == null)
        {
            yield break;
        }

        var whenVisitor = new WhenVisitor(compilation, invocationOperation);
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

    public IEnumerable<IOperation> FindForReceivedInOrderExpression(
        Compilation compilation,
        IInvocationOperation invocationOperation,
        bool includeAll = false)
    {
        var visitor = new WhenVisitor(compilation, invocationOperation, includeAll);
        visitor.Visit();

        return visitor.Operations;
    }

    public IOperation FindForStandardExpression(IInvocationOperation invocationOperation)
    {
        return invocationOperation.GetSubstituteOperation();
    }

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

    private IOperation FindForAndDoesExpression(IInvocationOperation invocationOperation)
    {
        if (invocationOperation.GetSubstituteOperation() is not IInvocationOperation parentInvocationOperation)
        {
            return null;
        }

        return FindForStandardExpression(parentInvocationOperation);
    }

    private class WhenVisitor : OperationWalker
    {
        private readonly Compilation _compilation;
        private readonly IInvocationOperation _whenInvocationOperation;
        private readonly bool _includeAll;
        private readonly HashSet<IOperation> _operations = new ();

        private readonly Dictionary<SyntaxTree, SemanticModel> _semanticModelCache = new (1);

        public WhenVisitor(
            Compilation compilation,
            IInvocationOperation whenInvocationOperation,
            bool includeAll = false)
        {
            _compilation = compilation;
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

            semanticModel = _compilation.GetSemanticModel(syntaxTree);
            _semanticModelCache[syntaxTree] = semanticModel;

            return semanticModel;
        }
    }
}