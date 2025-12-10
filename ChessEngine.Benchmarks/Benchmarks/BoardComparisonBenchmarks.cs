namespace ChessEngine.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Side-by-side comparison benchmarks between the legacy 2D array Board
/// and the BitBoard implementation.
/// 
/// This demonstrates the performance difference between:
/// - Classical array-based board representation
/// - Magic bitboard representation with precomputed attacks
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class BoardComparisonBenchmarks
{
    private Board _board = null!;
    private BitBoard _bitBoard = null!;
    
    private Board _midGameBoard = null!;
    private BitBoard _midGameBitBoard = null!;
    
    private Move _e2e4 = null!;
    
    private const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private const string MidGameFen = "r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
    
    [GlobalSetup]
    public void Setup()
    {
        // Starting position setup
        _board = new Board();
        _board.LoadForsythEdwardsNotation(StartingFen);
        
        _bitBoard = new BitBoard();
        // BitBoard is already at starting position by default
        
        // MidGame position setup
        _midGameBoard = new Board();
        _midGameBoard.LoadForsythEdwardsNotation(MidGameFen);
        
        _midGameBitBoard = new BitBoard();
        _midGameBitBoard.LoadForsythEdwardsNotation(MidGameFen);
        
        _e2e4 = new Move(new Position(6, 4), new Position(4, 4)); // e2-e4 (Board uses different row orientation)
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MOVE GENERATION COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("MoveGen - Starting")]
    public List<Move> MoveGen_Board_Starting()
    {
        return _board.GenerateMoves(PieceColor.White);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("MoveGen - Starting")]
    public List<Move> MoveGen_BitBoard_Starting()
    {
        return _bitBoard.GenerateMoves(PieceColor.White);
    }
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("MoveGen - MidGame")]
    public List<Move> MoveGen_Board_MidGame()
    {
        return _midGameBoard.GenerateMoves(PieceColor.White);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("MoveGen - MidGame")]
    public List<Move> MoveGen_BitBoard_MidGame()
    {
        return _midGameBitBoard.GenerateMoves(PieceColor.White);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // APPLY + UNDO MOVE COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("ApplyUndo")]
    public void ApplyUndo_Board()
    {
        var undoInfo = _board.ApplyMove(_e2e4);
        _board.UndoMove(undoInfo);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("ApplyUndo")]
    public void ApplyUndo_BitBoard()
    {
        // BitBoard uses different position mapping (0-63 index)
        var move = new Move(new Position(1, 4), new Position(3, 4)); // e2-e4 in BitBoard coordinates
        var undoInfo = _bitBoard.ApplyMove(move);
        _bitBoard.UndoMove(undoInfo);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // CHECK DETECTION COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("IsInCheck")]
    public bool IsInCheck_Board()
    {
        return _midGameBoard.IsInCheck(PieceColor.White);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("IsInCheck")]
    public bool IsInCheck_BitBoard()
    {
        return _midGameBitBoard.IsInCheck(PieceColor.White);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // CLONE COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("Clone")]
    public Board Clone_Board()
    {
        return _midGameBoard.Clone();
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("Clone")]
    public IBoard Clone_BitBoard()
    {
        return _midGameBitBoard.Clone();
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // GET PIECES COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("GetPiecesForColor")]
    public List<(Piece, Position)> GetPieces_Board()
    {
        return _midGameBoard.GetPiecesForColor(PieceColor.White);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("GetPiecesForColor")]
    public List<(Piece, Position)> GetPieces_BitBoard()
    {
        return _midGameBitBoard.GetPiecesForColor(PieceColor.White);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // LOAD FEN COMPARISON
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Board (2D Array)")]
    [BenchmarkCategory("LoadFEN")]
    public void LoadFen_Board()
    {
        var board = new Board();
        board.LoadForsythEdwardsNotation(MidGameFen);
    }
    
    [Benchmark(Description = "BitBoard (Magic)")]
    [BenchmarkCategory("LoadFEN")]
    public void LoadFen_BitBoard()
    {
        var board = new BitBoard();
        board.LoadForsythEdwardsNotation(MidGameFen);
    }
}

