namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public abstract class Piece {
    public PieceColor Color { get; }
    public PieceType Type { get; }
    public bool HasMoved { get; set; } = false;

    // Direction vectors for sliding pieces
    protected static readonly (int dRow, int dCol)[] DiagonalDirections = [(-1, -1), (-1, 1), (1, -1), (1, 1)];
    protected static readonly (int dRow, int dCol)[] OrthogonalDirections = [(-1, 0), (1, 0), (0, -1), (0, 1)];
    protected static readonly (int dRow, int dCol)[] AllDirections = [(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)];

    public Piece(PieceColor color, PieceType type) {
        Color = color;
        Type = type;
    }

    public abstract bool IsValidMove(Board board, Move move);

    /// <summary>
    /// Generates all pseudo-legal moves for this piece from the given position.
    /// Pseudo-legal moves don't account for leaving the king in check.
    /// This is much more efficient than testing all 64 squares.
    /// </summary>
    public abstract IEnumerable<Move> GetPossibleMoves(Board board, Position from);

    /// <summary>
    /// Checks if this piece can attack the target square.
    /// By default, attack pattern equals valid moves.
    /// Override for pieces where this differs (King excludes castling, Pawn attacks diagonally).
    /// </summary>
    public virtual bool CanAttackSquare(Board board, Move move) {
        return IsValidMove(board, move);
    }

    public abstract Piece Clone();

    /// <summary>
    /// Helper: generates moves along a direction until blocked or off-board (for sliding pieces).
    /// </summary>
    protected IEnumerable<Move> GenerateSlidingMoves(Board board, Position from, (int dRow, int dCol)[] directions) {
        foreach (var (dRow, dCol) in directions) {
            int row = from.Row + dRow;
            int col = from.Column + dCol;

            while (IsOnBoard(row, col)) {
                var targetPiece = board.GetPieceAtPosition(new Position(row, col));
                
                if (targetPiece == null) {
                    // Empty square - can move here and continue
                    yield return new Move(from, new Position(row, col));
                } else if (targetPiece.Color != Color) {
                    // Enemy piece - can capture but stop sliding
                    yield return new Move(from, new Position(row, col));
                    break;
                } else {
                    // Own piece - blocked, stop sliding
                    break;
                }

                row += dRow;
                col += dCol;
            }
        }
    }

    protected static bool IsOnBoard(int row, int col) {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }
}   