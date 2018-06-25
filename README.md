Analyzers to detect possible NSubstitute usage problems, such as attempts to setup non-virtual members.

| Branch        | Build           | Coverage  |
| ------------- |-------------| -----|
| Master      | [![Build status](https://ci.appveyor.com/api/projects/status/t9lhmp61nuy3a7k5/branch/master?svg=true)](https://ci.appveyor.com/project/NSubstitute/nsubstitute-analyzers/branch/master)      |   [![Coverage Status](https://coveralls.io/repos/github/nsubstitute/NSubstitute.Analyzers/badge.svg?branch=master)](https://coveralls.io/github/nsubstitute/NSubstitute.Analyzers?branch=master) |
| Dev      | [![Build status](https://ci.appveyor.com/api/projects/status/t9lhmp61nuy3a7k5/branch/dev?svg=true)](https://ci.appveyor.com/project/NSubstitute/nsubstitute-analyzers/branch/dev)    |   [![Coverage Status](https://coveralls.io/repos/github/nsubstitute/NSubstitute.Analyzers/badge.svg?branch=dev)](https://coveralls.io/github/nsubstitute/NSubstitute.Analyzers?branch=dev) |

## Install via NuGet

* [NSubstitute.Analyzers.CSharp](https://www.nuget.org/packages/NSubstitute.Analyzers.CSharp/)
* [NSubstitute.Analyzers.VisualBasic](https://www.nuget.org/packages/NSubstitute.Analyzers.VisualBasic/)

## Motivation

[NSubstitute](https://github.com/nsubstitute/NSubstitute) was designed with the aim of having concise, friendly syntax for mocking. The downside of this syntax is that certain failure modes are hard to detect. One example is attempting to mock non-virtual members -- NSubstitute can not see these calls so can not communicate problems such as `sub.Received().NonVirtualCall()`. Thanks to the goodness of Roslyn analyzers, we now have the option to detect cases like these (as originally proposed in [NSubstitute issue #328](https://github.com/nsubstitute/NSubstitute/issues/328)).

## Diagnostics

* Non-virtual member setup detected (`NonVirtualSetupSpecification`, `NS001`)
* `Received` used without a following method call. (`UnusedReceived`, `NS002`)

## Support

Please report any problems or ask questions via the [Issue tracker](https://github.com/nsubstitute/NSubstitute.Analyzers/issues).

## Examples

### C# example
![C#](https://raw.githubusercontent.com/nsubstitute/NSubstitute.Analyzers/dev/images/csharp-example.png)

### Visual Basic example
![VisualBasic](https://raw.githubusercontent.com/nsubstitute/NSubstitute.Analyzers/dev/images/vb-example.png)
