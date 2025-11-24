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

    private bool IsOneSquareAway(Position from, Position to) {
        return Math.Abs(from.Row - to.Row) <= 1 
            && Math.Abs(from.Column - to.Column) <= 1;
    }
    
    private bool IsTwoSquaresHorizontal(Position from, Position to) {
        return from.Row == to.Row 
            && Math.Abs(from.Column - to.Column) == 2;
    }

    public override Piece Clone() {
        return new King(Color) {
            HasMoved = HasMoved
        };
    }
}