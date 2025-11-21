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
}   