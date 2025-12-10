using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using ChessEngine.Benchmarks;

// Run all benchmarks
var config = DefaultConfig.Instance
    .WithSummaryStyle(SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend));

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);

