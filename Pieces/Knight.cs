namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight) {
    public override bool IsValidMove(Board board, Move move) {
        var columnDifference = Math.Abs(move.From.Column - move.To.Column);
        var rowDifference = Math.Abs(move.From.Row - move.To.Row);

        var onlyMovedInLShape = (columnDifference == 2 && rowDifference == 1) 
            || (columnDifference == 1 && rowDifference == 2);

        return onlyMovedInLShape 
            && !DestinationIsOccupiedByOwnPiece(board, move);
    }

    private bool DestinationIsOccupiedByOwnPiece(Board board, Move move) {
        return board.GetPieceAtPosition(move.To) != null 
            && board.GetPieceAtPosition(move.To)!.Color == Color;
    }
}   