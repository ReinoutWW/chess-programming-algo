<img width="1638" height="1256" alt="image" src="https://github.com/user-attachments/assets/f6b9c1f9-8ca5-4eb4-8f28-b2379f399d4a" />


// Benchmark Process 42460 has exited with code 0.

Mean = 266.964 ns, StdErr = 8.889 ns (3.33%), N = 3, StdDev = 15.396 ns
Min = 253.819 ns, Q1 = 258.495 ns, Median = 263.170 ns, Q3 = 273.536 ns, Max = 283.902 ns
IQR = 15.042 ns, LowerFence = 235.932 ns, UpperFence = 296.099 ns
ConfidenceInterval = [-13.922 ns; 547.850 ns] (CI 99.9%), Margin = 280.886 ns (105.21% of Mean)
Skewness = 0.23, Kurtosis = 0.67, MValue = 2

// ** Remained 0 (0,0%) benchmark(s) to run. Estimated finish 2025-12-10 13:30 (0h 0m from now) **
Successfully reverted power plan (GUID: 8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c FriendlyName: High performance)
// ***** BenchmarkRunner: Finish  *****

// * Export *
  BenchmarkDotNet.Artifacts\results\ChessEngine.Benchmarks.BoardComparisonBenchmarks-report.csv
  BenchmarkDotNet.Artifacts\results\ChessEngine.Benchmarks.BoardComparisonBenchmarks-report-github.md
  BenchmarkDotNet.Artifacts\results\ChessEngine.Benchmarks.BoardComparisonBenchmarks-report.html

// * Detailed results *
BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 29.914 ns, StdErr = 0.146 ns (0.49%), N = 3, StdDev = 0.253 ns
Min = 29.681 ns, Q1 = 29.780 ns, Median = 29.879 ns, Q3 = 30.031 ns, Max = 30.182 ns
IQR = 0.251 ns, LowerFence = 29.404 ns, UpperFence = 30.407 ns
ConfidenceInterval = [25.307 ns; 34.521 ns] (CI 99.9%), Margin = 4.607 ns (15.40% of Mean)
Skewness = 0.14, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[29.451 ns ; 30.412 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 34.461 ns, StdErr = 0.191 ns (0.55%), N = 3, StdDev = 0.330 ns
Min = 34.201 ns, Q1 = 34.275 ns, Median = 34.348 ns, Q3 = 34.590 ns, Max = 34.833 ns
IQR = 0.316 ns, LowerFence = 33.801 ns, UpperFence = 35.064 ns
ConfidenceInterval = [28.432 ns; 40.489 ns] (CI 99.9%), Margin = 6.028 ns (17.49% of Mean)
Skewness = 0.3, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[33.901 ns ; 35.134 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 88.084 ns, StdErr = 0.234 ns (0.27%), N = 3, StdDev = 0.405 ns
Min = 87.624 ns, Q1 = 87.931 ns, Median = 88.238 ns, Q3 = 88.313 ns, Max = 88.389 ns
IQR = 0.382 ns, LowerFence = 87.357 ns, UpperFence = 88.887 ns
ConfidenceInterval = [80.692 ns; 95.475 ns] (CI 99.9%), Margin = 7.391 ns (8.39% of Mean)
Skewness = -0.33, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[87.255 ns ; 88.757 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 405.295 ns, StdErr = 4.653 ns (1.15%), N = 3, StdDev = 8.058 ns
Min = 396.810 ns, Q1 = 401.521 ns, Median = 406.232 ns, Q3 = 409.538 ns, Max = 412.845 ns
IQR = 8.018 ns, LowerFence = 389.495 ns, UpperFence = 421.565 ns
ConfidenceInterval = [258.279 ns; 552.312 ns] (CI 99.9%), Margin = 147.016 ns (36.27% of Mean)
Skewness = -0.11, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[389.476 ns ; 402.205 ns) | @
[402.205 ns ; 416.872 ns) | @@
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 179.069 ns, StdErr = 4.811 ns (2.69%), N = 3, StdDev = 8.332 ns
Min = 172.739 ns, Q1 = 174.350 ns, Median = 175.960 ns, Q3 = 182.235 ns, Max = 188.509 ns
IQR = 7.885 ns, LowerFence = 162.522 ns, UpperFence = 194.062 ns
ConfidenceInterval = [27.061 ns; 331.077 ns] (CI 99.9%), Margin = 152.008 ns (84.89% of Mean)
Skewness = 0.32, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[166.767 ns ; 181.932 ns) | @@
[181.932 ns ; 196.091 ns) | @
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 501.033 ns, StdErr = 2.150 ns (0.43%), N = 3, StdDev = 3.723 ns
Min = 497.070 ns, Q1 = 499.320 ns, Median = 501.570 ns, Q3 = 503.014 ns, Max = 504.458 ns
IQR = 3.694 ns, LowerFence = 493.779 ns, UpperFence = 508.555 ns
ConfidenceInterval = [433.110 ns; 568.956 ns] (CI 99.9%), Margin = 67.923 ns (13.56% of Mean)
Skewness = -0.14, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[493.682 ns ; 507.846 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 10.243 ns, StdErr = 0.022 ns (0.21%), N = 3, StdDev = 0.038 ns
Min = 10.219 ns, Q1 = 10.221 ns, Median = 10.222 ns, Q3 = 10.254 ns, Max = 10.286 ns
IQR = 0.034 ns, LowerFence = 10.170 ns, UpperFence = 10.305 ns
ConfidenceInterval = [9.550 ns; 10.935 ns] (CI 99.9%), Margin = 0.692 ns (6.76% of Mean)
Skewness = 0.38, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[10.218 ns ; 10.287 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 583.376 ns, StdErr = 4.362 ns (0.75%), N = 3, StdDev = 7.556 ns
Min = 575.051 ns, Q1 = 580.163 ns, Median = 585.276 ns, Q3 = 587.538 ns, Max = 589.800 ns
IQR = 7.375 ns, LowerFence = 569.101 ns, UpperFence = 598.600 ns
ConfidenceInterval = [445.527 ns; 721.224 ns] (CI 99.9%), Margin = 137.848 ns (23.63% of Mean)
Skewness = -0.24, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[568.175 ns ; 580.662 ns) | @
[580.662 ns ; 596.676 ns) | @@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 430.365 ns, StdErr = 3.250 ns (0.76%), N = 3, StdDev = 5.628 ns
Min = 425.907 ns, Q1 = 427.202 ns, Median = 428.498 ns, Q3 = 432.594 ns, Max = 436.689 ns
IQR = 5.391 ns, LowerFence = 419.115 ns, UpperFence = 440.681 ns
ConfidenceInterval = [327.681 ns; 533.048 ns] (CI 99.9%), Margin = 102.683 ns (23.86% of Mean)
Skewness = 0.3, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[422.080 ns ; 432.325 ns) | @@
[432.325 ns ; 441.811 ns) | @
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 661.344 ns, StdErr = 3.271 ns (0.49%), N = 3, StdDev = 5.665 ns
Min = 657.641 ns, Q1 = 658.084 ns, Median = 658.526 ns, Q3 = 663.196 ns, Max = 667.866 ns
IQR = 5.112 ns, LowerFence = 650.415 ns, UpperFence = 670.864 ns
ConfidenceInterval = [557.991 ns; 764.698 ns] (CI 99.9%), Margin = 103.354 ns (15.63% of Mean)
Skewness = 0.37, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[657.598 ns ; 667.909 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 438.842 ns, StdErr = 3.196 ns (0.73%), N = 3, StdDev = 5.536 ns
Min = 433.201 ns, Q1 = 436.129 ns, Median = 439.058 ns, Q3 = 441.662 ns, Max = 444.267 ns
IQR = 5.533 ns, LowerFence = 427.830 ns, UpperFence = 449.961 ns
ConfidenceInterval = [337.845 ns; 539.838 ns] (CI 99.9%), Margin = 100.996 ns (23.01% of Mean)
Skewness = -0.04, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[428.163 ns ; 446.700 ns) | @@@
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 24.419 us, StdErr = 0.178 us (0.73%), N = 3, StdDev = 0.308 us
Min = 24.089 us, Q1 = 24.280 us, Median = 24.470 us, Q3 = 24.584 us, Max = 24.698 us
IQR = 0.304 us, LowerFence = 23.823 us, UpperFence = 25.041 us
ConfidenceInterval = [18.808 us; 30.030 us] (CI 99.9%), Margin = 5.611 us (22.98% of Mean)
Skewness = -0.16, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[23.809 us ; 24.304 us) | @
[24.304 us ; 24.864 us) | @@
---------------------------------------------------

BoardComparisonBenchmarks.'BitBoard (Magic)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 266.964 ns, StdErr = 8.889 ns (3.33%), N = 3, StdDev = 15.396 ns
Min = 253.819 ns, Q1 = 258.495 ns, Median = 263.170 ns, Q3 = 273.536 ns, Max = 283.902 ns
IQR = 15.042 ns, LowerFence = 235.932 ns, UpperFence = 296.099 ns
ConfidenceInterval = [-13.922 ns; 547.850 ns] (CI 99.9%), Margin = 280.886 ns (105.21% of Mean)
Skewness = 0.23, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[244.484 ns ; 272.506 ns) | @@
[272.506 ns ; 297.914 ns) | @
---------------------------------------------------

BoardComparisonBenchmarks.'Board (2D Array)': ShortRun(IterationCount=3, LaunchCount=1, WarmupCount=3)
Runtime = .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI; GC = Concurrent Workstation
Mean = 26.065 us, StdErr = 0.356 us (1.37%), N = 3, StdDev = 0.617 us
Min = 25.685 us, Q1 = 25.709 us, Median = 25.733 us, Q3 = 26.255 us, Max = 26.776 us
IQR = 0.546 us, LowerFence = 24.890 us, UpperFence = 27.073 us
ConfidenceInterval = [14.815 us; 37.314 us] (CI 99.9%), Margin = 11.250 us (43.16% of Mean)
Skewness = 0.38, Kurtosis = 0.67, MValue = 2
-------------------- Histogram --------------------
[25.669 us ; 26.791 us) | @@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.7171)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

| Method             | Categories         | Mean         | Error         | StdDev     | Rank | Gen0   | Gen1   | Allocated |
|------------------- |------------------- |-------------:|--------------:|-----------:|-----:|-------:|-------:|----------:|
| 'Board (2D Array)' | ApplyUndo          |     29.91 ns |      4.607 ns |   0.253 ns |    1 |      - |      - |         - |
| 'BitBoard (Magic)' | ApplyUndo          |     34.46 ns |      6.028 ns |   0.330 ns |    1 | 0.0017 |      - |      88 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | Clone              |     88.08 ns |      7.391 ns |   0.405 ns |    1 | 0.0064 |      - |     328 B |
| 'Board (2D Array)' | Clone              |    405.30 ns |    147.016 ns |   8.058 ns |    2 | 0.0353 |      - |    1776 B |
|                    |                    |              |               |            |      |        |        |           |
| 'Board (2D Array)' | GetPiecesForColor  |    179.07 ns |    152.008 ns |   8.332 ns |    1 | 0.0186 |      - |     936 B |
| 'BitBoard (Magic)' | GetPiecesForColor  |    501.03 ns |     67.923 ns |   3.723 ns |    2 | 0.0639 |      - |    3240 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | IsInCheck          |     10.24 ns |      0.692 ns |   0.038 ns |    1 | 0.0005 |      - |      24 B |
| 'Board (2D Array)' | IsInCheck          |    583.38 ns |    137.848 ns |   7.556 ns |    2 | 0.0629 |      - |    3160 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | LoadFEN            |    430.36 ns |    102.683 ns |   5.628 ns |    1 | 0.0253 |      - |    1288 B |
| 'Board (2D Array)' | LoadFEN            |    661.34 ns |    103.354 ns |   5.665 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| 'Board (2D Array)' | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| 'Board (2D Array)' | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

// * Legends *
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | IsInCheck          |     10.24 ns |      0.692 ns |   0.038 ns |    1 | 0.0005 |      - |      24 B |
| 'Board (2D Array)' | IsInCheck          |    583.38 ns |    137.848 ns |   7.556 ns |    2 | 0.0629 |      - |    3160 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | LoadFEN            |    430.36 ns |    102.683 ns |   5.628 ns |    1 | 0.0253 |      - |    1288 B |
| 'Board (2D Array)' | LoadFEN            |    661.34 ns |    103.354 ns |   5.665 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| 'Board (2D Array)' | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| 'Board (2D Array)' | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

// * Legends *
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
| 'BitBoard (Magic)' | LoadFEN            |    430.36 ns |    102.683 ns |   5.628 ns |    1 | 0.0253 |      - |    1288 B |
| 'Board (2D Array)' | LoadFEN            |    661.34 ns |    103.354 ns |   5.665 ns |    2 | 0.0582 |      - |    2920 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| 'Board (2D Array)' | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| 'Board (2D Array)' | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

// * Legends *
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
| 'BitBoard (Magic)' | MoveGen - MidGame  |    438.84 ns |    100.996 ns |   5.536 ns |    1 | 0.0677 | 0.0005 |    3416 B |
| 'Board (2D Array)' | MoveGen - MidGame  | 24,419.07 ns |  5,610.865 ns | 307.550 ns |    2 | 2.4719 | 0.0305 |  125040 B |
|                    |                    |              |               |            |      |        |        |           |
| 'BitBoard (Magic)' | MoveGen - Starting |    266.96 ns |    280.886 ns |  15.396 ns |    1 | 0.0439 |      - |    2224 B |
| 'Board (2D Array)' | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

// * Legends *
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
| 'Board (2D Array)' | MoveGen - Starting | 26,064.66 ns | 11,249.671 ns | 616.632 ns |    2 | 2.4109 |      - |  122080 B |

// * Legends *
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
  Categories : All categories of the corresponded method, class, and assembly
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
  Rank       : Relative position of current benchmark mean among all benchmarks (Arabic style)
  Gen0       : GC Generation 0 collects per 1000 operations
  Gen1       : GC Generation 1 collects per 1000 operations
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
  Allocated  : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ns       : 1 Nanosecond (0.000000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:01:59 (119.18 sec), executed benchmarks: 14

// ***** BenchmarkRunner: End *****
Run time: 00:01:59 (119.18 sec), executed benchmarks: 14
Run time: 00:01:59 (119.18 sec), executed benchmarks: 14

Global total time: 00:02:06 (126.28 sec), executed benchmarks: 14
// * Artifacts cleanup *
Artifacts cleanup is finished