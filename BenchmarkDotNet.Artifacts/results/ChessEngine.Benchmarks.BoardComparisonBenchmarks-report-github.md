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
| &#39;Board (2D Array)&#39; | ApplyUndo          |     32.45 ns |     20.814 ns |   1.141 ns |    1 |      - |      - |         - |
| &#39;BitBoard (Magic)&#39; | ApplyUndo          |     37.57 ns |     46.897 ns |   2.571 ns |    1 | 0.0017 |      - |      88 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | Clone              |     87.54 ns |      7.362 ns |   0.404 ns |    1 | 0.0064 |      - |     328 B |
| &#39;Board (2D Array)&#39; | Clone              |    447.98 ns |    228.341 ns |  12.516 ns |    2 | 0.0353 |      - |    1776 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;Board (2D Array)&#39; | GetPiecesForColor  |    185.77 ns |    249.425 ns |  13.672 ns |    1 | 0.0186 |      - |     936 B |
| &#39;BitBoard (Magic)&#39; | GetPiecesForColor  |    547.90 ns |    255.308 ns |  13.994 ns |    2 | 0.0639 |      - |    3240 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | IsInCheck          |     12.90 ns |     25.536 ns |   1.400 ns |    1 | 0.0005 |      - |      24 B |
| &#39;Board (2D Array)&#39; | IsInCheck          |    656.12 ns |    372.057 ns |  20.394 ns |    2 | 0.0629 |      - |    3160 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | LoadFEN            |    489.29 ns |    224.154 ns |  12.287 ns |    1 | 0.0248 |      - |    1288 B |
| &#39;Board (2D Array)&#39; | LoadFEN            |    744.51 ns |    446.224 ns |  24.459 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | MoveGen - MidGame  |    419.87 ns |    528.251 ns |  28.955 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| &#39;Board (2D Array)&#39; | MoveGen - MidGame  | 24,820.43 ns | 15,921.721 ns | 872.723 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| &#39;BitBoard (Magic)&#39; | MoveGen - Starting |    242.68 ns |     35.911 ns |   1.968 ns |    1 | 0.0439 |      - |    2224 B |
| &#39;Board (2D Array)&#39; | MoveGen - Starting | 24,859.06 ns |  8,188.870 ns | 448.860 ns |    2 | 2.4109 |      - |  122080 B |
