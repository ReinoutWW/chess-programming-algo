namespace ChessEngine.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Benchmarks for core BitBoard operations.
/// These are the most performance-critical operations in a chess engine.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BitBoardBenchmarks
{
    private BitBoard _startingPosition = null!;
    private BitBoard _midGamePosition = null!;
    private BitBoard _complexPosition = null!;
    private Move _e2e4 = null!;
    
    // Common test positions (FEN strings)
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private const string MidGameFen = "r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
    private const string ComplexFen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
    
    [GlobalSetup]
    public void Setup()
    {
        _startingPosition = new BitBoard();
        
        _midGamePosition = new BitBoard();
        _midGamePosition.LoadForsythEdwardsNotation(MidGameFen);
        
        _complexPosition = new BitBoard();
        _complexPosition.LoadForsythEdwardsNotation(ComplexFen);
        
        _e2e4 = new Move(new Position(1, 4), new Position(3, 4));
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MOVE GENERATION BENCHMARKS
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "GenerateMoves - Starting Position (White)")]
    public List<Move> GenerateMoves_StartingPosition_White()
    {
        return _startingPosition.GenerateMoves(PieceColor.White);
    }
    
    [Benchmark(Description = "GenerateMoves - Starting Position (Black)")]
    public List<Move> GenerateMoves_StartingPosition_Black()
    {
        return _startingPosition.GenerateMoves(PieceColor.Black);
    }
    
    [Benchmark(Description = "GenerateMoves - MidGame Position")]
    public List<Move> GenerateMoves_MidGame()
    {
        return _midGamePosition.GenerateMoves(PieceColor.White);
    }
    
    [Benchmark(Description = "GenerateMoves - Complex Position (Kiwipete)")]
    public List<Move> GenerateMoves_Complex()
    {
        return _complexPosition.GenerateMoves(PieceColor.White);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // APPLY/UNDO MOVE BENCHMARKS
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "ApplyMove + UndoMove")]
    public void ApplyAndUndoMove()
    {
        var undoInfo = _startingPosition.ApplyMove(_e2e4);
        _startingPosition.UndoMove(undoInfo);
    }
    
    [Benchmark(Description = "ApplyMove only")]
    public UndoMoveInfo ApplyMove()
    {
        var undoInfo = _startingPosition.ApplyMove(_e2e4);
        _startingPosition.UndoMove(undoInfo); // Reset for next iteration
        return undoInfo;
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // BOARD OPERATIONS BENCHMARKS
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Clone Board")]
    public IBoard CloneBoard()
    {
        return _midGamePosition.Clone();
    }
    
    [Benchmark(Description = "Load FEN Position")]
    public void LoadFen()
    {
        var board = new BitBoard();
        board.LoadForsythEdwardsNotation(ComplexFen);
    }
    
    [Benchmark(Description = "GetPieces (8x8 array)")]
    public Piece[,] GetPieces()
    {
        return _midGamePosition.GetPieces();
    }
    
    [Benchmark(Description = "GetPiecesForColor")]
    public List<(Piece, Position)> GetPiecesForColor()
    {
        return _midGamePosition.GetPiecesForColor(PieceColor.White);
    }
}

