using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using NSubstitute.Analyzers.Tests.Shared.Text;

namespace NSubstitute.Analyzers.Tests.Shared;

public abstract class CodeVerifier
{
    protected WorkspaceFactory WorkspaceFactory { get; }

    protected virtual string? AnalyzerSettings { get; }

    protected CodeVerifier(WorkspaceFactory workspaceFactory)
    {
        WorkspaceFactory = workspaceFactory;
        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture =
            CultureInfo.GetCultureInfoByIetfLanguageTag("en-US");
    }

    protected static TextParser TextParser { get; } = TextParser.Default;

    protected Project AddProject(Solution solution, string source)
    {
        return WorkspaceFactory.AddProject(solution, source, AnalyzerSettings);
    }

    protected Project AddProject(Solution solution, string[] sources)
    {
        return WorkspaceFactory.AddProject(solution, sources, AnalyzerSettings);
    }

    protected static void VerifyNoCompilerDiagnosticErrors(ImmutableArray<Diagnostic> diagnostics)
    {
        var compilationErrorDiagnostics = diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (compilationErrorDiagnostics.Count > 0)
        {
            Execute.Assertion.Fail($"Compilation failed. Errors encountered {compilationErrorDiagnostics.ToDebugString()}");
        }
    }

    protected static Project UpdateNSubstituteMetadataReference(Project project, NSubstituteVersion version)
    {
        if (version == NSubstituteVersion.Latest)
        {
            return project;
        }

        project = project.RemoveMetadataReference(RuntimeMetadataReference.NSubstituteLatestReference);

        project = version switch
        {
            NSubstituteVersion.NSubstitute4_2_2 => project.AddMetadataReference(RuntimeMetadataReference.NSubstitute422Reference),
            _ => throw new ArgumentException($"NSubstitute {version} is not supported", nameof(version))
        };

        return project;
    }
}