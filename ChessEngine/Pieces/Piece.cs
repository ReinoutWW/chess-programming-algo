namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public abstract class Piece {
    public PieceColor Color { get; }
    public PieceType Type { get; }
    public bool HasMoved { get; set; } = false;


    public Piece(PieceColor color, PieceType type) {
        Color = color;
        Type = type;
    }

    public abstract bool IsValidMove(Board board, Move move);

    /// <summary>
    /// Checks if this piece can attack the target square.
    /// By default, attack pattern equals valid moves.
    /// Override for pieces where this differs (King excludes castling, Pawn attacks diagonally).
    /// </summary>
    public virtual bool CanAttackSquare(Board board, Move move) {
        return IsValidMove(board, move);
    }

    public abstract Piece Clone();
}   