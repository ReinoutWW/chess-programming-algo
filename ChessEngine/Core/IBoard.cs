using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Core;

public interface IBoard {
    UndoMoveInfo ApplyMove(Move move);
    void UndoMove(UndoMoveInfo undoMoveInfo);
    bool IsSquareAttacked(PieceColor color, Position position);
    bool IsInCheck(PieceColor color);
    IBoard Clone();
    List<Move> GenerateMoves(PieceColor color);
    List<(Piece, Position)> GetPiecesForColor(PieceColor color);
    Piece[,] GetPieces();
    Piece? GetPieceAtPosition(Position position);
}