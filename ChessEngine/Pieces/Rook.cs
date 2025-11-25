namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class Rook(PieceColor color) : Piece(color, PieceType.Rook) {
    public override bool IsValidMove(Board board, Move move) {
        var columnDifference = Math.Abs(move.From.Column - move.To.Column);
        var rowDifference = Math.Abs(move.From.Row - move.To.Row);

        var onlyMovedOneDirection = columnDifference == 0 || rowDifference == 0;

        return onlyMovedOneDirection 
            && !HasJumpedOverPiece(board, move)
            && !DestinationIsOccupiedByOwnPiece(board, move);
    }

    public override IEnumerable<Move> GetPossibleMoves(Board board, Position from) {
        // Rook slides orthogonally (horizontally/vertically)
        return GenerateSlidingMoves(board, from, OrthogonalDirections);
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
                return true;
            }
        }

        return hasPieceBetween;
    }

    public override Piece Clone() {
        return new Rook(Color) {
            HasMoved = HasMoved
        };
    }
}
