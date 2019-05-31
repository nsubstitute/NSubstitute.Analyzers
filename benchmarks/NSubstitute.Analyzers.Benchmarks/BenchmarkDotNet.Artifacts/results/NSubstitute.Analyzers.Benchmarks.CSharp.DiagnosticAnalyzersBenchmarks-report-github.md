``` ini

BenchmarkDotNet=v0.11.5, OS=ubuntu 18.04
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.203
  [Host] : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT
  Core   : .NET Core 2.1.10 (CoreCLR 4.6.27514.02, CoreFX 4.6.27514.02), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                                 Method |        Mean |        Error |       StdDev |      Median |     Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|--------------------------------------- |------------:|-------------:|-------------:|------------:|----------:|--------:|------:|-----------:|
|                       CallInfoAnalyzer |  2,178.3 us |    14.514 us |    12.120 us |  2,178.0 us |  167.9688 | 11.7188 |     - |  524.01 KB |
| ConflictingArgumentAssignmentsAnalyzer |    597.9 us |    12.716 us |    25.101 us |    592.9 us |   34.1797 |  1.9531 |     - |  107.03 KB |
|         NonSubstitutableMemberAnalyzer |  2,617.1 us |    38.423 us |    35.941 us |  2,611.3 us |  179.6875 | 15.6250 |     - |  561.46 KB |
| NonSubstitutableMemberReceivedAnalyzer |    673.6 us |     3.574 us |     3.168 us |    673.5 us |   31.2500 |  6.8359 |     - |    97.4 KB |
|     NonSubstitutableMemberWhenAnalyzer |  1,098.3 us |     6.122 us |     5.112 us |  1,100.7 us |   37.1094 |  7.8125 |     - |   118.8 KB |
|                 ReEntrantSetupAnalyzer | 59,015.5 us | 1,434.491 us | 1,707.659 us | 58,449.1 us | 2333.3333 |       - |     - | 7443.79 KB |
|                     SubstituteAnalyzer |  2,291.5 us |    45.674 us |   115.424 us |  2,278.2 us |   89.8438 | 19.5313 |     - |   344.8 KB |
|                 UnusedReceivedAnalyzer |    624.7 us |    18.581 us |    54.494 us |    599.2 us |   30.2734 |  1.9531 |     - |   93.57 KB |
