using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public class AnalyzerAdditionalText : AdditionalText
{
    private readonly string _fileName;
    private readonly string _text;

    public AnalyzerAdditionalText(string fileName, string text)
    {
        _fileName = fileName;
        _text = text;
    }

    public override SourceText GetText(CancellationToken cancellationToken = new CancellationToken())
    {
        return SourceText.From(_text);
    }

    public override string Path => _fileName;
}