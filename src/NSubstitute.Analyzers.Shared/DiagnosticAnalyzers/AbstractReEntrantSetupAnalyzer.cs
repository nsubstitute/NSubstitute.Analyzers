using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractReEntrantSetupAnalyzer<TSyntaxKind> : AbstractDiagnosticAnalyzer
        where TSyntaxKind : struct
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

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
        {
            var invocationExpression = syntaxNodeContext.Node;
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

            var invocationOperation = (IInvocationOperation)syntaxNodeContext.SemanticModel.GetOperation(invocationExpression);

            var arguments = invocationOperation.GetOrderedArgumentOperations();

            foreach (var argumentOperation in arguments)
            {
                if (IsPassedByParamsArrayOfCallInfoFunc(syntaxNodeContext.SemanticModel, argumentOperation))
                {
                    continue;
                }

                if (IsPassedByParamsArray(argumentOperation))
                {
                    AnalyzeParamsArgument(syntaxNodeContext, argumentOperation, invocationExpression, methodSymbol);
                }
                else
                {
                    AnalyzeExpression(syntaxNodeContext, argumentOperation.Value, invocationExpression, methodSymbol);
                }
            }
        }

        private void AnalyzeParamsArgument(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            IArgumentOperation argumentOperation,
            SyntaxNode invocationExpression,
            IMethodSymbol methodSymbol)
        {
            // TODO naming
            ImmutableArray<IOperation> x;
            if (argumentOperation.Value is IArrayCreationOperation arrayCreationOperation)
            {
                x = arrayCreationOperation.Initializer.ElementValues;
            }
            else
            {
                x = ImmutableArray.Create(argumentOperation.Value);
            }

            // if array elements can't be extracted, analyze argument itself
            foreach (var argumentExpression in x)
            {
                AnalyzeExpression(syntaxNodeContext, argumentExpression, invocationExpression, methodSymbol);
            }
        }

        private void AnalyzeExpression(
            SyntaxNodeAnalysisContext syntaxNodeContext,
            IOperation argumentOperation,
            SyntaxNode invocationExpression,
            IMethodSymbol methodSymbol)
        {
            var argumentOperationSyntax = argumentOperation.Syntax;
            var reentrantSymbol = _reEntrantCallFinder.GetReEntrantCalls(
                syntaxNodeContext.Compilation,
                syntaxNodeContext.SemanticModel,
                invocationExpression,
                argumentOperationSyntax).FirstOrDefault();

            if (reentrantSymbol != null)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptorsProvider.ReEntrantSubstituteCall,
                    argumentOperationSyntax.GetLocation(),
                    methodSymbol.Name,
                    reentrantSymbol.Name,
                    argumentOperationSyntax.ToString());

                syntaxNodeContext.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsPassedByParamsArray(IArgumentOperation argumentOperation)
        {
            return argumentOperation != null &&
                   argumentOperation.Parameter.IsParams &&
                   argumentOperation.Parameter.Type is IArrayTypeSymbol;
        }

        private bool IsPassedByParamsArrayOfCallInfoFunc(SemanticModel semanticModel, IArgumentOperation argumentOperation)
        {
            return IsPassedByParamsArray(argumentOperation) &&
                   argumentOperation.Parameter.Type is IArrayTypeSymbol arrayTypeSymbol &&
                   arrayTypeSymbol.ElementType is INamedTypeSymbol namedTypeSymbol &&
                   namedTypeSymbol.IsCallInfoDelegate(semanticModel);
        }
    }
}