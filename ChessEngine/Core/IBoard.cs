namespace Chess.Programming.Ago.Core;

public interface IBoard {

    void ApplyMove(Move move);
    bool IsSquareAttacked(PieceColor color, Position position);
    bool IsInCheck(PieceColor color);
}