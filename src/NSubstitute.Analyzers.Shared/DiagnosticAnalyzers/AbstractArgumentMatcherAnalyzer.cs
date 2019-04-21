using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class
        AbstractArgumentMatcherAnalyzer<TSyntaxKind, TInvocationExpressionSyntax, TMemberAccessExpression> : AbstractDiagnosticAnalyzer
        where TInvocationExpressionSyntax : SyntaxNode
        where TMemberAccessExpression : SyntaxNode
        where TSyntaxKind : struct
    {
        private static readonly ImmutableDictionary<string, HashSet<string>> ArgMethodNames = new Dictionary<string, HashSet<string>>
        {
            [MetadataNames.ArgIsMethodName] = new HashSet<string> { MetadataNames.NSubstituteArgFullTypeName, MetadataNames.NSubstituteArgCompatFullTypeName },
            [MetadataNames.ArgAnyMethodName] = new HashSet<string> { MetadataNames.NSubstituteArgFullTypeName, MetadataNames.NSubstituteArgCompatFullTypeName}
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> MockSetupNamesMap = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] =
                MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsForAnyArgsMethod] =
                MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullMethod] = MetadataNames.NSubstituteReturnsExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullForAnyArgsMethod] =
                MetadataNames.NSubstituteReturnsExtensionsFullTypeName
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> ReceivedMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReceivedMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReceivedWithAnyArgsMethod] =
                MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod] =
                MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<string, string> WhenMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteWhenMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteWhenForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        }.ToImmutableDictionary();

        protected abstract TSyntaxKind InvocationExpressionKind { get; }

        protected AbstractArgumentMatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            : base(diagnosticDescriptorsProvider)
        {
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(DiagnosticDescriptorsProvider.ArgumentMatcherUsedOutsideOfCall);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(compilationContext =>
            {
                var analyzer = GetMatcher();
                compilationContext.RegisterSyntaxNodeAction(analyzer.AnalyzeInvocation, InvocationExpressionKind);

                compilationContext.RegisterCompilationEndAction(analyzer.EndAction);
            });
        }

        protected abstract MatcherAnalyzer GetMatcher();
        
        internal abstract class MatcherAnalyzer
        {
            private readonly IDiagnosticDescriptorsProvider _diagnosticDescriptorsProvider;
            
            private HashSet<SyntaxNode> ReceivedInOrderNodes { get; } = new HashSet<SyntaxNode>();

            private HashSet<SyntaxNode> WhenNodes { get; } = new HashSet<SyntaxNode>();

            private Dictionary<SyntaxNode, List<SyntaxNode>> PotentialMissusedNodes { get; } = new Dictionary<SyntaxNode, List<SyntaxNode>>();
            
            public MatcherAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
            {
                _diagnosticDescriptorsProvider = diagnosticDescriptorsProvider;
            }
            
            protected bool IsSetupLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
            {
                if (symbol == null)
                {
                    return false;
                }

                if (MockSetupNamesMap.TryGetValue(symbol.Name, out var containingType) == false)
                {
                    return false;
                }

                return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                           StringComparison.OrdinalIgnoreCase) == true &&
                       symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) ==
                       true;
            }

            protected bool IsReceivedLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
            {
                if (symbol == null)
                {
                    return false;
                }

                if (ReceivedMethodNames.TryGetValue(symbol.Name, out var containingType) == false)
                {
                    return false;
                }

                return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                           StringComparison.OrdinalIgnoreCase) == true &&
                       symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) ==
                       true;
            }

            protected bool IsWhenLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
            {
                if (symbol == null)
                {
                    return false;
                }

                if (WhenMethodNames.TryGetValue(symbol.Name, out var containingType) == false)
                {
                    return false;
                }

                return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                           StringComparison.OrdinalIgnoreCase) == true &&
                       symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) ==
                       true;
            }

            protected bool IsReceivedInOrderMethod(SyntaxNodeAnalysisContext syntaxNodeContext, ISymbol symbol)
            {
                if (symbol == null)
                {
                    return false;
                }

                if (symbol.Name != MetadataNames.NSubstituteInOrderMethod)
                {
                    return false;
                }

                return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                           StringComparison.OrdinalIgnoreCase) == true &&
                       symbol.ContainingType?.ToString().Equals(MetadataNames.NSubstituteReceivedFullTypeName,
                           StringComparison.OrdinalIgnoreCase) == true;
            }

            
            protected abstract SyntaxNode FindEnclosingExpression(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, TInvocationExpressionSyntax invocationExpression);

            protected abstract bool IsFollowedBySetupInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

            protected abstract bool IsPrecededByReceivedInvocation(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode invocationExpressionSyntax);

            public void AnalyzeInvocation(SyntaxNodeAnalysisContext syntaxNodeContext)
            {
                var invocationExpression = (TInvocationExpressionSyntax) syntaxNodeContext.Node;
                var methodSymbolInfo = syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression);

                if (methodSymbolInfo.Symbol?.Kind != SymbolKind.Method)
                {
                    return;
                }

                var methodSymbol = (IMethodSymbol) methodSymbolInfo.Symbol;
                
                if (IsArgLikeMethod(syntaxNodeContext, invocationExpression, methodSymbol.Name) == false)
                {
                    if (IsWhenLikeMethod(syntaxNodeContext, methodSymbol))
                    {
                        var substitutionNodeFinder = GetFinder();
                        foreach (var syntaxNode in substitutionNodeFinder.FindForWhenExpression(syntaxNodeContext, invocationExpression))
                        {
                            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
                            var actualNode = syntaxNode is TMemberAccessExpression && symbol is IMethodSymbol _ ? syntaxNode.Parent : syntaxNode;
                            WhenNodes.Add(actualNode);
                        }
                    }

                    if (IsReceivedInOrderMethod(syntaxNodeContext, methodSymbol))
                    {
                        var substitutionNodeFinder = GetFinder();
                        foreach (var syntaxNode in substitutionNodeFinder.FindForReceivedInOrderExpression(syntaxNodeContext, invocationExpression).ToList())
                        {
                            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
                            var actualNode = syntaxNode is TMemberAccessExpression && symbol is IMethodSymbol _ ? syntaxNode.Parent : syntaxNode;
                            ReceivedInOrderNodes.Add(actualNode);
                        }
                    }
                }
                else
                {

                    // find enclosing type
                    var enclosingExpression = FindEnclosingExpression(syntaxNodeContext, invocationExpression);

                    if (enclosingExpression != null &&
                        IsFollowedBySetupInvocation(syntaxNodeContext, enclosingExpression) == false &&
                        IsPrecededByReceivedInvocation(syntaxNodeContext, enclosingExpression) == false)
                    {

                        if (PotentialMissusedNodes.TryGetValue(enclosingExpression, out var nodes))
                        {
                            nodes.Add(invocationExpression);
                        }
                        else
                        {
                            PotentialMissusedNodes.Add(enclosingExpression, new List<SyntaxNode> { invocationExpression });
                        }
                        
                    }
                }
            }

            // TODO unify across all analyzers
            private bool IsArgLikeMethod(SyntaxNodeAnalysisContext syntaxNodeContext, SyntaxNode syntax,
                string memberName)
            {
                if (ArgMethodNames.TryGetValue(memberName, out var containingType) == false)
                {
                    return false;
                }

                var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntax);

                return symbol.Symbol?.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName,
                           StringComparison.OrdinalIgnoreCase) == true &&
                       containingType.Contains(symbol.Symbol?.ContainingType?.ToString() ?? string.Empty);
            }

            public void EndAction(CompilationAnalysisContext obj)
            {
                foreach (var potential in PotentialMissusedNodes)
                {
                    if (WhenNodes.Contains(potential.Key) == false && ReceivedInOrderNodes.Contains(potential.Key) == false)
                    {
                        foreach (var arg in potential.Value)
                        {
                            var diagnostic = Diagnostic.Create(
                                _diagnosticDescriptorsProvider.ArgumentMatcherUsedOutsideOfCall,
                                arg.GetLocation());

                            obj.ReportDiagnostic(diagnostic);   
                        }
                    }
                }
                
            }

            protected abstract ISubstitutionNodeFinder<TInvocationExpressionSyntax> GetFinder();
        }
    }
}