using System.Collections.Generic;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Pieces
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color) : base(color, PieceType.Pawn) { }

        public override IEnumerable<Move> GetValidMoves(Board board)
        {
            var moves = new List<Move>();
            int direction = Color == PieceColor.White ? 1 : -1;
            
            // Forward 1
            var forward1 = new Position(Position.Row + direction, Position.Col);
            if (board.IsEmpty(forward1))
            {
                moves.Add(new Move(Position, forward1, this));

                // Forward 2 (if first move)
                bool isStartingRank = (Color == PieceColor.White && Position.Row == 1) || (Color == PieceColor.Black && Position.Row == 6);
                if (isStartingRank)
                {
                    var forward2 = new Position(Position.Row + (direction * 2), Position.Col);
                    if (board.IsEmpty(forward2))
                    {
                        moves.Add(new Move(Position, forward2, this));
                    }
                }
            }

            // Captures
            int[] captureCols = { -1, 1 };
            foreach (var colOffset in captureCols)
            {
                var capturePos = new Position(Position.Row + direction, Position.Col + colOffset);
                if (board.IsValidPosition(capturePos))
                {
                    var target = board.GetPieceAt(capturePos);
                    if (target != null && target.Color != Color)
                    {
                        moves.Add(new Move(Position, capturePos, this, target));
                    }
                }
            }
            // TODO: En Passant, Promotion
            return moves;
        }
    }
}

