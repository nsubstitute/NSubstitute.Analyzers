using System;

namespace NSubstitute.Analyzers.Shared;

[AttributeUsage(AttributeTargets.Field)]
internal class DisplayNameAttribute : Attribute
{
    public string Name { get; }

    public DisplayNameAttribute(string name)
    {
        Name = name;
    }
}