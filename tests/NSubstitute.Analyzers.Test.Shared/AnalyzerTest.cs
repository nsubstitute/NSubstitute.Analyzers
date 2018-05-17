using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Xunit;
#if VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic;

#endif

namespace NSubstitute.Analyzers.Test
{
    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// All methods are static.
    /// </summary>
    public abstract class AnalyzerTest
    {
        private static readonly MetadataReference CorlibReference =
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly MetadataReference SystemCoreReference =
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
#if CSHARP
        private static readonly MetadataReference CSharpSymbolsReference =
 MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
#elif VISUAL_BASIC
        private static readonly MetadataReference VisualBasicSymbolsReference =
            MetadataReference.CreateFromFile(typeof(VisualBasicCompilation).Assembly.Location);
#endif
        private static readonly MetadataReference CodeAnalysisReference =
            MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        private static readonly MetadataReference NSubstituteReference =
            MetadataReference.CreateFromFile(typeof(NSubstitute.Substitute).Assembly.Location);

        private static readonly MetadataReference ValueTaskReference =
            MetadataReference.CreateFromFile(typeof(ValueTask<>).Assembly.Location);

        public static string DefaultFilePathPrefix { get; } = "Test";

        public static string CSharpDefaultFileExt { get; } = "cs";

        public static string VisualBasicDefaultExt { get; } = "vb";

        public static string TestProjectName { get; } = "TestProject";

        protected AnalyzerTest()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture =
                CultureInfo.GetCultureInfoByIetfLanguageTag("en-US");
        }

        #if CSHARP
        /// <summary>
        /// Called to test a C# DiagnosticAnalyzer when applied on the single inputted string as a source.
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source.</param>
        public async Task VerifyCSharpDiagnostic(string source, params DiagnosticResult[] expected)
        {
            await this.VerifyVisualBasicDiagnostic(new[] { source }, LanguageNames.CSharp, this.GetCSharpDiagnosticAnalyzer(), expected, false);
        }

        /// <summary>
        /// Get the CSharp analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>The diagnostic analyzer being tested.</returns>
        protected abstract DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer();
#elif VISUAL_BASIC
        /// <summary>
        /// Called to test a VB.NET DiagnosticAnalyzer when applied on the single inputted string as a source.
        /// Note: input a DiagnosticResult for each Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"> DiagnosticResults that should appear after the analyzer is run on the source.</param>
        public async Task VerifyVisualBasicDiagnostic(string source, params DiagnosticResult[] expected)
        {
            await this.VerifyVisualBasicDiagnostic(new[] { source }, LanguageNames.VisualBasic, this.GetVisualBasicDiagnosticAnalyzer(), expected, false);
        }

        /// <summary>
        /// Get the Visual Basic analyzer being tested - to be implemented in non-abstract class.
        /// </summary>
        /// <returns>The diagnostic analyzer being tested.</returns>
        protected abstract DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer();
#endif

        /// <summary>
        /// Checks each of the actual Diagnostics found and compares them with the corresponding DiagnosticResult in the array of expected results.
        /// Diagnostics are considered equal only if the DiagnosticResultLocation, Id, Severity, and Message of the DiagnosticResult match the actual diagnostic.
        /// </summary>
        /// <param name="actualResults">The Diagnostics found by the compiler after running the analyzer on the source code.</param>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="expectedResults">Diagnostic Results that should have appeared in the code.</param>
        private static void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            int expectedCount = expectedResults.Count();
            int actualCount = actualResults.Count();

            if (expectedCount != actualCount)
            {
                string diagnosticsOutput = actualResults.Any() ? FormatDiagnostics(analyzer, actualResults.ToArray()) : "    NONE.";

                var message =
                    $@"Mismatch between number of diagnostics returned, expected ""{expectedCount}"" actual ""{actualCount}""

Diagnostics:
{diagnosticsOutput}
";
                Execute.Assertion.FailWith(message);
            }

            for (int i = 0; i < expectedResults.Length; i++)
            {
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults[i];

                if (expected.Line == -1 && expected.Column == -1)
                {
                    if (actual.Location != Location.None)
                    {
                        var message =
                            $@"Expected:
A project diagnostic with No location
Actual:
{FormatDiagnostics(analyzer, actual)}";
                        Execute.Assertion.FailWith(message);
                    }
                }
                else
                {
                    VerifyDiagnosticLocation(analyzer, actual, actual.Location, expected.Locations.First());
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    if (additionalLocations.Length != expected.Locations.Length - 1)
                    {
                        var message =
                            $@"Expected {expected.Locations.Length - 1} additional locations but got {additionalLocations.Length} for Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                        Execute.Assertion.FailWith(message);
                    }

                    for (int j = 0; j < additionalLocations.Length; ++j)
                    {
                        VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j], expected.Locations[j + 1]);
                    }
                }

                if (actual.Id != expected.Id)
                {
                    var message =
                        $@"Expected diagnostic id to be ""{expected.Id}"" was ""{actual.Id}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }

                if (actual.Severity != expected.Severity)
                {
                    var message =
                            $@"Expected diagnostic severity to be ""{expected.Severity}"" was ""{actual.Severity}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }

                if (actual.GetMessage() != expected.Message)
                {
                    var message =
                            $@"Expected diagnostic message to be ""{expected.Message}"" was ""{actual.GetMessage()}""

Diagnostic:
    {FormatDiagnostics(analyzer, actual)}
";
                    Execute.Assertion.FailWith(message);
                }
            }
        }

        /// <summary>
        /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and compares it with the location in the expected DiagnosticResult.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The Location of the Diagnostic found in the code.</param>
        /// <param name="expected">The DiagnosticResultLocation that should have been found.</param>
        private static void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic, Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            string message;
            var actualLinePosition = actualSpan.StartLinePosition;

            // Only check line position if there is an actual line in the real diagnostic
            if (actualLinePosition.Line > 0)
            {
                if (actualLinePosition.Line + 1 != expected.Line)
                {
                    message =
                            $@"Expected diagnostic to be on line ""{expected.Line}"" was actually on line ""{actualLinePosition.Line + 1}""

Diagnostic:
    {FormatDiagnostics(analyzer, diagnostic)}
";
                    Execute.Assertion.FailWith(message);
                }
            }

            // Only check column position if there is an actual column position in the real diagnostic
            if (actualLinePosition.Character > 0)
            {
                if (actualLinePosition.Character + 1 != expected.Column)
                {
                    message =
                            $@"Expected diagnostic to start at column ""{expected.Column}"" was actually at column ""{actualLinePosition.Character + 1}""

Diagnostic:
    {FormatDiagnostics(analyzer, diagnostic)}
";
                    Execute.Assertion.FailWith(message);
                }
            }
        }

        /// <summary>
        /// Helper method to format a Diagnostic into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The analyzer that this verifier tests.</param>
        /// <param name="diagnostics">The Diagnostics to be formatted.</param>
        /// <returns>The Diagnostics formatted as a string.</returns>
        private static string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < diagnostics.Length; ++i)
            {
                builder.AppendLine("// " + diagnostics[i]);

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule != null && rule.Id == diagnostics[i].Id)
                    {
                        var location = diagnostics[i].Location;
                        if (location == Location.None)
                        {
                            builder.Append($"GetGlobalResult({analyzerType.Name}.{rule.Id})");
                        }
                        else
                        {
                            location.IsInSource.Should()
                                .BeTrue(
                                    $"Test base does not currently handle diagnostics in metadata locations. Diagnostic in metadata: {diagnostics[i]}\r\n");

                            string resultMethodName = diagnostics[i].Location.SourceTree.FilePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ? "GetCSharpResultAt" : "GetBasicResultAt";
                            var linePosition = diagnostics[i].Location.GetLineSpan().StartLinePosition;

                            builder.Append(
                                    $"{resultMethodName}({linePosition.Line + 1}, {linePosition.Character + 1}, {analyzerType.Name}.{rule.Id})");
                        }

                        if (i != diagnostics.Length - 1)
                        {
                            builder.Append(',');
                        }

                        builder.AppendLine();
                        break;
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after the analyzer is run,
        /// then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzer">The analyzer to be run on the source code.</param>
        /// <param name="expected">DiagnosticResults that should appear after the analyzer is run on the sources.</param>
        /// <param name="allowCompilationErrors">Allow compiler errors.</param>
        private async Task VerifyVisualBasicDiagnostic(string[] sources, string language, DiagnosticAnalyzer analyzer, DiagnosticResult[] expected, bool allowCompilationErrors)
        {
            var diagnostics = await GetSortedDiagnostics(sources, language, analyzer, allowCompilationErrors);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents.</param>
        /// <param name="documents">The Documents that the analyzer will be run on.</param>
        /// <param name="allowCompilationErrors">Allow compiler errors.</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        protected static async Task<Diagnostic[]> GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer,
            Document[] documents, bool allowCompilationErrors)
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
            var analyzerExceptions = new List<Exception>();
            foreach (var project in projects)
            {
                var options = new CompilationWithAnalyzersOptions(
                    new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty),
                    (exception, diagnosticAnalyzer, diagnostic) => analyzerExceptions.Add(exception),
                    false,
                    true);
                var compilationWithAnalyzers = (await project.GetCompilationAsync())
                    .WithAnalyzers(ImmutableArray.Create(analyzer), options);

                if (!allowCompilationErrors)
                {
                    AssertThatCompilationSucceeded(compilationWithAnalyzers);
                }

                var diags = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

                if (analyzerExceptions.Any())
                {
                    if (analyzerExceptions.Count == 1)
                    {
                        ExceptionDispatchInfo.Capture(analyzerExceptions[0]).Throw();
                    }
                    else
                    {
                        throw new AggregateException("Multiple exceptions thrown during analysis", analyzerExceptions);
                    }
                }

                foreach (var diag in diags)
                {
                    if (diag.Location == Location.None || diag.Location.IsInMetadata)
                    {
                        diagnostics.Add(diag);
                    }
                    else
                    {
                        for (int i = 0; i < documents.Length; i++)
                        {
                            var document = documents[i];
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

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Document created from the source string.</returns>
        protected static Document CreateDocument(string source, string language)
        {
            return CreateProject(new[] {source}, language).Documents.First();
        }

        /// <summary>
        /// Given an array of strings as sources and a language, turn them into a project and return the documents and spans of it.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Tuple containing the Documents produced from the sources and their TextSpans if relevant.</returns>
        private static Document[] GetDocuments(string[] sources, string language)
        {
            if (language != LanguageNames.CSharp && language != LanguageNames.VisualBasic)
            {
                throw new ArgumentException("Unsupported Language");
            }

            var project = CreateProject(sources, language);
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

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source code is in.</param>
        /// <returns>A Project created out of the Documents created from the source strings.</returns>
#if CSHARP
        private static Project CreateProject(string[] sources, string language = LanguageNames.CSharp)
#elif VISUAL_BASIC
        private static Project CreateProject(string[] sources, string language = LanguageNames.VisualBasic)
#endif
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;
            var referencedAssemblies = typeof(Substitute).Assembly.GetReferencedAssemblies();
            var systemRuntimeReference = GetAssemblyReference(referencedAssemblies, "System.Runtime");
            var systemThreadingTasksReference = GetAssemblyReference(referencedAssemblies, "System.Threading.Tasks");

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            using (var adhocWorkspace = new AdhocWorkspace())
            {
                var solution = adhocWorkspace
                    .CurrentSolution
                    .AddProject(projectId, TestProjectName, TestProjectName, language)
#if CSHARP
                    .WithProjectCompilationOptions(
                        projectId,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
#elif VISUAL_BASIC
                    .WithProjectCompilationOptions(
                        projectId,
                        new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                            optionStrict: OptionStrict.On))
#endif
                    .AddMetadataReference(projectId, CorlibReference)
                    .AddMetadataReference(projectId, SystemCoreReference)
#if CSHARP
                    .AddMetadataReference(projectId, CSharpSymbolsReference)
#elif VISUAL_BASIC
                    .AddMetadataReference(projectId, VisualBasicSymbolsReference)
#endif
                    .AddMetadataReference(projectId, CodeAnalysisReference)
                    .AddMetadataReference(projectId, NSubstituteReference)
                    .AddMetadataReference(projectId, ValueTaskReference)
                    .AddMetadataReference(projectId, systemRuntimeReference)
                    .AddMetadataReference(projectId, systemThreadingTasksReference);



                int count = 0;
                foreach (var source in sources)
                {
                    var newFileName = fileNamePrefix + count + "." + fileExt;
                    var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                    solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                    count++;
                }

                return solution.GetProject(projectId);
            }
        }

        /// <summary>
        /// Given classes in the form of strings, their language, and an IDiagnosticAnalyzer to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings.</param>
        /// <param name="language">The language the source classes are in.</param>
        /// <param name="analyzer">The analyzer to be run on the sources.</param>
        /// <param name="allowCompilationErrors">Allow compiler errors.</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location.</returns>
        private static async Task<Diagnostic[]> GetSortedDiagnostics(string[] sources, string language, DiagnosticAnalyzer analyzer,
            bool allowCompilationErrors)
        {
            return await GetSortedDiagnosticsFromDocuments(analyzer, GetDocuments(sources, language), allowCompilationErrors);
        }

        /// <summary>
        /// Sort diagnostics by location in source document.
        /// </summary>
        /// <param name="diagnostics">The list of Diagnostics to be sorted.</param>
        /// <returns>An IEnumerable containing the Diagnostics in order of Location.</returns>
        private static Diagnostic[] SortDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.OrderBy(d => d.Location.SourceSpan.Start).ToArray();
        }

        private static MetadataReference GetAssemblyReference(IEnumerable<AssemblyName> assemblies, string name)
        {
            return MetadataReference.CreateFromFile(Assembly.Load(assemblies.First(n => n.Name == name)).Location);
        }
    }
}