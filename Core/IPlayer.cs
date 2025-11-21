namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Game;

public interface IPlayer {

    PieceColor Color { get; }
    Move GetMove(IGame game);

}