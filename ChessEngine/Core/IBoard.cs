namespace Chess.Programming.Ago.Core;

public interface IBoard {
    UndoMoveInfo ApplyMove(Move move);
    void UndoMove(UndoMoveInfo undoMoveInfo);
    bool IsSquareAttacked(PieceColor color, Position position);
    bool IsInCheck(PieceColor color);
}