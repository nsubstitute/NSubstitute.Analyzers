### 1.0.8 (08 April 2019)

- [#89](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/89) NS1002 doesn't detect non-virtual calls from base class
- [#87](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/87) NS1002 highlights wrong member
- [#78](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/78) Update documentation so as it includes information about min supported Visual Studio version
- [#76](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/76) Analyze callInfo usages for Do method
- [#62](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/62) Detecting conflicting out/ref arguments

### 1.0.7 (15 March 2019)

 - [#79](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/79) - NS2003 triggers a warning when InternalsVisibleTo using strong name
 - [#73](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/73) - NS1001 highlights wrong member
 - [#70](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/70) - Implement code fix providers for substituting for internal member
 - [#66](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/66) - Detect substituting for `internal` members without `InternalsVisibleTo`

### 1.0.6 (21 February 2019)

 - [#71](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/71) - Incorrect NS4000 when using typeof expression

### 1.0.5 (09 February 2019)

 - [#61](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/61) - CallInfo analysis for AndDoes method

### 1.0.4 (03 January 2019)

 - [#57](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/57) - Incorrect NS3004 for derived types
 - [#56](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/56) - Analyzer 'NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers.ReEntrantSetupAnalyzer' threw an exception of type 'System.ArgumentException' with message 'SyntaxTree is not part of the compilation'

### 1.0.3 (22 November 2018)

 - [#54](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/54) - Rule NS2003 does not detect InternalsVisibleTo correctly

### 1.0.2 (07 November 2018)

 - [#52](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/52) - Detect substitution for non-virtual members for ReturnsNull like methods

### 1.0.1 (29 October 2018)

 - [#50](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/50) - Enforce release notes update during package publish
 - [#47](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/47) - Over-enthusiastic NS4000 error (Returns within Returns)
 - [#46](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/46) - ArgumentException on ReEntrantSetupAnalyzer on first load

### 1.0.0 (23 October 2018)

 - [#42](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/42) - Add basic documentation

### 0.1.0-beta6 (07 October 2018)

 - [#40](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/40) - Group diagnostics into categories

### 0.1.0-beta5 (30 September 2018)

 - [#38](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/38) - Add code fix provider for exposing internal types to proxy generator

### 0.1.0-beta4 (23 September 2018)

 - [#36](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/36) - ReEntrantSetupAnalyzer - syntax node is not within syntax tree

### 0.1.0-beta3 (07 September 2018)

 - [#31](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/31) - Update nuget package with proper NSubstitute.Analyzers icon
 - [#30](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/30) - Detect wrong usages of callInfo argument access

### 0.1.0-beta2 (01 August 2018)

 - [#28](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/28) - Detect non-virtual setup for Throws and ThrowsForAnyArgs
 - [#26](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/26) - Checking constructor arguments passed via `SubstitutionContext.Current.SubstituteFactory.Create`/`SubstitutionContext.Current.SubstituteFactory.CreatePartial`
 - [#22](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/22) - Unify resource files
 - [#18](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/18) - Detect non-virtual When...Do calls
 - [#16](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/16) - Detect non-virtual received calls
 - [#12](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/12) - Detect re-entrant substitute calls
 - [#11](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/11) - Allow configuration of safe API misuses
 - [#1](https://github.com/nsubstitute/NSubstitute.Analyzers/issues/1) - Checking constructor arguments passed via `Substitute.For`/`Substitute.ForPartsOf`

### 0.1.0-beta1 (01 August 2018)
 - Initial release

