namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;

/// <summary>
/// Contains the result of a minimax search, including the best move
/// and the complete search tree for visualization.
/// </summary>
public class MiniMaxSearchResult {
    /// <summary>
    /// The best move found by the search.
    /// </summary>
    public Move BestMove { get; set; } = null!;

    /// <summary>
    /// The root node of the search tree (contains all nodes as children).
    /// </summary>
    public MiniMaxNode RootNode { get; set; } = null!;

    /// <summary>
    /// Total number of nodes evaluated during the search.
    /// </summary>
    public int TotalNodesEvaluated { get; set; }

    /// <summary>
    /// Number of nodes pruned by alpha-beta pruning.
    /// </summary>
    public int NodesPruned { get; set; }

    /// <summary>
    /// The search depth used.
    /// </summary>
    public int SearchDepth { get; set; }

    /// <summary>
    /// Whether alpha-beta pruning was enabled.
    /// </summary>
    public bool AlphaBetaEnabled { get; set; }

    /// <summary>
    /// Flattened list of all nodes in visit order for easy replay.
    /// </summary>
    public List<MiniMaxNode> NodesInVisitOrder { get; set; } = new();
}
