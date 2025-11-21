using System;
using System.Linq;
using ChessProgrammingAlgo.Core;
using ChessProgrammingAlgo.Pieces;

namespace ChessProgrammingAlgo.Players
{
    public class RandomPlayer : Player
    {
        private Random _random = new Random();

        public RandomPlayer(PieceColor color) : base(color) { }

        public override Move GetMove(Game game)
        {
            // Get all pieces for this player
            var myPieces = game.Board.Grid.Cast<Piece>() // Flatten the 2D array
                .Where(p => p != null && p.Color == Color)
                .ToList();

            // Collect all valid moves
            var allMoves = myPieces
                .SelectMany(p => game.GetLegalMoves(p))
                .ToList();

            if (allMoves.Count == 0) return null; // No moves (Checkmate or Stalemate)

            return allMoves[_random.Next(allMoves.Count)];
        }
    }
}

