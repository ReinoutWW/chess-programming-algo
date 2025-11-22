using Chess.Programming.Ago.Core;

namespace Chess.Programming.Ago.Game;

public interface IGame {
    Func<Task>? NextMoveHandler { get; set; }

    IGameVisualizer Visualizer { get; }

    IPlayer? Winner { get; }

    public bool IsChecked(PieceColor color);

    Task Start();

    void Visualize();

    Task DoMove(Move move);

    IPlayer GetCurrentPlayer();

    List<Position> GetValidMovesForPosition(Position position);

    bool IsFinished();

    Board GetBoard();
}