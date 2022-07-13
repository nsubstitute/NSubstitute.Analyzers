// Src: https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/aac107d0f73332cf7c692f05e31470f3bd1104f6/Gu.Roslyn.Asserts/Benchmark.cs
// Copyright (c) Johan Larsson. All rights reserved. Licensed under the MIT License. See License at at https://github.com/GuOrg/Gu.Roslyn.Asserts/blob/master/LICENSE for license information.
// Some modifications made (such as namespace and support for VisualBasic) for NSubstitute.Analyzers project.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Benchmarks.Shared;

public class AnalyzerBenchmark
{
    private readonly BenchmarkAnalyzer _benchmarkAnalyzer;
    private readonly Solution _solution;

    private AnalyzerBenchmark(DiagnosticAnalyzer analyzer, BenchmarkAnalyzer benchmarkAnalyzer, Solution solution)
    {
        _benchmarkAnalyzer = benchmarkAnalyzer;
        _solution = solution;
        Analyzer = analyzer;
        RefreshActions();
    }

    public void RefreshActions()
    {
        _benchmarkAnalyzer.ClearActions();

        GetDiagnosticsAsync(_solution, _benchmarkAnalyzer)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        SyntaxNodeActions = _benchmarkAnalyzer.SyntaxNodeActions;
        CompilationStartActions = _benchmarkAnalyzer.CompilationStartActions;
        CompilationEndActions = _benchmarkAnalyzer.CompilationEndActions;
        CompilationActions = _benchmarkAnalyzer.CompilationActions;
        SemanticModelActions = _benchmarkAnalyzer.SemanticModelActions;
        SymbolActions = _benchmarkAnalyzer.SymbolActions;
        CodeBlockStartActions = _benchmarkAnalyzer.CodeBlockStartActions;
        CodeBlockActions = _benchmarkAnalyzer.CodeBlockActions;
        SyntaxTreeActions = _benchmarkAnalyzer.SyntaxTreeActions;
        OperationActions = _benchmarkAnalyzer.OperationActions;
        OperationBlockActions = _benchmarkAnalyzer.OperationBlockActions;
        OperationBlockStartActions = _benchmarkAnalyzer.OperationBlockStartActions;
    }

    private static async Task<IReadOnlyList<ImmutableArray<Diagnostic>>> GetDiagnosticsAsync(Solution solution, DiagnosticAnalyzer analyzer)
    {
        var results = new List<ImmutableArray<Diagnostic>>();
        foreach (var project in solution.Projects)
        {
            project.TryGetCompilation(out var compilation);

            var withAnalyzers = compilation.WithAnalyzers(
                ImmutableArray.Create(analyzer),
                project.AnalyzerOptions);
            results.Add(await withAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                .ConfigureAwait(false));
        }

        return results;
    }

    public interface IContextAndAction
    {
        void Run();
    }

    public DiagnosticAnalyzer Analyzer { get; }

    public IReadOnlyList<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions { get; private set; }

    public IReadOnlyList<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions { get; private set; }

    public IReadOnlyList<ContextAndAction<CompilationAnalysisContext>> CompilationEndActions { get; private set; }

    public IReadOnlyList<ContextAndAction<CompilationAnalysisContext>> CompilationActions { get; private set; }

    public IReadOnlyList<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions { get; private set; }

    public IReadOnlyList<ContextAndAction<SymbolAnalysisContext>> SymbolActions { get; private set; }

    public IReadOnlyList<IContextAndAction> CodeBlockStartActions { get; private set; }

    public IReadOnlyList<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions { get; private set; }

    public IReadOnlyList<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions { get; private set; }

    public IReadOnlyList<ContextAndAction<OperationAnalysisContext>> OperationActions { get; private set; }

    public IReadOnlyList<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions { get; private set; }

    public IReadOnlyList<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions { get; private set; }

    public static AnalyzerBenchmark CreateBenchmark(Solution solution, DiagnosticAnalyzer analyzer)
    {
        var benchmarkAnalyzer = new BenchmarkAnalyzer(analyzer);
        return new AnalyzerBenchmark(
            analyzer,
            benchmarkAnalyzer,
            solution);
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

        foreach (var contextAndAction in CompilationEndActions)
        {
            contextAndAction.Run();
        }
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
        internal List<ContextAndAction<SyntaxNodeAnalysisContext>> SyntaxNodeActions { get; } = new ();

        internal List<ContextAndAction<CompilationStartAnalysisContext>> CompilationStartActions { get; } = new ();

        internal List<ContextAndAction<CompilationAnalysisContext>> CompilationEndActions { get; } = new ();

        internal List<ContextAndAction<CompilationAnalysisContext>> CompilationActions { get; } = new ();

        internal List<ContextAndAction<SemanticModelAnalysisContext>> SemanticModelActions { get; } = new ();

        internal List<ContextAndAction<SymbolAnalysisContext>> SymbolActions { get; } = new ();

        internal List<IContextAndAction> CodeBlockStartActions { get; } = new ();

        internal List<ContextAndAction<CodeBlockAnalysisContext>> CodeBlockActions { get; } = new ();

        internal List<ContextAndAction<SyntaxTreeAnalysisContext>> SyntaxTreeActions { get; } = new ();

        internal List<ContextAndAction<OperationAnalysisContext>> OperationActions { get; } = new ();

        internal List<ContextAndAction<OperationBlockAnalysisContext>> OperationBlockActions { get; } = new ();

        internal List<ContextAndAction<OperationBlockStartAnalysisContext>> OperationBlockStartActions { get; } = new ();

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

        public void ClearActions()
        {
            SyntaxNodeActions.Clear();
            CompilationStartActions.Clear();
            CompilationEndActions.Clear();
            CompilationActions.Clear();
            SemanticModelActions.Clear();
            SymbolActions.Clear();
            CodeBlockStartActions.Clear();
            CodeBlockActions.Clear();
            SyntaxTreeActions.Clear();
            OperationActions.Clear();
            OperationBlockActions.Clear();
            OperationBlockStartActions.Clear();
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
                    x =>
                    {
                        _analyzer.CompilationStartActions.Add(new ContextAndAction<CompilationStartAnalysisContext>(x, action));
                        action(new BenchmarkCompilationStartAnalysisContext(_analyzer, x));
                    });
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

        private class BenchmarkCompilationStartAnalysisContext : CompilationStartAnalysisContext
        {
            private readonly BenchmarkAnalyzer _analyzer;
            private readonly CompilationStartAnalysisContext _inner;

#pragma warning disable RS1012 // Start action has no registered actions
            public BenchmarkCompilationStartAnalysisContext(BenchmarkAnalyzer analyzer, CompilationStartAnalysisContext inner)
                : base(inner.Compilation, inner.Options, inner.CancellationToken)
            {
                _analyzer = analyzer;
                _inner = inner;
            }
#pragma warning restore RS1012

            public override void RegisterCompilationEndAction(Action<CompilationAnalysisContext> action)
            {
                _inner.RegisterCompilationEndAction(x => _analyzer.CompilationEndActions.Add(new ContextAndAction<CompilationAnalysisContext>(x, action)));
            }

            public override void RegisterSemanticModelAction(Action<SemanticModelAnalysisContext> action)
            {
                _inner.RegisterSemanticModelAction(x => _analyzer.SemanticModelActions.Add(new ContextAndAction<SemanticModelAnalysisContext>(x, action)));
            }

            public override void RegisterSymbolAction(Action<SymbolAnalysisContext> action, ImmutableArray<SymbolKind> symbolKinds)
            {
                _inner.RegisterSymbolAction(
                    x => _analyzer.SymbolActions.Add(new ContextAndAction<SymbolAnalysisContext>(x, action)), symbolKinds);
            }

            public override void RegisterCodeBlockStartAction<TLanguageKindEnum>(Action<CodeBlockStartAnalysisContext<TLanguageKindEnum>> action)
            {
                _inner.RegisterCodeBlockStartAction<TLanguageKindEnum>(x =>
                    _analyzer.CodeBlockStartActions.Add(
                        new ContextAndAction<CodeBlockStartAnalysisContext<TLanguageKindEnum>>(x, action)));
            }

            public override void RegisterCodeBlockAction(Action<CodeBlockAnalysisContext> action)
            {
                _inner.RegisterCodeBlockAction(x => _analyzer.CodeBlockActions.Add(new ContextAndAction<CodeBlockAnalysisContext>(x, action)));
            }

            public override void RegisterSyntaxTreeAction(Action<SyntaxTreeAnalysisContext> action)
            {
                _inner.RegisterSyntaxTreeAction(x => _analyzer.SyntaxTreeActions.Add(new ContextAndAction<SyntaxTreeAnalysisContext>(x, action)));
            }

            public override void RegisterSyntaxNodeAction<TLanguageKindEnum>(Action<SyntaxNodeAnalysisContext> action, ImmutableArray<TLanguageKindEnum> syntaxKinds)
            {
                _inner.RegisterSyntaxNodeAction(x => _analyzer.SyntaxNodeActions.Add(new ContextAndAction<SyntaxNodeAnalysisContext>(x, action)), syntaxKinds);
            }
        }
    }
}