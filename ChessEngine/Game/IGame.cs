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

    List<Move> GetValidMovesForPosition(Position position);

    bool IsFinished();

    Board GetBoard();

    bool IsPawnPromotionMove(Move move);

    List<Move> GetAllValidMovesForColor(PieceColor color);

    string GetGameEndReason();

    IGame Clone(bool simulated = false);

    bool IsDraw();

    Move? GetLastMove();

    public void LoadForsythEdwardsNotation(string notation);

    bool IsValidMove(Move move);
}