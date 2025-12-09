using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Core;

public interface IVisualizedBoard : IBoard {
    void LogBoard();
    
    // Bitboard visualization accessors
    ulong OccupiedSquares { get; }
    ulong WhitePieces { get; }
    ulong BlackPieces { get; }
    ulong GetPieceBitboard(PieceColor color, PieceType type);
    
    // Attack pattern accessors
    ulong GetRookAttacksForSquare(int square);
    ulong GetBishopAttacksForSquare(int square);
    ulong GetQueenAttacksForSquare(int square);
    
    // Magic bitboard info for educational display
    MagicBitboardInfo GetRookMagicInfo(int square);
    MagicBitboardInfo GetBishopMagicInfo(int square);
    
    // Combined attack patterns
    ulong GetAllAttacksForColor(PieceColor color);
}