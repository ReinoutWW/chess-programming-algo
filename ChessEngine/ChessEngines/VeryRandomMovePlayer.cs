namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

/// <summary>
/// A bit better than the RandomMovePlayer.
/// Will choose a random piece from the list of pieces for the color.
/// Will then choose a random move from the valid moves for the piece.
/// It will not pick the piece multiple times, as it shuffles the list of pieces.
/// </summary>
/// <param name="color"></param>
public class VeryRandomMovePlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public bool IsAI() => true;
    public async Task<Move> GetMove(IGame game) {
        Console.WriteLine("Getting move for " + color);

        var pieces = game.GetBoard().GetPiecesForColor(color);
        pieces = pieces.OrderBy(p => Random.Shared.Next()).ToList();
        
        foreach(var piece in pieces) {
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