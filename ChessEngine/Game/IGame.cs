namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core;


public interface IGame {
    Func<Task>? NextMoveHandler { get; set; }
    Func<Task>? BeforeAIMoveHandler { get; set; }
    IPlayer? Winner { get; }

    public bool IsChecked(PieceColor color);

    Task Start();

    void Visualize();

    Task DoMove(Move move);

    UndoMoveInfo DoMoveForSimulation(Move move);
    void UndoMoveForSimulation(UndoMoveInfo undoInfo);

    IPlayer GetCurrentPlayer();

    List<Move> GetValidMovesForPosition(Position position);

    Piece? GetPieceAtPosition(Position position);

    List<Piece> GetCapturedPieces();

    List<(Piece, Position)> GetPiecesForColor(PieceColor color);

    bool IsFinished();

    bool IsPawnPromotionMove(Move move);

    List<Move> GetAllValidMovesForColor(PieceColor color);

    string GetGameEndReason();

    IGame Clone(bool simulated = false);

    bool IsDraw();

    Move? GetLastMove();

    public void LoadForsythEdwardsNotation(string notation);

    bool IsValidMove(Move move);
    
    /// <summary>
    /// Gets the visualized board for bitboard visualization purposes.
    /// Returns null if the game doesn't use a visualizable board.
    /// </summary>
    IVisualizedBoard? GetVisualizedBoard();
}