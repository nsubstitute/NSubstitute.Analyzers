using System.Collections.Generic;
using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class Suppression
    {
        public Suppression()
        {
        }

        public Suppression(string target, List<string> rules)
        {
            Target = target;
            Rules = rules;
        }

        public string Target { get; set; }

        public List<string> Rules { get; set; }
    }
}