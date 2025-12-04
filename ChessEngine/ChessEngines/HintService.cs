namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

/// <summary>
/// Service for calculating move hints for human players.
/// Uses MiniMaxPlayerWithStatistics to provide both the best move and search statistics.
/// </summary>
public class HintService {
    private readonly IEvaluationFunction _evaluationFunction;
    private MiniMaxSearchResult? _lastHintResult;

    public int SearchDepth { get; set; } = 3;
    public bool AlphaBetaPruningEnabled { get; set; } = true;

    public HintService(IEvaluationFunction? evaluationFunction = null) {
        _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
    }

    /// <summary>
    /// Gets the last calculated hint result, including the search tree for visualization.
    /// </summary>
    public MiniMaxSearchResult? GetLastHintResult() => _lastHintResult;

    /// <summary>
    /// Calculates the best move hint for the current player.
    /// </summary>
    /// <param name="game">The current game state</param>
    /// <returns>A HintResult containing the suggested move and evaluation details</returns>
    public async Task<HintResult> GetHint(IGame game) {
        var currentColor = game.GetCurrentPlayer().Color;
        
        // Create a temporary statistics player to calculate the hint
        var analysisPlayer = new MiniMaxPlayerWithStatistics(currentColor, _evaluationFunction) {
            AlphaBetaPruningEnabled = AlphaBetaPruningEnabled,
            SearchDepth = SearchDepth
        };

        // Clone the game to avoid any state modifications
        var clonedGame = game.Clone(simulated: true);
        
        // Calculate the best move
        var bestMove = await analysisPlayer.GetMove(clonedGame);
        
        // Store the search result for visualization
        _lastHintResult = analysisPlayer.GetLastSearchResult();

        return new HintResult {
            SuggestedMove = bestMove,
            SearchResult = _lastHintResult,
            ForColor = currentColor
        };
    }

    /// <summary>
    /// Clears the cached hint result.
    /// </summary>
    public void ClearHint() {
        _lastHintResult = null;
    }
}

/// <summary>
/// Contains the result of a hint calculation.
/// </summary>
public class HintResult {
    /// <summary>
    /// The suggested best move.
    /// </summary>
    public Move? SuggestedMove { get; init; }
    
    /// <summary>
    /// The full search result including the tree for visualization.
    /// </summary>
    public MiniMaxSearchResult? SearchResult { get; init; }
    
    /// <summary>
    /// The color for which this hint was calculated.
    /// </summary>
    public PieceColor ForColor { get; init; }
    
    /// <summary>
    /// The evaluation score of the suggested move.
    /// </summary>
    public int Score => SearchResult?.RootNode?.Score ?? 0;
    
    /// <summary>
    /// Human-readable description of the suggested move.
    /// </summary>
    public string MoveDescription {
        get {
            if (SuggestedMove == null) return "No move available";
            
            var files = "abcdefgh";
            var fromFile = files[SuggestedMove.From.Column];
            var fromRank = 8 - SuggestedMove.From.Row;
            var toFile = files[SuggestedMove.To.Column];
            var toRank = 8 - SuggestedMove.To.Row;
            
            var notation = $"{fromFile}{fromRank} → {toFile}{toRank}";
            
            if (SuggestedMove.PromotedTo.HasValue) {
                var pieceChar = SuggestedMove.PromotedTo.Value switch {
                    Pieces.PieceType.Queen => "Q",
                    Pieces.PieceType.Rook => "R",
                    Pieces.PieceType.Bishop => "B",
                    Pieces.PieceType.Knight => "N",
                    _ => ""
                };
                notation += $" (promote to {pieceChar})";
            }
            
            return notation;
        }
    }
}
