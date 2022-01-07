using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using NSubstitute.Analyzers.Tests.Shared.Text;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public abstract class DiagnosticVerifier : CodeVerifier
{
    protected abstract DiagnosticAnalyzer DiagnosticAnalyzer { get; }

    protected DiagnosticVerifier(WorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }

    protected async Task VerifyDiagnostic(
        string source,
        DiagnosticDescriptor diagnosticDescriptor,
        string overridenDiagnosticMessage = null)
    {
        await VerifyDiagnostics(
            sources: new[] { source },
            diagnosticDescriptor: diagnosticDescriptor,
            overridenDiagnosticMessage: overridenDiagnosticMessage);
    }

    protected async Task VerifyDiagnostics(
        string[] sources,
        DiagnosticDescriptor diagnosticDescriptor,
        string overridenDiagnosticMessage = null)
    {
        diagnosticDescriptor = overridenDiagnosticMessage == null
            ? diagnosticDescriptor
            : diagnosticDescriptor.OverrideMessage(overridenDiagnosticMessage);

        var textParserResult = sources.Select(source => TextParser.GetSpans(source)).ToList();
        var diagnostics = textParserResult.SelectMany(result => CreateDiagnostics(diagnosticDescriptor, result)).ToArray();

        await VerifyDiagnostics(
            sources: textParserResult.Select(result => result.Text).ToArray(),
            expectedDiagnostics: diagnostics);
    }

    protected async Task VerifyDiagnostic(string source, Diagnostic[] diagnostics)
    {
        await VerifyDiagnostics(new[] { source }, diagnostics);
    }

    protected async Task VerifyDiagnostics(string[] sources, Diagnostic[] expectedDiagnostics)
    {
        if (expectedDiagnostics == null || expectedDiagnostics.Length == 0)
        {
            throw new ArgumentException("Diagnostics should not be empty", nameof(expectedDiagnostics));
        }

        await VerifyDiagnosticsInternal(sources, expectedDiagnostics);
    }

    protected async Task VerifyNoDiagnostic(string source)
    {
        await VerifyNoDiagnostics(new[] { source });
    }

    protected async Task VerifyNoDiagnostics(string[] sources)
    {
        await VerifyDiagnosticsInternal(sources, Array.Empty<Diagnostic>());
    }

    protected IEnumerable<Diagnostic> CreateDiagnostics(DiagnosticDescriptor descriptor, TextParserResult result)
    {
        return result.Spans.Select(spanInfo => CreateDiagnostic(descriptor, spanInfo));
    }

    protected Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, LinePositionSpanInfo linePositionSpanInfo)
    {
        var location = Location.Create(
            $"{WorkspaceFactory.DefaultDocumentName}0.{WorkspaceFactory.DocumentExtension}",
            linePositionSpanInfo.Span,
            linePositionSpanInfo.LineSpan);

        return Diagnostic.Create(descriptor, location);
    }

    private async Task VerifyDiagnosticsInternal(string[] sources, Diagnostic[] expectedDiagnostics)
    {
        using (var workspace = new AdhocWorkspace())
        {
            var project = AddProject(
                workspace.CurrentSolution,
                sources);

            var compilation = await project.GetCompilationAsync();

            var compilerDiagnostics = compilation.GetDiagnostics();
            VerifyNoCompilerDiagnosticErrors(compilerDiagnostics);

            var diagnostics = await compilation.GetSortedAnalyzerDiagnostics(DiagnosticAnalyzer, project.AnalyzerOptions);

            VerifyDiagnosticResults(diagnostics, ImmutableArray.Create(expectedDiagnostics));
        }
    }

    private static void VerifyDiagnosticResults(ImmutableArray<Diagnostic> actualResults, ImmutableArray<Diagnostic> expectedResults)
    {
        actualResults.Should().HaveSameCount(expectedResults, "because diagnostic count should match. {0}", actualResults.ToDebugString());
        for (var i = 0; i < expectedResults.Length; i++)
        {
            var actual = actualResults.ElementAt(i);
            var expected = expectedResults[i];

            VerifyLocation(actual.Location, expected.Location);
            var additionalLocations = actual.AdditionalLocations.ToArray();

            additionalLocations.Should().HaveSameCount(expected.AdditionalLocations);

            for (var j = 0; j < additionalLocations.Length; ++j)
            {
                VerifyLocation(additionalLocations[j], expected.AdditionalLocations[j + 1]);
            }

            actual.Id.Should().Be(expected.Id);
            actual.Severity.Should().Be(expected.Severity);
            actual.GetMessage().Should().Be(expected.GetMessage());
        }
    }

    private static void VerifyLocation(
        Location actualLocation,
        Location expectedLocation)
    {
        VerifyFileLinePositionSpan(actualLocation.GetLineSpan(), expectedLocation.GetLineSpan());
    }

    private static void VerifyFileLinePositionSpan(
        FileLinePositionSpan actual,
        FileLinePositionSpan expected)
    {
        actual.Path.Should().Be(expected.Path);
        VerifyLinePosition(actual.StartLinePosition, expected.StartLinePosition, "start");

        VerifyLinePosition(actual.EndLinePosition, expected.EndLinePosition, "end");
    }

    private static void VerifyLinePosition(
        LinePosition actual,
        LinePosition expected,
        string startOrEnd)
    {
        actual.Should().Be(expected, $"because diagnostic should ${startOrEnd} on expected line");

        actual.Character.Should().Be(expected.Character, $"because diagnostic should ${startOrEnd} at expected column");
    }
}