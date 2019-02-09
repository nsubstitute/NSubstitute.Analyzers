using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.Extensions;
using NSubstitute.Analyzers.Tests.Shared.Text;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// All methods are static.
    /// </summary>
    public abstract class DiagnosticVerifier
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);

        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        private static readonly MetadataReference NSubstituteReference = MetadataReference.CreateFromFile(typeof(Substitute).Assembly.Location);

        private static readonly MetadataReference ValueTaskReference = MetadataReference.CreateFromFile(typeof(ValueTask<>).Assembly.Location);

        public static string DefaultFilePathPrefix { get; } = "Test";

        public static string TestProjectName { get; } = "TestProject";

        public static TextParser TextParser { get; } = TextParser.Default;

        protected abstract string Language { get; }

        protected abstract string FileExtension { get; }

        protected DiagnosticVerifier()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture =
                CultureInfo.GetCultureInfoByIetfLanguageTag("en-US");
        }

        protected async Task VerifyDiagnostic(string source, DiagnosticDescriptor diagnosticDescriptor, string overridenDiagnosticMessage = null)
        {
            await VerifyDiagnostics(new[] { source }, diagnosticDescriptor, overridenDiagnosticMessage);
        }

        protected async Task VerifyDiagnostics(string[] sources, DiagnosticDescriptor diagnosticDescriptor, string overridenDiagnosticMessage = null)
        {
            diagnosticDescriptor = overridenDiagnosticMessage == null
                ? diagnosticDescriptor
                : diagnosticDescriptor.OverrideMessage(overridenDiagnosticMessage);

            var textParserResult = sources.Select(source => TextParser.GetSpans(source)).ToList();
            var diagnostics = textParserResult.SelectMany(result => result.Spans.Select(span => CreateDiagnostic(diagnosticDescriptor, span.Span, span.LineSpan))).ToArray();
            await VerifyDiagnostics(textParserResult.Select(result => result.Text).ToArray(), diagnostics);
        }

        protected async Task VerifyDiagnostic(string source, Diagnostic[] diagnostics)
        {
            await VerifyDiagnostics(new[] { source }, diagnostics);
        }

        protected async Task VerifyDiagnostics(string[] sources, Diagnostic[] diagnostics)
        {
            if (diagnostics == null || diagnostics.Length == 0)
            {
                throw new ArgumentException("Diagnostics should not be empty", nameof(diagnostics));
            }

            await VerifyDiagnostics(sources, GetDiagnosticAnalyzer(), diagnostics, false);
        }

        protected async Task VerifyNoDiagnostic(string source)
        {
            await VerifyNoDiagnostics(new[] { source });
        }

        protected async Task VerifyNoDiagnostics(string[] sources)
        {
            await VerifyDiagnostics(sources, GetDiagnosticAnalyzer(), Array.Empty<Diagnostic>(), false);
        }

        protected abstract DiagnosticAnalyzer GetDiagnosticAnalyzer();

        protected abstract CompilationOptions GetCompilationOptions();

        protected virtual IEnumerable<MetadataReference> GetAdditionalMetadataReferences()
        {
            return Enumerable.Empty<MetadataReference>();
        }

        protected virtual string GetSettings()
        {
            return null;
        }

        protected async Task<Diagnostic[]> GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer, Document[] documents, bool allowCompilationErrors)
        {
            if (documents == null)
            {
                throw new ArgumentNullException(nameof(documents));
            }

            var projects = new HashSet<Project>();
            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            var diagnostics = new List<Diagnostic>();

            foreach (var project in projects)
            {
                var compilation = await project.GetCompilationAsync();
                var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer), project.AnalyzerOptions);

                if (!allowCompilationErrors)
                {
                    AssertThatCompilationSucceeded(compilationWithAnalyzers);
                }

                var diags = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

                foreach (var diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                    {
                        diagnostics.Add(diag);
                    }
                    else
                    {
                        foreach (var document in documents)
                        {
                            var tree = await document.GetSyntaxTreeAsync();
                            if (tree == diag.Location.SourceTree)
                            {
                                diagnostics.Add(diag);
                            }
                        }
                    }
                }
            }

            var results = SortDiagnostics(diagnostics);
            diagnostics.Clear();
            return results;
        }

        protected Project CreateProject(string[] sources)
        {
            var fileNamePrefix = DefaultFilePathPrefix;
            var referencedAssemblies = typeof(Substitute).Assembly.GetReferencedAssemblies();
            var systemRuntimeReference = GetAssemblyReference(referencedAssemblies, "System.Runtime");
            var systemThreadingTasksReference = GetAssemblyReference(referencedAssemblies, "System.Threading.Tasks");

            var projectId = ProjectId.CreateNewId(TestProjectName);

            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var compilationOptions = GetCompilationOptions();

                var solution = adhocWorkspace
                    .CurrentSolution
                    .AddProject(projectId, TestProjectName, TestProjectName, Language)
                    .WithProjectCompilationOptions(projectId, compilationOptions)
                    .AddMetadataReference(projectId, CorlibReference)
                    .AddMetadataReference(projectId, SystemCoreReference)
                    .AddMetadataReference(projectId, CodeAnalysisReference)
                    .AddMetadataReference(projectId, NSubstituteReference)
                    .AddMetadataReference(projectId, ValueTaskReference)
                    .AddMetadataReference(projectId, systemRuntimeReference)
                    .AddMetadataReference(projectId, systemThreadingTasksReference)
                    .AddMetadataReferences(projectId, GetAdditionalMetadataReferences());

                var count = 0;
                foreach (var source in sources)
                {
                    var newFileName = fileNamePrefix + count + "." + FileExtension;
                    var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                    solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                    count++;
                }

                var settings = GetSettings();
                if (!string.IsNullOrEmpty(settings))
                {
                    var documentId = DocumentId.CreateNewId(projectId);
                    solution = solution.AddAdditionalDocument(documentId, AnalyzersSettings.AnalyzerFileName, settings);
                }

                return solution.GetProject(projectId);
            }
        }

        protected Diagnostic CreateDiagnostic(DiagnosticDescriptor descriptor, TextSpan span, LinePositionSpan lineSpan)
        {
            var location = Location.Create($"{DefaultFilePathPrefix}0.{FileExtension}", span, lineSpan);

            return Diagnostic.Create(descriptor, location);
        }

        private static void VerifyDiagnosticResults(Diagnostic[] actualResults, DiagnosticAnalyzer analyzer, params Diagnostic[] expectedResults)
        {
            actualResults.Should().HaveSameCount(expectedResults, "because diagnostic count should match. {0}", FormatDiagnostics(actualResults));
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

        private static string FormatDiagnostics(params Diagnostic[] diagnostics)
        {
            var formattedName = diagnostics.Length == 0 ? "no diagnostic" : string.Join(Environment.NewLine, diagnostics.Select(d => d.ToString()));

            return $"{Environment.NewLine}Diagnostics:{Environment.NewLine}{formattedName}";
        }

        private async Task VerifyDiagnostics(string[] sources, DiagnosticAnalyzer analyzer, Diagnostic[] expected, bool allowCompilationErrors)
        {
            var diagnostics = await GetSortedDiagnostics(sources, analyzer, allowCompilationErrors);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }

        private Document[] GetDocuments(string[] sources)
        {
            var project = CreateProject(sources);
            var documents = project.Documents.ToArray();

            if (sources.Length != documents.Length)
            {
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        private static void AssertThatCompilationSucceeded(CompilationWithAnalyzers compilationWithAnalyzers)
        {
            var compilationDiagnostics = compilationWithAnalyzers.Compilation.GetDiagnostics();

            if (compilationDiagnostics.Any(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error))
            {
                var messageBuilder = new StringBuilder();
                messageBuilder.Append("Test code compilation failed. Error(s) encountered:");
                foreach (var diagnostic in compilationDiagnostics)
                {
                    messageBuilder.AppendLine();
                    messageBuilder.AppendFormat("  {0}", diagnostic);
                }

                throw new ArgumentException(messageBuilder.ToString());
            }
        }

        private async Task<Diagnostic[]> GetSortedDiagnostics(string[] sources, DiagnosticAnalyzer analyzer, bool allowCompilationErrors)
        {
            return await GetSortedDiagnosticsFromDocuments(analyzer, GetDocuments(sources), allowCompilationErrors);
        }

        private Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        private MetadataReference GetAssemblyReference(IEnumerable<AssemblyName> assemblies, string name)
        {
            return MetadataReference.CreateFromFile(Assembly.Load(assemblies.First(n => n.Name == name)).Location);
        }
    }
}