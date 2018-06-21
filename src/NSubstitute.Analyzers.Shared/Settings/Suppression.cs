using System.Collections.Generic;
using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class Suppression
    {
        public string Target { get; set; }

        public List<string> Rules { get; set; }
    }
}