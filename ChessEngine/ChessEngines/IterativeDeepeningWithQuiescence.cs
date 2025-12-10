namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.ChessEngines.Ordering;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;
using System.Diagnostics;

/// <summary>
/// Advanced iterative deepening player with Quiescence Search.
/// Quiescence search continues at leaf nodes until the position is "quiet" (no captures),
/// preventing the horizon effect where the engine misses obvious tactics.
/// </summary>
public class IterativeDeepeningWithQuiescence : IPlayer {
    private readonly PieceColor _color;
    private readonly IEvaluationFunction _evaluationFunction;
    private readonly IMoveOrdering _moveOrdering;
    private readonly TimeSpan _timeLimit;
    private Stopwatch _stopwatch = null!;
    private bool _timeUp;
    
    // Statistics
    private int _nodesEvaluated;
    private int _quiescenceNodes;
    private int _depthReached;
    private int _cutoffs;
    
    // Killer moves - moves that caused beta cutoffs at each depth
    private Move?[] _killerMoves = new Move?[64]; // Max depth 64
    
    private const int CheckmateScore = 1_000_000;
    private const int MaxQuiescenceDepth = 10; // Limit quiescence search depth

    private static readonly Dictionary<PieceType, int> PieceValues = new() {
        { PieceType.Pawn, 100 },
        { PieceType.Knight, 320 },
        { PieceType.Bishop, 330 },
        { PieceType.Rook, 500 },
        { PieceType.Queen, 900 },
        { PieceType.King, 20000 }
    };

    public PieceColor Color => _color;

    public IterativeDeepeningWithQuiescence(
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

    public (int nodesEvaluated, int quiescenceNodes, int depthReached, int cutoffs, TimeSpan timeUsed) GetLastSearchStats() 
        => (_nodesEvaluated, _quiescenceNodes, _depthReached, _cutoffs, _stopwatch?.Elapsed ?? TimeSpan.Zero);

    public Task<Move> GetMove(IGame game) {
        _nodesEvaluated = 0;
        _quiescenceNodes = 0;
        _depthReached = 0;
        _cutoffs = 0;
        _timeUp = false;
        _stopwatch = Stopwatch.StartNew();
        Array.Clear(_killerMoves);

        Move? bestMove = null;
        int bestScore = int.MinValue;
        
        var possibleMoves = game.GetAllValidMovesForColor(_color);
        if (possibleMoves.Count == 0) {
            throw new InvalidOperationException("No valid moves available");
        }
        
        bestMove = possibleMoves[0];
        int depth = 1;
        const int maxDepth = 100;

        while (depth <= maxDepth && !_timeUp) {
            var (move, score) = SearchAtDepth(game, depth);
            
            if (!_timeUp && move != null) {
                bestMove = move;
                bestScore = score;
                _depthReached = depth;
                
                Console.WriteLine($"Depth {depth}: {GetMoveNotation(move)} (score: {score}, nodes: {_nodesEvaluated}, qnodes: {_quiescenceNodes}, time: {_stopwatch.Elapsed.TotalSeconds:F2}s)");
                
                if (Math.Abs(score) >= CheckmateScore - 100) {
                    Console.WriteLine($"Checkmate found at depth {depth}!");
                    break;
                }
            }
            
            depth++;
        }

        _stopwatch.Stop();
        Console.WriteLine($"Search complete: depth={_depthReached}, nodes={_nodesEvaluated}, qnodes={_quiescenceNodes}, cutoffs={_cutoffs}, time={_stopwatch.Elapsed.TotalSeconds:F2}s");

        return Task.FromResult(bestMove!);
    }

    private (Move? move, int score) SearchAtDepth(IGame game, int depth) {
        return Minimax(game, depth, int.MinValue, int.MaxValue, true, 0);
    }

    private (Move? move, int score) Minimax(IGame game, int depth, int alpha, int beta, bool maximizingPlayer, int ply) {
        if (_nodesEvaluated % 1000 == 0 && _stopwatch.Elapsed >= _timeLimit) {
            _timeUp = true;
            return (null, 0);
        }

        var currentColor = maximizingPlayer ? _color : GetOpponentColor();
        var possibleMoves = game.GetAllValidMovesForColor(currentColor);
        
        // Terminal states
        if (possibleMoves.Count == 0) {
            _nodesEvaluated++;
            if (game.IsChecked(currentColor)) {
                return maximizingPlayer 
                    ? (null, -CheckmateScore - depth) 
                    : (null, CheckmateScore + depth);
            }
            return (null, 0); // Stalemate
        }
        
        if (game.IsDraw()) {
            _nodesEvaluated++;
            return (null, 0);
        }

        // At depth 0, do quiescence search instead of static eval
        if (depth == 0) {
            int qScore = QuiescenceSearch(game, alpha, beta, maximizingPlayer, 0);
            return (null, qScore);
        }

        // Order moves with killer move bonus
        var orderedMoves = OrderMovesWithKillers(possibleMoves, game, ply);
        
        if (maximizingPlayer) {
            return MaximizingSearch(game, depth, alpha, beta, orderedMoves, ply);
        } else {
            return MinimizingSearch(game, depth, alpha, beta, orderedMoves, ply);
        }
    }

    /// <summary>
    /// Quiescence Search - continue searching captures until position is quiet
    /// </summary>
    private int QuiescenceSearch(IGame game, int alpha, int beta, bool maximizingPlayer, int qDepth) {
        _quiescenceNodes++;
        
        // Check time
        if (_quiescenceNodes % 500 == 0 && _stopwatch.Elapsed >= _timeLimit) {
            _timeUp = true;
            return 0;
        }

        // Stand-pat: evaluate the current position
        int standPat = _evaluationFunction.Evaluate(game, _color);
        
        if (maximizingPlayer) {
            if (standPat >= beta) return beta;
            if (standPat > alpha) alpha = standPat;
        } else {
            if (standPat <= alpha) return alpha;
            if (standPat < beta) beta = standPat;
        }

        // Limit quiescence depth
        if (qDepth >= MaxQuiescenceDepth) {
            return standPat;
        }

        var currentColor = maximizingPlayer ? _color : GetOpponentColor();
        var allMoves = game.GetAllValidMovesForColor(currentColor);
        
        // Only search captures and promotions (tactical moves)
        var tacticalMoves = allMoves.Where(m => 
            game.GetPieceAtPosition(m.To) != null || // Capture
            m.PromotedTo.HasValue                     // Promotion
        ).ToList();

        if (tacticalMoves.Count == 0) {
            return standPat;
        }

        // Order captures by MVV-LVA
        var orderedCaptures = OrderCaptures(tacticalMoves, game);

        if (maximizingPlayer) {
            foreach (var move in orderedCaptures) {
                if (_timeUp) break;
                
                var undoInfo = game.DoMoveForSimulation(move);
                int score = QuiescenceSearch(game, alpha, beta, false, qDepth + 1);
                game.UndoMoveForSimulation(undoInfo);

                if (score > alpha) alpha = score;
                if (alpha >= beta) {
                    _cutoffs++;
                    break;
                }
            }
            return alpha;
        } else {
            foreach (var move in orderedCaptures) {
                if (_timeUp) break;
                
                var undoInfo = game.DoMoveForSimulation(move);
                int score = QuiescenceSearch(game, alpha, beta, true, qDepth + 1);
                game.UndoMoveForSimulation(undoInfo);

                if (score < beta) beta = score;
                if (alpha >= beta) {
                    _cutoffs++;
                    break;
                }
            }
            return beta;
        }
    }

    private List<Move> OrderCaptures(List<Move> moves, IGame game) {
        return moves
            .Select(move => {
                int priority = 0;
                var capturedPiece = game.GetPieceAtPosition(move.To);
                var movingPiece = game.GetPieceAtPosition(move.From);
                
                if (capturedPiece != null && movingPiece != null) {
                    priority = PieceValues.GetValueOrDefault(capturedPiece.Type, 0) * 10 
                             - PieceValues.GetValueOrDefault(movingPiece.Type, 0);
                }
                
                if (move.PromotedTo.HasValue) {
                    priority += PieceValues.GetValueOrDefault(move.PromotedTo.Value, 0);
                }
                
                return (move, priority);
            })
            .OrderByDescending(x => x.priority)
            .Select(x => x.move)
            .ToList();
    }

    private List<Move> OrderMovesWithKillers(List<Move> moves, IGame game, int ply) {
        var orderedMoves = _moveOrdering.OrderMoves(moves, game);
        
        // Boost killer move to front if it exists and is legal
        if (ply < _killerMoves.Length && _killerMoves[ply] != null) {
            var killer = _killerMoves[ply];
            int killerIndex = orderedMoves.FindIndex(m => 
                m.From.Row == killer!.From.Row && m.From.Column == killer.From.Column &&
                m.To.Row == killer.To.Row && m.To.Column == killer.To.Column);
            
            if (killerIndex > 0) {
                var killerMove = orderedMoves[killerIndex];
                orderedMoves.RemoveAt(killerIndex);
                orderedMoves.Insert(0, killerMove);
            }
        }
        
        return orderedMoves;
    }

    private void StoreKillerMove(Move move, int ply) {
        if (ply < _killerMoves.Length) {
            _killerMoves[ply] = move;
        }
    }

    private (Move? move, int score) MaximizingSearch(IGame game, int depth, int alpha, int beta, List<Move> moves, int ply) {
        var maxEval = int.MinValue;
        Move? bestMove = moves[0];

        foreach (var move in moves) {
            if (_timeUp) break;
            
            var undoInfo = game.DoMoveForSimulation(move);
            var (_, evalScore) = Minimax(game, depth - 1, alpha, beta, false, ply + 1);
            game.UndoMoveForSimulation(undoInfo);

            if (evalScore > maxEval) {
                maxEval = evalScore;
                bestMove = move;
            }

            alpha = Math.Max(alpha, evalScore);
            if (beta <= alpha) {
                _cutoffs++;
                StoreKillerMove(move, ply);
                break;
            }
        }

        return (bestMove, maxEval);
    }

    private (Move? move, int score) MinimizingSearch(IGame game, int depth, int alpha, int beta, List<Move> moves, int ply) {
        var minEval = int.MaxValue;
        Move? bestMove = moves[0];

        foreach (var move in moves) {
            if (_timeUp) break;
            
            var undoInfo = game.DoMoveForSimulation(move);
            var (_, evalScore) = Minimax(game, depth - 1, alpha, beta, true, ply + 1);
            game.UndoMoveForSimulation(undoInfo);

            if (evalScore < minEval) {
                minEval = evalScore;
                bestMove = move;
            }

            beta = Math.Min(beta, evalScore);
            if (beta <= alpha) {
                _cutoffs++;
                StoreKillerMove(move, ply);
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
                PieceType.Queen => "Q",
                PieceType.Rook => "R",
                PieceType.Bishop => "B",
                PieceType.Knight => "N",
                _ => ""
            };
            notation += $"={pieceChar}";
        }
        
        return notation;
    }
}

