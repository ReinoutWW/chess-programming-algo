namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class King(PieceColor color) : Piece(color, PieceType.King) {
    public override bool IsValidMove(Board board, Move move) {
        bool isAllowedToMove = IsOneSquareAway(move.From, move.To);
        
        bool isAllowedToCastle = IsAllowedToCastle(board, move);

        return isAllowedToMove || isAllowedToCastle;
    }

    private bool IsOneSquareAway(Position from, Position to) {
        return Math.Abs(from.Row - to.Row) == 1 
            || Math.Abs(from.Column - to.Column) == 1;
    }

    private bool IsAllowedToCastle(Board board, Move move) {
        return false;
    }
}