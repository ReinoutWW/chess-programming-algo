namespace ChessConsole;

using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Core;

public class HumanConsolePlayer(PieceColor color) : IPlayer {

    public PieceColor Color => color;
    public bool IsAI() => false;
    
    public async Task<Move> GetMove(IGame game) {
        Console.WriteLine("Enter your move (e.g. a1a2):");

        var move = Console.ReadLine();

        if(move == null) {
            throw new InvalidOperationException("Invalid move");
        }

        var (from, to) = move.GetPosition();

        return await Task.FromResult(new Move(from, to));
    }
}