using Chess.Programming.Ago.Core;

namespace Chess.Programming.Ago.Game;

public interface IGame {
    IGameVisualizer Visualizer { get; }

    IPlayer? Winner { get; }

    void Start();

    void Visualize();

    void DoMove(Move move);

    IPlayer GetCurrentPlayer();

    List<Position> GetValidMovesForPosition(Position position);

    bool IsFinished();

    Board GetBoard();
}