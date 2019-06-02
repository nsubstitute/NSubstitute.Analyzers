using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Benchmarks.Shared
{
    public class AnalyzerBenchmark
    {
        private AnalyzerBenchmark(
            DiagnosticAnalyzer analyzer,
            IReadOnlyList<ContextAndAction<SyntaxNodeAnalysisContext>> syntaxNodeActions,
            IReadOnlyList<ContextAndAction<CompilationStartAnalysisContext>> compilationStartActions,
            IReadOnlyList<ContextAndAction<CompilationAnalysisContext>> compilationActions,
            IReadOnlyList<ContextAndAction<SemanticModelAnalysisContext>> semanticModelActions,
            IReadOnlyList<ContextAndAction<SymbolAnalysisContext>> symbolActions,
            IReadOnlyList<IContextAndAction> codeBlockStartActions,
            IReadOnlyList<ContextAndAction<CodeBlockAnalysisContext>> codeBlockActions,
            IReadOnlyList<ContextAndAction<SyntaxTreeAnalysisContext>> syntaxTreeActions,
            IReadOnlyList<ContextAndAction<OperationAnalysisContext>> operationActions,
            IReadOnlyList<ContextAndAction<OperationBlockAnalysisContext>> operationBlockActions,
            IReadOnlyList<ContextAndAction<OperationBlockStartAnalysisContext>> operationBlockStartActions)
        {
            Analyzer = analyzer;
            SyntaxNodeActions = syntaxNodeActions;
            CompilationStartActions = compilationStartActions;
            CompilationActions = compilationActions;
            SemanticModelActions = semanticModelActions;
            SymbolActions = symbolActions;
            CodeBlockStartActions = codeBlockStartActions;
            CodeBlockActions = codeBlockActions;
            SyntaxTreeActions = syntaxTreeActions;
            OperationActions = operationActions;
            OperationBlockActions = operationBlockActions;
            OperationBlockStartActions = operationBlockStartActions;
        }

        public interface IContextAndAction
        {
            void Run();
        }

        public DiagnosticAnalyzer Analyzer { get; }

        public IReadOnlyList<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions { get; }

        public IReadOnlyList<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions { get; }

        public IReadOnlyList<ContextAndAction<CompilationAnalysisContext>> CompilationActions { get; }

        public IReadOnlyList<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions { get; }

        public IReadOnlyList<ContextAndAction<SymbolAnalysisContext>> SymbolActions { get; }

        public IReadOnlyList<IContextAndAction> CodeBlockStartActions { get; }

        public IReadOnlyList<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions { get; }

        public IReadOnlyList<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions { get; }

        public IReadOnlyList<ContextAndAction<OperationAnalysisContext>> OperationActions { get; }

        public IReadOnlyList<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions { get; }

        public IReadOnlyList<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions { get; }

        public static AnalyzerBenchmark CreateBenchmark(Solution solution, DiagnosticAnalyzer analyzer) => CreateBenchmarkAsync(solution, analyzer).GetAwaiter().GetResult();

        public static async Task<AnalyzerBenchmark> CreateBenchmarkAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var benchmarkAnalyzer = new BenchmarkAnalyzer(analyzer);
            await GetDiagnosticsAsync(solution, benchmarkAnalyzer).ConfigureAwait(false);
            return new AnalyzerBenchmark(
                analyzer,
                benchmarkAnalyzer.SyntaxNodeActions,
                benchmarkAnalyzer.CompilationStartActions,
                benchmarkAnalyzer.CompilationActions,
                benchmarkAnalyzer.SemanticModelActions,
                benchmarkAnalyzer.SymbolActions,
                benchmarkAnalyzer.CodeBlockStartActions,
                benchmarkAnalyzer.CodeBlockActions,
                benchmarkAnalyzer.SyntaxTreeActions,
                benchmarkAnalyzer.OperationActions,
                benchmarkAnalyzer.OperationBlockActions,
                benchmarkAnalyzer.OperationBlockStartActions);
        }

        public void Run()
        {
            foreach (var contextAndAction in SyntaxNodeActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in CompilationStartActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in CompilationActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in SemanticModelActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in SymbolActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in CodeBlockStartActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in CodeBlockActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in SyntaxTreeActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in OperationActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in OperationBlockActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in OperationBlockStartActions)
            {
                contextAndAction.Run();
            }
        }

        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                    .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                    .ConfigureAwait(false));
            }

            return results;
        }

        public class ContextAndAction<TContext> : IContextAndAction
        {
            public ContextAndAction(TContext context, Action<TContext> action)
            {
                Context = context;
                Action = action;
            }

            public TContext Context { get; }

            public Action<TContext> Action { get; }

            public void Run()
            {
                Action(Context);
            }
        }

        [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
        private class BenchmarkAnalyzer : DiagnosticAnalyzer
        {
            internal List<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions { get; } = new List<ContextAndAction<SyntaxNodeAnalysisContext>>();

            internal List<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions { get; } = new List<ContextAndAction<CompilationStartAnalysisContext>>();

            internal List<ContextAndAction<CompilationAnalysisContext>> CompilationActions { get; } = new List<ContextAndAction<CompilationAnalysisContext>>();

            internal List<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions { get; } = new List<ContextAndAction<SemanticModelAnalysisContext>>();

            internal List<ContextAndAction<SymbolAnalysisContext>> SymbolActions { get; } = new List<ContextAndAction<SymbolAnalysisContext>>();

            internal List<IContextAndAction> CodeBlockStartActions { get; } = new List<IContextAndAction>();

            internal List<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions { get; } = new List<ContextAndAction<CodeBlockAnalysisContext>>();

            internal List<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions { get; } = new List<ContextAndAction<SyntaxTreeAnalysisContext>>();

            internal List<ContextAndAction<OperationAnalysisContext>> OperationActions { get; } = new List<ContextAndAction<OperationAnalysisContext>>();

            internal List<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions { get; } = new List<ContextAndAction<OperationBlockAnalysisContext>>();

            internal List<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions { get; } = new List<ContextAndAction<OperationBlockStartAnalysisContext>>();

            private readonly DiagnosticAnalyzer _inner;

            public BenchmarkAnalyzer(DiagnosticAnalyzer inner)
            {
                _inner = inner;
            }

            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => _inner.SupportedDiagnostics;

            public override void Initialize(AnalysisContext context)
            {
                var benchmarkContext = new BenchmarkAnalysisContext(this, context);
                _inner.Initialize(benchmarkContext);
            }

            private class BenchmarkAnalysisContext : AnalysisContext
            {
                private readonly BenchmarkAnalyzer _analyzer;
                private readonly AnalysisContext _context;

                public BenchmarkAnalysisContext(BenchmarkAnalyzer analyzer, AnalysisContext context)
                {
                    _analyzer = analyzer;
                    _context = context;
                }

                public override void EnableConcurrentExecution()
                {
                }

                public override void ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags analysisMode)
                {
                }

                public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
                {
                    _context.RegisterSyntaxNodeAction(
                        x => _analyzer.SyntaxNodeActions.Add(new ContextAndAction<SyntaxNodeAnalysisContext>(x, action)),
                        syntaxKinds);
                }

                public override void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
                {
                    _context.RegisterCompilationStartAction(
                        x => _analyzer.CompilationStartActions.Add(new ContextAndAction<CompilationStartAnalysisContext>(x, action)));
                }

                public override void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
                {
                    _context.RegisterCompilationAction(
                        x => _analyzer.CompilationActions.Add(new ContextAndAction<CompilationAnalysisContext>(x, action)));
                }

                public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
                {
                    _context.RegisterSemanticModelAction(
                        x => _analyzer.SemanticModelActions.Add(new ContextAndAction<SemanticModelAnalysisContext>(x, action)));
                }

                public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
                {
                    _context.RegisterSymbolAction(
                        x => _analyzer.SymbolActions.Add(new ContextAndAction<SymbolAnalysisContext>(x, action)),
                        symbolKinds);
                }

                public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
                {
                    _context.RegisterCodeBlockStartAction<TLanguageKindEnum>(
                        x => _analyzer.CodeBlockStartActions.Add(new ContextAndAction<CodeBlockStartAnalysisContext<TLanguageKindEnum>>(x, action)));
                }

                public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
                {
                    _context.RegisterCodeBlockAction(
                        x => _analyzer.CodeBlockActions.Add(new ContextAndAction<CodeBlockAnalysisContext>(x, action)));
                }

                public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
                {
                    _context.RegisterSyntaxTreeAction(
                        x => _analyzer.SyntaxTreeActions.Add(new ContextAndAction<SyntaxTreeAnalysisContext>(x, action)));
                }

                public override void RegisterOperationAction(Action<OperationAnalysisContext> action, ImmutableArray<OperationKind> operationKinds)
                {
                    _context.RegisterOperationAction(
                        x => _analyzer.OperationActions.Add(new ContextAndAction<OperationAnalysisContext>(x, action)),
                        operationKinds);
                }

                public override void RegisterOperationBlockAction(Action<OperationBlockAnalysisContext> action)
                {
                    _context.RegisterOperationBlockAction(
                        x => _analyzer.OperationBlockActions.Add(new ContextAndAction<OperationBlockAnalysisContext>(x, action)));
                }

                public override void RegisterOperationBlockStartAction(Action<OperationBlockStartAnalysisContext> action)
                {
                    _context.RegisterOperationBlockStartAction(
                        x => _analyzer.OperationBlockStartActions.Add(new ContextAndAction<OperationBlockStartAnalysisContext>(x, action)));
                }
            }
        }
    }
}
