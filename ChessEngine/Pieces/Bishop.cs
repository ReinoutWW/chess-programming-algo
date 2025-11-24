namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class Bishop(PieceColor color) : Piece(color, PieceType.Bishop) {
    public override bool IsValidMove(Board board, Move move) {
        var columnDifference = Math.Abs(move.From.Column - move.To.Column);
        var rowDifference = Math.Abs(move.From.Row - move.To.Row);

        var onlyMovedDiagonally = columnDifference == rowDifference;

        return onlyMovedDiagonally 
            && !HasJumpedOverPiece(board, move) 
            && !DestinationIsOccupiedByOwnPiece(board, move);
    }

    private bool DestinationIsOccupiedByOwnPiece(Board board, Move move) {
        return board.GetPieceAtPosition(move.To) != null 
            && board.GetPieceAtPosition(move.To)!.Color == Color;
    }

    private bool HasJumpedOverPiece(Board board, Move move) {
        var hasPieceBetween = false;

        var rowStep = Math.Sign(move.To.Row - move.From.Row);
        var columnStep = Math.Sign(move.To.Column - move.From.Column);

        var row = move.From.Row;
        var column = move.From.Column;

        while(row != move.To.Row || column != move.To.Column) {
            row += rowStep;
            column += columnStep;

            if (row == move.To.Row && column == move.To.Column) {
                break;
            }

            if(board.GetPieceAtPosition(new Position(row, column)) != null) {
                hasPieceBetween = true;
                break;
            }
        }

        return hasPieceBetween;
    }

    public override Piece Clone() {
        return new Bishop(Color) {
            HasMoved = HasMoved
        };
    }
}