namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

public class Move(Position from, Position to, PieceType? promotedTo = null) {
    public Position From => from;
    public Position To => to;
    public PieceType? PromotedTo { get; set; } = promotedTo;
}