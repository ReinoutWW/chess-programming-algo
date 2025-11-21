using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public class King : Piece
    {
        public King(PieceColor color) : base(color, PieceType.King) { }

        public override IEnumerable<Move> GetValidMoves(Board board)
        {
            var moves = new List<Move>();
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                var pos = new Position(Position.Row + dr[i], Position.Col + dc[i]);
                if (board.IsValidPosition(pos))
                {
                    var pieceAtDest = board.GetPieceAt(pos);
                    if (pieceAtDest == null || pieceAtDest.Color != Color)
                    {
                        moves.Add(new Move(Position, pos, this, pieceAtDest));
                    }
                }
            }
            // TODO: Castling
            return moves;
        }
    }
}

