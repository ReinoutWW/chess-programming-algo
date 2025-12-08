namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

public struct UndoMoveInfo {
    public Move Move { get; set; }
    
    public PieceType MovedType { get; set; }
    public PieceColor MovedColor { get; set; }
    
    public PieceType? CapturedType { get; set; }
    public PieceColor? CapturedColor { get; set; }
    
    // Castling state before the move (for undo)
    public bool WhiteKingSideCastle { get; set; }
    public bool WhiteQueenSideCastle { get; set; }
    public bool BlackKingSideCastle { get; set; }
    public bool BlackQueenSideCastle { get; set; }
    public bool WasCastling { get; set; }
    
    // En passant state
    public int PreviousEnPassantSquare { get; set; }
    public bool WasEnPassant { get; set; }
}