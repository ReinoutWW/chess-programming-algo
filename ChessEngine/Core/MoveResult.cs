namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

public class MoveResult(bool wasCapture, Piece? capturedPiece) {
    public bool WasCapture { get; set; } = wasCapture;
    public Piece? CapturedPiece { get; set; } = capturedPiece;
}

/// <summary>
/// Extended move result that stores all information needed to unmake a move.
/// Used by the search algorithm for make/unmake pattern (no cloning needed).
/// </summary>
public class UndoInfo {
    public Move Move { get; init; } = null!;
    public Piece? CapturedPiece { get; init; }
    public Position? CapturedPiecePosition { get; init; }  // For en passant (different from To)
    public Move? PreviousLastMove { get; init; }
    public bool MovingPieceHadMoved { get; init; }
    public Piece? OriginalMovingPiece { get; init; }  // For pawn promotion (to restore the pawn)
    
    // Castling info
    public bool WasCastling { get; init; }
    public Position? RookFrom { get; init; }
    public Position? RookTo { get; init; }
    public bool RookHadMoved { get; init; }
}