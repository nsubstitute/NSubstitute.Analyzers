using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        protected IDiagnosticDescriptorsProvider DiagnosticDescriptorsProvider { get; }

        protected AbstractDiagnosticAnalyzer(IDiagnosticDescriptorsProvider diagnosticDescriptorsProvider)
        {
            DiagnosticDescriptorsProvider = diagnosticDescriptorsProvider;
        }

        public sealed override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            InitializeAnalyzer(context);
        }

        protected abstract void InitializeAnalyzer(AnalysisContext context);
    }
}