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
    /// Whether move ordering was enabled.
    /// </summary>
    public bool MoveOrderingEnabled { get; set; }

    /// <summary>
    /// Number of capture moves that were ordered first.
    /// </summary>
    public int CaptureMovesOrdered { get; set; }

    /// <summary>
    /// Number of promotion moves that were ordered first.
    /// </summary>
    public int PromotionMovesOrdered { get; set; }

    /// <summary>
    /// Number of cutoffs that occurred due to ordered moves (captures/promotions).
    /// </summary>
    public int CutoffsFromOrderedMoves { get; set; }

    /// <summary>
    /// Total number of alpha-beta cutoffs.
    /// </summary>
    public int TotalCutoffs { get; set; }

    /// <summary>
    /// Percentage of cutoffs caused by move ordering (0-100).
    /// </summary>
    public double MoveOrderingEffectiveness => 
        TotalCutoffs > 0 ? Math.Round((double)CutoffsFromOrderedMoves / TotalCutoffs * 100, 1) : 0;

    /// <summary>
    /// Flattened list of all nodes in visit order for easy replay.
    /// </summary>
    public List<MiniMaxNode> NodesInVisitOrder { get; set; } = new();
}
