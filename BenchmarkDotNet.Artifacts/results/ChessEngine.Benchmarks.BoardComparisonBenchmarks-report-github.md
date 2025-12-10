```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.7171)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method             | Categories         | Mean         | Error         | StdDev     | Rank | Gen0   | Gen1   | Allocated |
|------------------- |------------------- |-------------:|--------------:|-----------:|-----:|-------:|-------:|----------:|
| &#39;Board (2D Array)&#39; | ApplyUndo          |     29.91 ns |      4.607 ns |   0.253 ns |    1 |      - |      - |         - |
| &#39;BitBoard (Magic)&#39; | ApplyUndo          |     34.46 ns |      6.028 ns |   0.330 ns |    1 | 0.0017 |      - |      88 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | Clone              |     88.08 ns |      7.391 ns |   0.405 ns |    1 | 0.0064 |      - |     328 B |
| &#39;Board (2D Array)&#39; | Clone              |    405.30 ns |    147.016 ns |   8.058 ns |    2 | 0.0353 |      - |    1776 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;Board (2D Array)&#39; | GetPiecesForColor  |    179.07 ns |    152.008 ns |   8.332 ns |    1 | 0.0186 |      - |     936 B |
| &#39;BitBoard (Magic)&#39; | GetPiecesForColor  |    501.03 ns |     67.923 ns |   3.723 ns |    2 | 0.0639 |      - |    3240 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | IsInCheck          |     10.24 ns |      0.692 ns |   0.038 ns |    1 | 0.0005 |      - |      24 B |
| &#39;Board (2D Array)&#39; | IsInCheck          |    583.38 ns |    137.848 ns |   7.556 ns |    2 | 0.0629 |      - |    3160 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | LoadFEN            |    430.36 ns |    102.683 ns |   5.628 ns |    1 | 0.0253 |      - |    1288 B |
| &#39;Board (2D Array)&#39; | LoadFEN            |    661.34 ns |    103.354 ns |   5.665 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| &#39;Board (2D Array)&#39; | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| &#39;Board (2D Array)&#39; | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |
