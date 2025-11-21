using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(PieceColor color) : base(color, PieceType.Bishop) { }

        public override IEnumerable<Move> GetValidMoves(Board board)
        {
            int[] dr = { -1, -1, 1, 1 };
            int[] dc = { -1, 1, -1, 1 };
            return GetSlidingMoves(board, dr, dc);
        }
    }
}

