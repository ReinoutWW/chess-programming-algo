using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public class Rook : Piece
    {
        public Rook(PieceColor color) : base(color, PieceType.Rook) { }

        public override IEnumerable<Move> GetValidMoves(Board board)
        {
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };
            return GetSlidingMoves(board, dr, dc);
        }
    }
}

