namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;

public class MoveResult(bool wasCapture, Piece? capturedPiece) {
    public bool WasCapture { get; set; } = wasCapture;
    public Piece? CapturedPiece { get; set; } = capturedPiece;
}