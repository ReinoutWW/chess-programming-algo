using System;
using System.Linq;
using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Players
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(PieceColor color) : base(color) { }

        public override Move GetMove(Game game)
        {
            // Simple Console Input for now
            // Format: "e2 e4" or "1,4 3,4"
            while (true)
            {
                Console.WriteLine($"{Color}'s turn. Enter move (e.g., e2e4): ");
                string input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input)) continue;

                Move move = ParseMove(input, game);
                if (move != null) return move;

                Console.WriteLine("Invalid move. Try again.");
            }
        }

        private Move ParseMove(string input, Game game)
        {
            try
            {
                if (input.Length != 4) return null;

                var fromPos = ParsePosition(input.Substring(0, 2));
                var toPos = ParsePosition(input.Substring(2, 2));

                if (!game.Board.IsValidPosition(fromPos) || !game.Board.IsValidPosition(toPos)) return null;

                var piece = game.Board.GetPieceAt(fromPos);
                if (piece == null || piece.Color != Color) return null;

                var legalMoves = game.GetLegalMoves(piece);
                return legalMoves.FirstOrDefault(m => m.To == toPos);
            }
            catch
            {
                return null;
            }
        }

        private Position ParsePosition(string posStr)
        {
            // e2 -> col 4 (e), row 1 (2)
            // Files: a=0, b=1, ... h=7
            // Ranks: 1=0, 2=1, ... 8=7
            
            char fileChar = char.ToLower(posStr[0]);
            char rankChar = posStr[1];

            int col = fileChar - 'a';
            int row = rankChar - '1';

            return new Position(row, col);
        }
    }
}
