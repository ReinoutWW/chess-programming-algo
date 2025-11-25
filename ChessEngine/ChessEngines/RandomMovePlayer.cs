namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

/// <summary>
/// Probablt the most shit player in the set. 
/// Will choose a random move from the valid moves. 
/// It might pick the piece multiple times because it doesnt check if the piece has already been used, and the list is not shuffled.
/// </summary>
/// <param name="color"></param>
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

            var randomMove = validMoves[Random.Shared.Next(validMoves.Count)];
            return new Move(piece.Item2, randomMove.To, randomMove.PromotedTo);
        }

        Console.WriteLine("No valid moves found!");
        throw new InvalidOperationException("No valid moves found");
    }
}   