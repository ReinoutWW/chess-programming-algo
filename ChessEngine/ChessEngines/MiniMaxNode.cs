namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Represents a node in the minimax search tree for visualization purposes.
/// Each node captures the state of the algorithm at that point in the search.
/// </summary>
public class MiniMaxNode {
    /// <summary>
    /// Unique identifier for this node.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The move that leads to this node (null for root).
    /// </summary>
    public Move? Move { get; set; }

    /// <summary>
    /// Human-readable notation of the move (e.g., "e2-e4").
    /// </summary>
    public string MoveNotation { get; set; } = string.Empty;

    /// <summary>
    /// Current depth in the search tree (0 = root).
    /// </summary>
    public int Depth { get; set; }

    /// <summary>
    /// True if this is a maximizing node, false if minimizing.
    /// </summary>
    public bool IsMaximizing { get; set; }

    /// <summary>
    /// Alpha value at this node (best score for maximizer).
    /// </summary>
    public int Alpha { get; set; }

    /// <summary>
    /// Beta value at this node (best score for minimizer).
    /// </summary>
    public int Beta { get; set; }

    /// <summary>
    /// The evaluation score computed for this node.
    /// </summary>
    public int? Score { get; set; }

    /// <summary>
    /// True if this branch was pruned by alpha-beta.
    /// </summary>
    public bool IsPruned { get; set; }

    /// <summary>
    /// The reason for pruning (if pruned).
    /// </summary>
    public string? PruneReason { get; set; }

    /// <summary>
    /// Order in which this node was visited during search.
    /// Used for replay animation.
    /// </summary>
    public int VisitOrder { get; set; }

    /// <summary>
    /// Reference to the parent node (null for root).
    /// </summary>
    public MiniMaxNode? Parent { get; set; }

    /// <summary>
    /// Child nodes in the search tree.
    /// </summary>
    public List<MiniMaxNode> Children { get; set; } = new();

    /// <summary>
    /// True if this node contains the best move found.
    /// </summary>
    public bool IsBestMove { get; set; }

    /// <summary>
    /// Board state at this node for visualization.
    /// 8x8 array where each cell contains piece info or null.
    /// </summary>
    public MiniBoardPiece?[,]? BoardState { get; set; }

    /// <summary>
    /// Creates a string representation for debugging.
    /// </summary>
    public override string ToString() {
        var status = IsPruned ? "PRUNED" : (Score.HasValue ? $"Score: {Score}" : "...");
        var type = IsMaximizing ? "MAX" : "MIN";
        return $"[{Id}] {MoveNotation} ({type}) α={Alpha} β={Beta} {status}";
    }
}

/// <summary>
/// Simplified piece representation for mini board visualization.
/// </summary>
public class MiniBoardPiece {
    public PieceType Type { get; set; }
    public PieceColor Color { get; set; }
}
