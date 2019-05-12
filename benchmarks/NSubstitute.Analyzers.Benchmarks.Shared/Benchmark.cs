using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

/*
namespace NSubstitute.Analyzers.Benchmarks.Shared
{
    /// <summary>
    /// A benchmark runner for a <see cref="DiagnosticAnalyzer"/>.
    /// </summary>
    public class Benchmark
    {
        private Benchmark(
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
            this.Analyzer = analyzer;
            this.SyntaxNodeActions = syntaxNodeActions;
            this.CompilationStartActions = compilationStartActions;
            this.CompilationActions = compilationActions;
            this.SemanticModelActions = semanticModelActions;
            this.SymbolActions = symbolActions;
            this.CodeBlockStartActions = codeBlockStartActions;
            this.CodeBlockActions = codeBlockActions;
            this.SyntaxTreeActions = syntaxTreeActions;
            this.OperationActions = operationActions;
            this.OperationBlockActions = operationBlockActions;
            this.OperationBlockStartActions = operationBlockStartActions;
        }

        /// <summary>
        /// A context and corresponding action recorded for the analyzer.
        /// </summary>
        public interface IContextAndAction
        {
            /// <summary>
            /// Calls this.Action(this.Context);.
            /// </summary>
            void Run();
        }

        /// <summary>
        /// Gets the analyzer to run benchmarks for.
        /// </summary>
        public DiagnosticAnalyzer Analyzer { get; }

        /// <summary>
        /// Gets the <see cref="SyntaxNodeAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions { get; }

        /// <summary>
        /// Gets the <see cref="CompilationStartAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions { get; }

        /// <summary>
        /// Gets the <see cref="CompilationAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<CompilationAnalysisContext>> CompilationActions { get; }

        /// <summary>
        /// Gets the <see cref="SemanticModelAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions { get; }

        /// <summary>
        /// Gets the <see cref="SymbolAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<SymbolAnalysisContext>> SymbolActions { get; }

        /// <summary>
        /// Gets the <see cref="CodeBlockStartAnalysisContext{TLanguageKindEnum}"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<IContextAndAction> CodeBlockStartActions { get; }

        /// <summary>
        /// Gets the <see cref="CodeBlockAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions { get; }

        /// <summary>
        /// Gets the <see cref="SyntaxTreeAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions { get; }

        /// <summary>
        /// Gets the <see cref="OperationAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<OperationAnalysisContext>> OperationActions { get; }

        /// <summary>
        /// Gets the <see cref="OperationBlockAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions { get; }

        /// <summary>
        /// Gets the <see cref="OperationBlockStartAnalysisContext"/> to invoke actions registered by the analyzer on.
        /// </summary>
        public IReadOnlyList<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static Benchmark Create(Solution solution, DiagnosticAnalyzer analyzer) => CreateAsync(solution, analyzer).GetAwaiter().GetResult();
        
        /// <summary>
        /// Creates a new instance of the <see cref="Benchmark"/> class.
        /// </summary>
        public static async Task<Benchmark> CreateAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var benchmarkAnalyzer = new BenchmarkAnalyzer(analyzer);
            await GetDiagnosticsAsync(solution, benchmarkAnalyzer).ConfigureAwait(false);
            return new Benchmark(
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
        
        /// <summary>
        /// Run the benchmark.
        /// This invokes all actions recorded for <see cref="Analyzer"/>.
        /// </summary>
        public void Run()
        {
            foreach (var contextAndAction in this.SyntaxNodeActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.CompilationStartActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.CompilationActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.SemanticModelActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.SymbolActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.CodeBlockStartActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.CodeBlockActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.SyntaxTreeActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.OperationActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.OperationBlockActions)
            {
                contextAndAction.Run();
            }

            foreach (var contextAndAction in this.OperationBlockStartActions)
            {
                contextAndAction.Run();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(this.Analyzer)}: {this.Analyzer.GetType().Name}";
        }

        /// <summary>
        /// An instance where the analyzer registered and action.
        /// </summary>
        /// <typeparam name="TContext">The type of the analysis context.</typeparam>
        [DebuggerDisplay("{Context}")]
        public class ContextAndAction<TContext> : IContextAndAction
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ContextAndAction{T}"/> class.
            /// </summary>
            public ContextAndAction(TContext context, Action<TContext> action)
            {
                this.Context = context;
                this.Action = action;
            }

            /// <summary>
            /// Gets the <see cref="SyntaxNodeAnalysisContext"/> to pass in when invoking <see cref="Action"/>.
            /// </summary>
            public TContext Context { get; }

            /// <summary>
            /// Gets the action registered for <see cref="Context"/> by BenchmarkAnalysisContext.RegisterSyntaxNodeAction(action, syntaxKinds)"/>.
            /// </summary>
            public Action<TContext> Action { get; }

            /// <summary>
            /// Calls this.Action(this.Context);.
            /// </summary>
            public void Run()
            {
                this.Action(this.Context);
            }
        }

        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        private class BenchmarkAnalyzer : DiagnosticAnalyzer
        {
#pragma warning disable SA1401 // Fields must be private
            internal readonly List<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions = new List<ContextAndAction<SyntaxNodeAnalysisContext>>();
            internal readonly List<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions = new List<ContextAndAction<CompilationStartAnalysisContext>>();
            internal readonly List<ContextAndAction<CompilationAnalysisContext>> CompilationActions = new List<ContextAndAction<CompilationAnalysisContext>>();
            internal readonly List<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions = new List<ContextAndAction<SemanticModelAnalysisContext>>();
            internal readonly List<ContextAndAction<SymbolAnalysisContext>> SymbolActions = new List<ContextAndAction<SymbolAnalysisContext>>();
            internal readonly List<IContextAndAction> CodeBlockStartActions = new List<IContextAndAction>();
            internal readonly List<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions = new List<ContextAndAction<CodeBlockAnalysisContext>>();
            internal readonly List<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions = new List<ContextAndAction<SyntaxTreeAnalysisContext>>();
            internal readonly List<ContextAndAction<OperationAnalysisContext>> OperationActions = new List<ContextAndAction<OperationAnalysisContext>>();
            internal readonly List<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions = new List<ContextAndAction<OperationBlockAnalysisContext>>();
            internal readonly List<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions = new List<ContextAndAction<OperationBlockStartAnalysisContext>>();
#pragma warning restore SA1401 // Fields must be private

            private readonly DiagnosticAnalyzer inner;

            public BenchmarkAnalyzer(DiagnosticAnalyzer inner)
            {
                this.inner = inner;
            }

            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => this.inner.SupportedDiagnostics;

            public override void Initialize(AnalysisContext context)
            {
                var benchmarkContext = new BenchmarkAnalysisContext(this, context);
                this.inner.Initialize(benchmarkContext);
            }

            private class BenchmarkAnalysisContext : AnalysisContext
            {
                private readonly BenchmarkAnalyzer analyzer;
                private readonly AnalysisContext context;

                public BenchmarkAnalysisContext(BenchmarkAnalyzer analyzer, AnalysisContext context)
                {
                    this.analyzer = analyzer;
                    this.context = context;
                }

                public override void EnableConcurrentExecution()
                {
                }

                public override void ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags analysisMode)
                {
                }

                public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
                {
                    this.context.RegisterSyntaxNodeAction(
                        x => this.analyzer.SyntaxNodeActions.Add(new ContextAndAction<SyntaxNodeAnalysisContext>(x, action)),
                        syntaxKinds);
                }

                public override void RegisterCompilationStartAction(Action<CompilationStartAnalysisContext> action)
                {
                    this.context.RegisterCompilationStartAction(
                        x => this.analyzer.CompilationStartActions.Add(new ContextAndAction<CompilationStartAnalysisContext>(x, action)));
                }

                public override void RegisterCompilationAction(Action<CompilationAnalysisContext> action)
                {
                    this.context.RegisterCompilationAction(
                        x => this.analyzer.CompilationActions.Add(new ContextAndAction<CompilationAnalysisContext>(x, action)));
                }

                public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
                {
                    this.context.RegisterSemanticModelAction(
                        x => this.analyzer.SemanticModelActions.Add(new ContextAndAction<SemanticModelAnalysisContext>(x, action)));
                }

                public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
                {
                    this.context.RegisterSymbolAction(
                        x => this.analyzer.SymbolActions.Add(new ContextAndAction<SymbolAnalysisContext>(x, action)),
                        symbolKinds);
                }

                public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
                {
                    this.context.RegisterCodeBlockStartAction<TLanguageKindEnum>(
                        x => this.analyzer.CodeBlockStartActions.Add(new ContextAndAction<CodeBlockStartAnalysisContext<TLanguageKindEnum>>(x, action)));
                }

                public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
                {
                    this.context.RegisterCodeBlockAction(
                        x => this.analyzer.CodeBlockActions.Add(new ContextAndAction<CodeBlockAnalysisContext>(x, action)));
                }

                public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
                {
                    this.context.RegisterSyntaxTreeAction(
                        x => this.analyzer.SyntaxTreeActions.Add(new ContextAndAction<SyntaxTreeAnalysisContext>(x, action)));
                }

                public override void RegisterOperationAction(Action<OperationAnalysisContext> action, ImmutableArray<OperationKind> operationKinds)
                {
                    this.context.RegisterOperationAction(
                        x => this.analyzer.OperationActions.Add(new ContextAndAction<OperationAnalysisContext>(x, action)),
                        operationKinds);
                }

                public override void RegisterOperationBlockAction(Action<OperationBlockAnalysisContext> action)
                {
                    this.context.RegisterOperationBlockAction(
                        x => this.analyzer.OperationBlockActions.Add(new ContextAndAction<OperationBlockAnalysisContext>(x, action)));
                }

                public override void RegisterOperationBlockStartAction(Action<OperationBlockStartAnalysisContext> action)
                {
                    this.context.RegisterOperationBlockStartAction(
                        x => this.analyzer.OperationBlockStartActions.Add(new ContextAndAction<OperationBlockStartAnalysisContext>(x, action)));
                }
            }
        }

        /// <summary>
        /// Creates a solution, compiles it and returns the diagnostics.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <returns>A list with diagnostics per document.</returns>
        public static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer)
        {
            var results = new List<ImmutableArray<Diagnostic>>();
            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync(CancellationToken.None)
                    .ConfigureAwait(false);

                var withAnalyzers = compilation.WithAnalyzers(
                    ImmutableArray.Create(analyzer),
                    project.AnalyzerOptions,
                    CancellationToken.None);
                results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                    .ConfigureAwait(false));
            }

            return results;
        }
    }
}
*/