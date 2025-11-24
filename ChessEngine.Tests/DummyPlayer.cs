namespace ChessEngine.Tests;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;

public class DummyPlayer : IPlayer {
    public PieceColor Color { get; }
    
    public DummyPlayer(PieceColor color) {
        Color = color;
    }
    
    public bool IsAI() => false;
    
    public Task<Move> GetMove(IGame game) {
        return Task.FromResult(new Move(new Position(0, 0), new Position(0, 0)));
    }
}
