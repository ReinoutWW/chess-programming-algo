namespace ChessEngine.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.ChessEngines;

/// <summary>
/// Benchmarks for evaluation functions.
/// Evaluation is called at every leaf node of the search tree,
/// making it extremely performance-critical.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class EvaluationBenchmarks
{
    private BitBoardGame _startingGame = null!;
    private BitBoardGame _midGame = null!;
    private BitBoardGame _complexGame = null!;
    
    private MaterialEvaluation _materialEval = null!;
    private MaterialMobilityFunction _materialMobilityEval = null!;
    
    private const string MidGameFen = "r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
    private const string ComplexFen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
    
    [GlobalSetup]
    public void Setup()
    {
        var dummyPlayer = new RandomMovePlayer(PieceColor.White);
        var dummyPlayer2 = new RandomMovePlayer(PieceColor.Black);
        
        _startingGame = new BitBoardGame(dummyPlayer, dummyPlayer2, 0);
        
        _midGame = new BitBoardGame(dummyPlayer, dummyPlayer2, 0);
        _midGame.LoadForsythEdwardsNotation(MidGameFen);
        
        _complexGame = new BitBoardGame(dummyPlayer, dummyPlayer2, 0);
        _complexGame.LoadForsythEdwardsNotation(ComplexFen);
        
        _materialEval = new MaterialEvaluation();
        _materialMobilityEval = new MaterialMobilityFunction();
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MATERIAL EVALUATION (Simple)
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "MaterialEvaluation - Starting")]
    public int MaterialEval_Starting()
    {
        return _materialEval.Evaluate(_startingGame, PieceColor.White);
    }
    
    [Benchmark(Description = "MaterialEvaluation - MidGame")]
    public int MaterialEval_MidGame()
    {
        return _materialEval.Evaluate(_midGame, PieceColor.White);
    }
    
    [Benchmark(Description = "MaterialEvaluation - Complex")]
    public int MaterialEval_Complex()
    {
        return _materialEval.Evaluate(_complexGame, PieceColor.White);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MATERIAL + MOBILITY EVALUATION (More complex)
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "MaterialMobilityEval - Starting")]
    public int MaterialMobilityEval_Starting()
    {
        return _materialMobilityEval.Evaluate(_startingGame, PieceColor.White);
    }
    
    [Benchmark(Description = "MaterialMobilityEval - MidGame")]
    public int MaterialMobilityEval_MidGame()
    {
        return _materialMobilityEval.Evaluate(_midGame, PieceColor.White);
    }
    
    [Benchmark(Description = "MaterialMobilityEval - Complex")]
    public int MaterialMobilityEval_Complex()
    {
        return _materialMobilityEval.Evaluate(_complexGame, PieceColor.White);
    }
}

