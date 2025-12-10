namespace ChessEngine.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Chess.Programming.Ago.Core;

/// <summary>
/// Benchmarks for attack calculations and check detection.
/// Magic bitboards are tested here for their lookup performance.
/// </summary>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class AttackBenchmarks
{
    private BitBoard _midGamePosition = null!;
    private BitBoard _complexPosition = null!;
    private Position _centerSquare;
    private Position _kingPosition;
    
    private const string MidGameFen = "r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4";
    private const string ComplexFen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1";
    
    [GlobalSetup]
    public void Setup()
    {
        _midGamePosition = new BitBoard();
        _midGamePosition.LoadForsythEdwardsNotation(MidGameFen);
        
        _complexPosition = new BitBoard();
        _complexPosition.LoadForsythEdwardsNotation(ComplexFen);
        
        _centerSquare = new Position(3, 3); // d4
        _kingPosition = new Position(0, 4); // e1
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // CHECK DETECTION BENCHMARKS
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "IsInCheck - Not in check")]
    public bool IsInCheck_NotInCheck()
    {
        return _midGamePosition.IsInCheck(PieceColor.White);
    }
    
    [Benchmark(Description = "IsSquareAttacked - Center Square")]
    public bool IsSquareAttacked_Center()
    {
        return _midGamePosition.IsSquareAttacked(PieceColor.White, _centerSquare);
    }
    
    [Benchmark(Description = "IsSquareAttacked - King Square")]
    public bool IsSquareAttacked_King()
    {
        return _midGamePosition.IsSquareAttacked(PieceColor.White, _kingPosition);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MAGIC BITBOARD ATTACK LOOKUPS
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "Knight Attacks Lookup (precomputed)")]
    public ulong KnightAttacks()
    {
        return BitBoard.GetKnightAttacksForSquare(27); // d4
    }
    
    [Benchmark(Description = "King Attacks Lookup (precomputed)")]
    public ulong KingAttacks()
    {
        return BitBoard.GetKingAttacksForSquare(27); // d4
    }
    
    [Benchmark(Description = "Rook Attacks (magic bitboard lookup)")]
    public ulong RookAttacks()
    {
        return _midGamePosition.GetRookAttacksForSquare(27); // d4
    }
    
    [Benchmark(Description = "Bishop Attacks (magic bitboard lookup)")]
    public ulong BishopAttacks()
    {
        return _midGamePosition.GetBishopAttacksForSquare(27); // d4
    }
    
    [Benchmark(Description = "Queen Attacks (rook + bishop)")]
    public ulong QueenAttacks()
    {
        return _midGamePosition.GetQueenAttacksForSquare(27); // d4
    }
    
    [Benchmark(Description = "Pawn Attacks (White)")]
    public ulong PawnAttacks_White()
    {
        return BitBoard.GetPawnAttacksForSquare(27, PieceColor.White); // d4
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // ALL ATTACKS FOR COLOR
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "GetAllAttacksForColor - White")]
    public ulong AllAttacks_White()
    {
        return _midGamePosition.GetAllAttacksForColor(PieceColor.White);
    }
    
    [Benchmark(Description = "GetAllAttacksForColor - Black")]
    public ulong AllAttacks_Black()
    {
        return _midGamePosition.GetAllAttacksForColor(PieceColor.Black);
    }
    
    // ═══════════════════════════════════════════════════════════════════
    // MAGIC BITBOARD DEBUG INFO
    // ═══════════════════════════════════════════════════════════════════
    
    [Benchmark(Description = "GetRookMagicInfo (debug)")]
    public MagicBitboardInfo RookMagicInfo()
    {
        return _midGamePosition.GetRookMagicInfo(27);
    }
    
    [Benchmark(Description = "GetBishopMagicInfo (debug)")]
    public MagicBitboardInfo BishopMagicInfo()
    {
        return _midGamePosition.GetBishopMagicInfo(27);
    }
}

