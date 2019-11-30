# Configuring NSubstitute Analyzers

NSubstitute Analyzers normally does not require additional configuration. It is sufficient to reference the package in your test project. If you would like to customise how the NSubstitute Analyzers rules are applied to your project you can use one of the following configuration mechanisms:

1. Code analysis rule set files

   * Enable and disable individual rules
   * Configure the severity of violations reported by individual rules

2. `nsubstitute.json`

   * Configure suppressions of certain rules for selected members in order to allow NSubstitute API misuse

## Code analysis rule set files

Code analysis rule sets are the standard way to configure most diagnostic analyzers within Visual Studio. Information about creating and customizing these files can be found in the [Using Rule Sets to Group Code Analysis Rules](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) documentation on docs.microsoft.com.

## Getting Started with `nsubstitute.json`

The easiest way to add `nsubstitute.json` into your project is to add following entries in csproj file:

````xml
  <ItemGroup>
    <None Remove="nsubstitute.json" />
  </ItemGroup>

  <ItemGroup>
    <AdditionalFiles Include="nsubstitute.json" />
  </ItemGroup>
````

It is also possible to add file manually in Visual Studio, you just have to remember to set `Build Action` to `C# analyzer additional file`.

### JSON Schema for IntelliSense

A JSON schema is available for `nsubstitute.json`. By including a reference in `nsubstitute.json` to this schema, Visual Studio will offer IntelliSense functionality (code completion, quick info, etc.) while editing this file. The schema may be configured by adding the following top-level property in `nsubstitute.json`:

```json
{
  "$schema": "https://raw.githubusercontent.com/nsubstitute/NSubstitute.Analyzers/master/src/NSubstitute.Analyzers.Shared/Settings/nsubstitute.schema.json"
}
```

### Safe API misuse

This section describes the suppression mechanism for NSubstitute safe API misuse, which can be configured in `nsubstitute.json`. Each of the described
properties are configured in the `suppressions` object, which is shown in the following sample file.

```json
{
  "Suppressions": [
    {
      "Target": "M:MyNamespace.Foo.Bar~System.Int32",
      "Rules": [
        "NS1000"
      ]
    }
  ]
}
```

#### Target

Defines target id for which given suppression is applicable. Target might be a namespace, class or member. Format of an id follows the rules defined on  [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/processing-the-xml-file#examples)

#### Rules

An array of rules to suppress.

> :warning: As for today the only rules which can be safely suppressed is `NS1000` and `NS1004`. See more on [GitHub](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/11)
