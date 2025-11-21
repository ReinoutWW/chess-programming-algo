using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public class Queen : Piece
    {
        public Queen(PieceColor color) : base(color, PieceType.Queen) { }

        public override IEnumerable<Move> GetValidMoves(Board board)
        {
            // Combine Rook and Bishop directions
            int[] dr = { -1, 1, 0, 0, -1, -1, 1, 1 };
            int[] dc = { 0, 0, -1, 1, -1, 1, -1, 1 };
            return GetSlidingMoves(board, dr, dc);
        }
    }
}

