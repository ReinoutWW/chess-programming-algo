namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.ChessEngines.Ordering;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using System.Diagnostics;

/// <summary>
/// A time-based chess engine that uses iterative deepening.
/// Searches as deep as possible within the given time limit.
/// Uses alpha-beta pruning and move ordering for efficiency.
/// </summary>
public class IterativeDeepeningPlayer : IPlayer {
    private readonly PieceColor _color;
    private readonly IEvaluationFunction _evaluationFunction;
    private readonly IMoveOrdering _moveOrdering;
    private readonly TimeSpan _timeLimit;
    private Stopwatch _stopwatch = null!;
    private bool _timeUp;
    
    // Statistics from last search
    private int _nodesEvaluated;
    private int _depthReached;
    private int _cutoffs;
    
    // Checkmate score constants
    private const int CheckmateScore = 1_000_000;

    public PieceColor Color => _color;

    /// <summary>
    /// Creates a time-based iterative deepening player.
    /// </summary>
    /// <param name="color">The color this player plays as</param>
    /// <param name="timeLimitSeconds">Time limit for each move in seconds (default: 10)</param>
    /// <param name="evaluationFunction">Evaluation function to use (default: MaterialEvaluation)</param>
    /// <param name="moveOrdering">Move ordering strategy to use (default: AggressiveOrdering)</param>
    public IterativeDeepeningPlayer(
        PieceColor color, 
        double timeLimitSeconds = 30.0,
        IEvaluationFunction? evaluationFunction = null,
        IMoveOrdering? moveOrdering = null) {
        _color = color;
        _timeLimit = TimeSpan.FromSeconds(timeLimitSeconds);
        _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
        _moveOrdering = moveOrdering ?? new AggressiveOrdering();
    }

    public bool IsAI() => true;

    /// <summary>
    /// Gets statistics from the last search.
    /// </summary>
    public (int nodesEvaluated, int depthReached, int cutoffs, TimeSpan timeUsed) GetLastSearchStats() 
        => (_nodesEvaluated, _depthReached, _cutoffs, _stopwatch?.Elapsed ?? TimeSpan.Zero);

    public Task<Move> GetMove(IGame game) {
        // Reset statistics
        _nodesEvaluated = 0;
        _depthReached = 0;
        _cutoffs = 0;
        _timeUp = false;
        _stopwatch = Stopwatch.StartNew();

        Move? bestMove = null;
        int bestScore = int.MinValue;
        
        // Start with depth 1 and keep going deeper
        int depth = 1;
        const int maxDepth = 100; // Safety limit
        
        // Get initial moves to have a fallback
        var possibleMoves = game.GetAllValidMovesForColor(_color);
        if (possibleMoves.Count == 0) {
            throw new InvalidOperationException("No valid moves available");
        }
        
        // Default to first move as fallback
        bestMove = possibleMoves[0];

        while (depth <= maxDepth && !_timeUp) {
            var (move, score) = SearchAtDepth(game, depth);
            
            // Only update best move if search completed (not interrupted)
            if (!_timeUp && move != null) {
                bestMove = move;
                bestScore = score;
                _depthReached = depth;
                
                Console.WriteLine($"Depth {depth}: {GetMoveNotation(move)} (score: {score}, nodes: {_nodesEvaluated}, time: {_stopwatch.Elapsed.TotalSeconds:F2}s)");
                
                // If we found a checkmate, no need to search deeper
                if (Math.Abs(score) >= CheckmateScore - 100) {
                    Console.WriteLine($"Checkmate found at depth {depth}!");
                    break;
                }
            }
            
            depth++;
        }

        _stopwatch.Stop();
        Console.WriteLine($"Search complete: depth={_depthReached}, nodes={_nodesEvaluated}, cutoffs={_cutoffs}, time={_stopwatch.Elapsed.TotalSeconds:F2}s");

        return Task.FromResult(bestMove!);
    }

    private (Move? move, int score) SearchAtDepth(IGame game, int depth) {
        return Minimax(game, depth, int.MinValue, int.MaxValue, true);
    }

    private (Move? move, int score) Minimax(IGame game, int depth, int alpha, int beta, bool maximizingPlayer) {
        // Check time limit periodically
        if (_nodesEvaluated % 1000 == 0 && _stopwatch.Elapsed >= _timeLimit) {
            _timeUp = true;
            return (null, 0);
        }

        // Check terminal states
        var currentColor = maximizingPlayer ? _color : GetOpponentColor();
        var possibleMoves = game.GetAllValidMovesForColor(currentColor);
        
        if (possibleMoves.Count == 0) {
            _nodesEvaluated++;
            if (game.IsChecked(currentColor)) {
                // Checkmate - add depth to prefer faster mates
                return maximizingPlayer 
                    ? (null, -CheckmateScore - depth) 
                    : (null, CheckmateScore + depth);
            }
            // Stalemate
            return (null, 0);
        }
        
        if (game.IsDraw()) {
            _nodesEvaluated++;
            return (null, 0);
        }

        // Leaf node - evaluate
        if (depth == 0) {
            _nodesEvaluated++;
            return (null, _evaluationFunction.Evaluate(game, _color));
        }

        // Order moves for better pruning
        var orderedMoves = _moveOrdering.OrderMoves(possibleMoves, game);
        
        if (maximizingPlayer) {
            return MaximizingSearch(game, depth, alpha, beta, orderedMoves);
        } else {
            return MinimizingSearch(game, depth, alpha, beta, orderedMoves);
        }
    }

    private (Move? move, int score) MaximizingSearch(
        IGame game, 
        int depth, 
        int alpha, 
        int beta,
        List<Move> moves) {
        
        var maxEval = int.MinValue;
        Move? bestMove = moves[0];

        foreach (var move in moves) {
            if (_timeUp) break;
            
            var undoInfo = game.DoMoveForSimulation(move);
            var (_, evalScore) = Minimax(game, depth - 1, alpha, beta, false);
            game.UndoMoveForSimulation(undoInfo);

            if (evalScore > maxEval) {
                maxEval = evalScore;
                bestMove = move;
            }

            alpha = Math.Max(alpha, evalScore);
            if (beta <= alpha) {
                _cutoffs++;
                break;
            }
        }

        return (bestMove, maxEval);
    }

    private (Move? move, int score) MinimizingSearch(
        IGame game, 
        int depth, 
        int alpha, 
        int beta,
        List<Move> moves) {
        
        var minEval = int.MaxValue;
        Move? bestMove = moves[0];

        foreach (var move in moves) {
            if (_timeUp) break;
            
            var undoInfo = game.DoMoveForSimulation(move);
            var (_, evalScore) = Minimax(game, depth - 1, alpha, beta, true);
            game.UndoMoveForSimulation(undoInfo);

            if (evalScore < minEval) {
                minEval = evalScore;
                bestMove = move;
            }

            beta = Math.Min(beta, evalScore);
            if (beta <= alpha) {
                _cutoffs++;
                break;
            }
        }

        return (bestMove, minEval);
    }

    private PieceColor GetOpponentColor() 
        => _color == PieceColor.White ? PieceColor.Black : PieceColor.White;

    private static string GetMoveNotation(Move move) {
        var files = "abcdefgh";
        var fromFile = files[move.From.Column];
        var fromRank = 8 - move.From.Row;
        var toFile = files[move.To.Column];
        var toRank = 8 - move.To.Row;
        
        var notation = $"{fromFile}{fromRank}-{toFile}{toRank}";
        
        if (move.PromotedTo.HasValue) {
            var pieceChar = move.PromotedTo.Value switch {
                Pieces.PieceType.Queen => "Q",
                Pieces.PieceType.Rook => "R",
                Pieces.PieceType.Bishop => "B",
                Pieces.PieceType.Knight => "N",
                _ => ""
            };
            notation += $"={pieceChar}";
        }
        
        return notation;
    }
}

