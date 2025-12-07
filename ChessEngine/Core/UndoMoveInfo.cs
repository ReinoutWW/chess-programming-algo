namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

public struct UndoMoveInfo {
    public Move Move { get; set; }
    
    public PieceType MovedType { get; set; }
    public PieceColor MovedColor { get; set; }
    
    public PieceType? CapturedType { get; set; }
    public PieceColor? CapturedColor { get; set; }
}