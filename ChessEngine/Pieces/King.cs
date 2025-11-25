namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class King(PieceColor color) : Piece(color, PieceType.King) {
    public override bool IsValidMove(Board board, Move move) {
        bool isAllowedToMove = IsOneSquareAway(move.From, move.To);

        // Is moving from the starting position to the left or right (2 squares)
        // And if BOTH pieces between de squares are empty
        // And if BOTH pieces have not moved yet
        var isCastlingMove = board.IsCastlingMove(move, color);
        
        return isAllowedToMove || isCastlingMove && IsTwoSquaresHorizontal(move.From, move.To);
    }

    public override IEnumerable<Move> GetPossibleMoves(Board board, Position from) {
        // Generate all 8 adjacent squares
        foreach (var (dRow, dCol) in AllDirections) {
            int toRow = from.Row + dRow;
            int toCol = from.Column + dCol;

            if (!IsOnBoard(toRow, toCol)) continue;

            var targetPiece = board.GetPieceAtPosition(new Position(toRow, toCol));
            
            // Can move if square is empty or contains enemy piece
            if (targetPiece == null || targetPiece.Color != Color) {
                yield return new Move(from, new Position(toRow, toCol));
            }
        }

        // Generate castling moves (only if king hasn't moved)
        if (!HasMoved) {
            // Kingside castling (move 2 squares right)
            var kingsideCastling = new Move(from, new Position(from.Row, from.Column + 2));
            if (board.IsCastlingMove(kingsideCastling, Color)) {
                yield return kingsideCastling;
            }

            // Queenside castling (move 2 squares left)
            var queensideCastling = new Move(from, new Position(from.Row, from.Column - 2));
            if (board.IsCastlingMove(queensideCastling, Color)) {
                yield return queensideCastling;
            }
        }
    }

    private bool IsOneSquareAway(Position from, Position to) {
        return Math.Abs(from.Row - to.Row) <= 1 
            && Math.Abs(from.Column - to.Column) <= 1;
    }
    
    private bool IsTwoSquaresHorizontal(Position from, Position to) {
        return from.Row == to.Row 
            && Math.Abs(from.Column - to.Column) == 2;
    }

    public override bool CanAttackSquare(Board board, Move move) {
        // King attacks one square in any direction (excludes castling)
        return IsOneSquareAway(move.From, move.To);
    }

    public override Piece Clone() {
        return new King(Color) {
            HasMoved = HasMoved
        };
    }
}