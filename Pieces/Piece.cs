using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public abstract class Piece
    {
        public PieceColor Color { get; }
        public PieceType Type { get; }
        public Position Position { get; set; }
        public bool HasMoved { get; set; } = false;

        protected Piece(PieceColor color, PieceType type)
        {
            Color = color;
            Type = type;
        }

        public abstract IEnumerable<Move> GetValidMoves(Board board);
        
        // Helper for sliding pieces (Rook, Bishop, Queen)
        protected IEnumerable<Move> GetSlidingMoves(Board board, int[] dRow, int[] dCol)
        {
            var moves = new List<Move>();

            for (int i = 0; i < dRow.Length; i++)
            {
                int r = Position.Row + dRow[i];
                int c = Position.Col + dCol[i];

                while (board.IsValidPosition(new Position(r, c)))
                {
                    var pos = new Position(r, c);
                    var pieceAtDest = board.GetPieceAt(pos);

                    if (pieceAtDest == null)
                    {
                        moves.Add(new Move(Position, pos, this));
                    }
                    else
                    {
                        if (pieceAtDest.Color != Color)
                        {
                            moves.Add(new Move(Position, pos, this, pieceAtDest));
                        }
                        break; // Blocked
                    }

                    r += dRow[i];
                    c += dCol[i];
                }
            }
            return moves;
        }
    }
}

