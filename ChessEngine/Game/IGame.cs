namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core;


public interface IGame {
    Func<Task>? NextMoveHandler { get; set; }
    IPlayer? Winner { get; }

    public bool IsChecked(PieceColor color);

    Task Start();

    void Visualize();

    Task DoMove(Move move);

    IPlayer GetCurrentPlayer();

    List<Move> GetValidMovesForPosition(Position position);

    Piece? GetPieceAtPosition(Position position);

    List<Piece> GetCapturedPieces();

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