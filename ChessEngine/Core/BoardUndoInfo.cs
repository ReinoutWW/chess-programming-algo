namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

/// <summary>
/// Captures all state needed to undo a move on the legacy Board class.
/// This is separate from UndoMoveInfo because Board uses Piece references
/// while BitBoard uses piece types and bitboard operations.
/// </summary>
public struct BoardUndoInfo {
    public Move Move { get; set; }
    
    /// <summary>
    /// The piece that was moved.
    /// </summary>
    public Piece MovedPiece { get; set; }
    
    /// <summary>
    /// Whether the moved piece had moved before this move.
    /// </summary>
    public bool MovedPieceHadMoved { get; set; }
    
    /// <summary>
    /// The piece that was captured, if any.
    /// </summary>
    public Piece? CapturedPiece { get; set; }
    
    /// <summary>
    /// Position where the captured piece was (usually move.To, but for en passant it differs).
    /// </summary>
    public Position? CapturedPosition { get; set; }
    
    /// <summary>
    /// The previous last move before this one was made.
    /// </summary>
    public Move? PreviousLastMove { get; set; }
    
    // Castling-specific undo info
    public bool WasCastling { get; set; }
    public Piece? CastlingRook { get; set; }
    public Position? RookFromPosition { get; set; }
    public Position? RookToPosition { get; set; }
    public bool RookHadMoved { get; set; }
    
    // En passant-specific
    public bool WasEnPassant { get; set; }
    
    // Promotion-specific
    public bool WasPromotion { get; set; }
    public Piece? OriginalPawn { get; set; }
}

