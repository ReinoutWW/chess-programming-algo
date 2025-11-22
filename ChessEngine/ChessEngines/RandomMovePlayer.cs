namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public class RandomMovePlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public bool IsAI() => true;
    public async Task<Move> GetMove(IGame game) {
        Console.WriteLine("Getting move for " + color);
        foreach(var piece in game.GetBoard().GetPiecesForColor(color)) {
            var validMoves = game.GetValidMovesForPosition(piece.Item2);
            if(validMoves.Count == 0) {
                continue;
            }

            Console.WriteLine("Valid moves found!");

            return new Move(piece.Item2, validMoves[Random.Shared.Next(validMoves.Count)]);
        }

        Console.WriteLine("No valid moves found!");
        throw new InvalidOperationException("No valid moves found");
    }
}   