``` ini

BenchmarkDotNet=v0.11.5, OS=ubuntu 18.04
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.203
  [Host] : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT
  Core   : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                                 Method |      Mean |     Error |    StdDev |     Gen 0 |   Gen 1 | Gen 2 |   Allocated |
|--------------------------------------- |----------:|----------:|----------:|----------:|--------:|------:|------------:|
|                       CallInfoAnalyzer |  3.798 ms | 0.0778 ms | 0.1879 ms |  218.7500 | 11.7188 |     - |   673.15 KB |
| ConflictingArgumentAssignmentsAnalyzer |  1.201 ms | 0.0094 ms | 0.0088 ms |   21.4844 |  1.9531 |     - |    69.15 KB |
|         NonSubstitutableMemberAnalyzer |  1.944 ms | 0.0153 ms | 0.0135 ms |   35.1563 |  7.8125 |     - |   108.01 KB |
| NonSubstitutableMemberReceivedAnalyzer |  1.245 ms | 0.0093 ms | 0.0077 ms |   13.6719 |  1.9531 |     - |    47.46 KB |
|     NonSubstitutableMemberWhenAnalyzer |  1.498 ms | 0.0114 ms | 0.0101 ms |   25.3906 |  5.8594 |     - |    81.98 KB |
|                 ReEntrantSetupAnalyzer | 51.968 ms | 0.2944 ms | 0.2610 ms | 3700.0000 |       - |     - | 11375.53 KB |
|                     SubstituteAnalyzer |  3.953 ms | 0.0778 ms | 0.0896 ms |  105.4688 | 23.4375 |     - |   393.13 KB |
|                 UnusedReceivedAnalyzer |  1.139 ms | 0.0276 ms | 0.0422 ms |   13.6719 |  1.9531 |     - |     43.3 KB |
