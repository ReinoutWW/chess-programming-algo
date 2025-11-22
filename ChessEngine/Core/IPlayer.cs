namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Game;

public interface IPlayer {

    PieceColor Color { get; }
    Task<Move> GetMove(IGame game);
    bool IsAI();
}