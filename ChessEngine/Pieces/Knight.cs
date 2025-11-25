namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class Knight(PieceColor color) : Piece(color, PieceType.Knight) {
    // All 8 possible knight move offsets
    private static readonly (int dRow, int dCol)[] KnightOffsets = [
        (-2, -1), (-2, 1), (-1, -2), (-1, 2),
        (1, -2), (1, 2), (2, -1), (2, 1)
    ];

    public override bool IsValidMove(Board board, Move move) {
        var columnDifference = Math.Abs(move.From.Column - move.To.Column);
        var rowDifference = Math.Abs(move.From.Row - move.To.Row);

        var onlyMovedInLShape = (columnDifference == 2 && rowDifference == 1) 
            || (columnDifference == 1 && rowDifference == 2);

        return onlyMovedInLShape 
            && !DestinationIsOccupiedByOwnPiece(board, move);
    }

    public override IEnumerable<Move> GetPossibleMoves(Board board, Position from) {
        foreach (var (dRow, dCol) in KnightOffsets) {
            int toRow = from.Row + dRow;
            int toCol = from.Column + dCol;

            if (!IsOnBoard(toRow, toCol)) continue;

            var targetPiece = board.GetPieceAtPosition(new Position(toRow, toCol));
            
            // Can move if square is empty or contains enemy piece
            if (targetPiece == null || targetPiece.Color != Color) {
                yield return new Move(from, new Position(toRow, toCol));
            }
        }
    }

    private bool DestinationIsOccupiedByOwnPiece(Board board, Move move) {
        return board.GetPieceAtPosition(move.To) != null 
            && board.GetPieceAtPosition(move.To)!.Color == Color;
    }

    public override Piece Clone() {
        return new Knight(Color) {
            HasMoved = HasMoved
        };
    }
}   