namespace Chess.Programming.Ago.ChessEngines;

/// <summary>
/// Interface for AI players that can provide visualization data
/// about their decision-making process.
/// </summary>
public interface IVisualizablePlayer {
    /// <summary>
    /// Gets the result of the last search performed, including
    /// the complete search tree for visualization.
    /// Returns null if no search has been performed yet.
    /// </summary>
    MiniMaxSearchResult? GetLastSearchResult();

    /// <summary>
    /// Gets or sets whether alpha-beta pruning is enabled.
    /// When disabled, the full tree is explored without pruning.
    /// </summary>
    bool AlphaBetaPruningEnabled { get; set; }

    /// <summary>
    /// Gets or sets whether move ordering is enabled.
    /// Move ordering improves alpha-beta pruning by evaluating likely good moves first.
    /// </summary>
    bool MoveOrderingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the search depth.
    /// </summary>
    int SearchDepth { get; set; }
}
