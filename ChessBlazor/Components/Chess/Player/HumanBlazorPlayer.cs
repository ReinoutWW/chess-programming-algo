namespace ChessBlazor.Components.Chess.Player;

using global::Chess.Programming.Ago.Core;
using global::Chess.Programming.Ago.Game;
using global::Chess.Programming.Ago.Pieces;

public class HumanBlazorPlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public Move? SelectedMove { get; set; }
    public bool IsAI() => false;
    public Task<Move> GetMove(IGame game) {
        return Task.FromResult(SelectedMove) 
            ?? throw new InvalidOperationException("No move selected");
    }
}
