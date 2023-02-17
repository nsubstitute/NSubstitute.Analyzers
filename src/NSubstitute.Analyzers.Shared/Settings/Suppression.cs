using System.Collections.Generic;

namespace NSubstitute.Analyzers.Shared.Settings;

internal class Suppression
{
    public Suppression()
    {
        Target = string.Empty;
        Rules = new List<string>();
    }

    public string Target { get; set; }

    public List<string> Rules { get; set; }
}