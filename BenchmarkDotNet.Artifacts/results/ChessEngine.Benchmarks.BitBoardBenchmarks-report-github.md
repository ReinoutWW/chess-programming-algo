```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.7171)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                                        | Mean      | Error      | StdDev    | Rank | Gen0   | Gen1   | Allocated |
|---------------------------------------------- |----------:|-----------:|----------:|-----:|-------:|-------:|----------:|
| &#39;ApplyMove + UndoMove&#39;                        |  27.01 ns |   0.586 ns |  0.032 ns |    1 |      - |      - |         - |
| &#39;ApplyMove only&#39;                              |  40.02 ns |   2.113 ns |  0.116 ns |    2 |      - |      - |         - |
| &#39;Clone Board&#39;                                 |  84.48 ns |  12.431 ns |  0.681 ns |    3 | 0.0064 |      - |     328 B |
| &#39;GenerateMoves - Starting Position (White)&#39;   | 245.32 ns | 120.401 ns |  6.600 ns |    4 | 0.0439 |      - |    2224 B |
| &#39;GenerateMoves - Starting Position (Black)&#39;   | 258.58 ns | 662.117 ns | 36.293 ns |    4 | 0.0439 |      - |    2224 B |
| &#39;GenerateMoves - MidGame Position&#39;            | 393.41 ns | 132.803 ns |  7.279 ns |    5 | 0.0677 | 0.0005 |    3416 B |
| &#39;Load FEN Position&#39;                           | 439.95 ns |  39.373 ns |  2.158 ns |    5 | 0.0253 |      - |    1288 B |
| GetPiecesForColor                             | 487.66 ns |  84.113 ns |  4.611 ns |    5 | 0.0639 |      - |    3240 B |
| &#39;GenerateMoves - Complex Position (Kiwipete)&#39; | 548.64 ns |  73.886 ns |  4.050 ns |    5 | 0.0944 | 0.0010 |    4784 B |
| &#39;GetPieces (8x8 array)&#39;                       | 562.01 ns |  38.097 ns |  2.088 ns |    5 | 0.0639 |      - |    3240 B |
