namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using System.Threading.Tasks;

/// <summary>
/// Minimax player with full statistics and visualization support.
/// Maximize the score of the player, and minimize the score of the opponent.
/// It will consider future moves and the score of the board after the move.
/// Implements IVisualizablePlayer to provide search tree data for visualization.
/// </summary>
public class MiniMaxPlayerWithStatistics : IPlayer, IVisualizablePlayer {
    private readonly PieceColor _color;
    private readonly IEvaluationFunction _evaluationFunction;
    private MiniMaxSearchResult? _lastSearchResult;
    private int _nodeIdCounter;
    private int _visitOrderCounter;
    private List<MiniMaxNode> _nodesInVisitOrder = new();
    
    // Move ordering statistics
    private int _captureMovesFirst;
    private int _promotionMovesFirst;
    private int _cutoffsFromOrderedMoves;
    private int _totalCutoffs;

    public PieceColor Color => _color;
    public bool AlphaBetaPruningEnabled { get; set; } = true;
    public bool MoveOrderingEnabled { get; set; } = true;
    public int SearchDepth { get; set; } = 3;

    public MiniMaxPlayerWithStatistics(PieceColor color, IEvaluationFunction? evaluationFunction = null) {
        _color = color;
        _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
    }

    public bool IsAI() => true;

    public MiniMaxSearchResult? GetLastSearchResult() => _lastSearchResult;

    /// <summary>
    /// Piece values for MVV-LVA move ordering.
    /// Higher value = more valuable piece.
    /// </summary>
    private static readonly Dictionary<Pieces.PieceType, int> PieceValues = new() {
        { Pieces.PieceType.Pawn, 100 },
        { Pieces.PieceType.Knight, 320 },
        { Pieces.PieceType.Bishop, 330 },
        { Pieces.PieceType.Rook, 500 },
        { Pieces.PieceType.Queen, 900 },
        { Pieces.PieceType.King, 20000 }
    };

    /// <summary>
    /// Orders moves to improve alpha-beta pruning efficiency.
    /// Priority: Captures (MVV-LVA) > Promotions > Other moves
    /// </summary>
    private List<(Move move, int priority, string orderReason)> OrderMoves(List<Move> moves, Board board) {
        if (!MoveOrderingEnabled) {
            return moves.Select(m => (m, 0, "unordered")).ToList();
        }

        var orderedMoves = new List<(Move move, int priority, string orderReason)>();

        foreach (var move in moves) {
            int priority = 0;
            string reason = "quiet";

            // Check for capture (MVV-LVA: Most Valuable Victim - Least Valuable Attacker)
            var capturedPiece = board.GetPieceAtPosition(move.To);
            var movingPiece = board.GetPieceAtPosition(move.From);
            
            if (capturedPiece != null && movingPiece != null) {
                // MVV-LVA score: victim value * 10 - attacker value
                // This prioritizes capturing high-value pieces with low-value pieces
                int victimValue = PieceValues.GetValueOrDefault(capturedPiece.Type, 0);
                int attackerValue = PieceValues.GetValueOrDefault(movingPiece.Type, 0);
                priority = victimValue * 10 - attackerValue + 10000; // +10000 to ensure captures come first
                reason = $"capture:{capturedPiece.Type}";
                _captureMovesFirst++;
            }

            // Check for promotion
            if (move.PromotedTo.HasValue) {
                int promotionValue = PieceValues.GetValueOrDefault(move.PromotedTo.Value, 0);
                priority = Math.Max(priority, promotionValue + 5000); // Promotions are very valuable
                reason = $"promote:{move.PromotedTo.Value}";
                _promotionMovesFirst++;
            }

            orderedMoves.Add((move, priority, reason));
        }

        // Sort by priority descending (highest priority first)
        return orderedMoves.OrderByDescending(m => m.priority).ToList();
    }

    public async Task<Move> GetMove(IGame game) {
        // Reset counters for new search
        _nodeIdCounter = 0;
        _visitOrderCounter = 0;
        _nodesInVisitOrder = new List<MiniMaxNode>();
        
        // Reset move ordering statistics
        _captureMovesFirst = 0;
        _promotionMovesFirst = 0;
        _cutoffsFromOrderedMoves = 0;
        _totalCutoffs = 0;

        // Create root node
        var rootNode = new MiniMaxNode {
            Id = _nodeIdCounter++,
            Move = null,
            MoveNotation = "Root",
            Depth = 0,
            IsMaximizing = true,
            Alpha = int.MinValue,
            Beta = int.MaxValue,
            Parent = null,
            VisitOrder = _visitOrderCounter++,
            BoardState = CaptureBoardState(game.GetBoard())
        };
        _nodesInVisitOrder.Add(rootNode);

        // Run minimax and build tree
        var (bestMove, score, _) = await Minimax(
            game, 
            SearchDepth, 
            int.MinValue, 
            int.MaxValue, 
            true, 
            rootNode
        );

        // Update root with final score
        rootNode.Score = score;

        // Mark the best move path
        MarkBestMovePath(rootNode, bestMove);

        // Count pruned nodes
        int prunedCount = CountPrunedNodes(rootNode);

        // Store result for visualization
        _lastSearchResult = new MiniMaxSearchResult {
            BestMove = bestMove,
            RootNode = rootNode,
            TotalNodesEvaluated = _nodesInVisitOrder.Count,
            NodesPruned = prunedCount,
            SearchDepth = SearchDepth,
            AlphaBetaEnabled = AlphaBetaPruningEnabled,
            MoveOrderingEnabled = MoveOrderingEnabled,
            CaptureMovesOrdered = _captureMovesFirst,
            PromotionMovesOrdered = _promotionMovesFirst,
            CutoffsFromOrderedMoves = _cutoffsFromOrderedMoves,
            TotalCutoffs = _totalCutoffs,
            NodesInVisitOrder = _nodesInVisitOrder
        };

        return bestMove;
    }

    private async Task<(Move move, int score, MiniMaxNode node)> Minimax(
        IGame game, 
        int depth, 
        int alpha, 
        int beta, 
        bool maximizingPlayer,
        MiniMaxNode parentNode) {
        
        // Leaf node - evaluate position
        if (depth == 0) {
            var evalScore = _evaluationFunction.Evaluate(game, _color);
            parentNode.Score = evalScore;
            return (null!, evalScore, parentNode);
        }

        if (maximizingPlayer) {
            return await MaximizingSearch(game, depth, alpha, beta, parentNode);
        } else {
            return await MinimizingSearch(game, depth, alpha, beta, parentNode);
        }
    }

    private async Task<(Move move, int score, MiniMaxNode node)> MaximizingSearch(
        IGame game, 
        int depth, 
        int alpha, 
        int beta,
        MiniMaxNode parentNode) {
        
        var possibleValidMoves = game.GetAllValidMovesForColor(_color);
        var maxEval = int.MinValue;

        // Handle terminal states
        if (possibleValidMoves.Count == 0) {
            if (game.IsChecked(_color)) {
                parentNode.Score = int.MinValue; // Checkmate - worst outcome
                return (null!, int.MinValue, parentNode);
            } else {
                parentNode.Score = int.MinValue; // Stalemate - treat as loss to avoid draws
                return (null!, int.MinValue, parentNode);
            }
        } else if (game.IsDraw()) {
            parentNode.Score = int.MinValue; // Treat draws as losses - avoid at all costs
            return (null!, int.MinValue, parentNode);
        }

        // Apply move ordering for better alpha-beta pruning
        var orderedMoves = OrderMoves(possibleValidMoves, game.GetBoard());
        var bestMove = orderedMoves[0].move;
        int moveIndex = 0;

        foreach (var (move, priority, orderReason) in orderedMoves) {
            // Simulate move first to capture board state
            var copiedGame = game.Clone(simulated: true);
            await copiedGame.DoMove(move);

            // Create child node with board state after move
            var childNode = new MiniMaxNode {
                Id = _nodeIdCounter++,
                Move = move,
                MoveNotation = GetMoveNotation(move),
                Depth = parentNode.Depth + 1,
                IsMaximizing = false,
                Alpha = alpha,
                Beta = beta,
                Parent = parentNode,
                VisitOrder = _visitOrderCounter++,
                BoardState = CaptureBoardState(copiedGame.GetBoard()),
                MoveOrderPriority = priority,
                MoveOrderReason = orderReason
            };
            parentNode.Children.Add(childNode);
            _nodesInVisitOrder.Add(childNode);

            // Recurse
            var (_, evalScore, _) = await Minimax(copiedGame, depth - 1, alpha, beta, false, childNode);
            childNode.Score = evalScore;

            if (evalScore > maxEval) {
                maxEval = evalScore;
                bestMove = move;
            }

            alpha = Math.Max(alpha, evalScore);

            // Alpha-beta pruning
            if (AlphaBetaPruningEnabled && beta <= alpha) {
                _totalCutoffs++;
                // Track if cutoff happened due to an ordered (high priority) move
                if (priority > 0) {
                    _cutoffsFromOrderedMoves++;
                }
                
                // Mark remaining moves as pruned
                var remainingMoves = orderedMoves.Skip(moveIndex + 1).ToList();
                foreach (var (prunedMove, prunedPriority, prunedReason) in remainingMoves) {
                    var prunedNode = new MiniMaxNode {
                        Id = _nodeIdCounter++,
                        Move = prunedMove,
                        MoveNotation = GetMoveNotation(prunedMove),
                        Depth = parentNode.Depth + 1,
                        IsMaximizing = false,
                        Alpha = alpha,
                        Beta = beta,
                        Parent = parentNode,
                        IsPruned = true,
                        PruneReason = $"β({beta}) ≤ α({alpha})",
                        VisitOrder = _visitOrderCounter++,
                        MoveOrderPriority = prunedPriority,
                        MoveOrderReason = prunedReason
                    };
                    parentNode.Children.Add(prunedNode);
                    _nodesInVisitOrder.Add(prunedNode);
                }
                break;
            }
            moveIndex++;
        }

        parentNode.Score = maxEval;
        return (bestMove, maxEval, parentNode);
    }

    private async Task<(Move move, int score, MiniMaxNode node)> MinimizingSearch(
        IGame game, 
        int depth, 
        int alpha, 
        int beta,
        MiniMaxNode parentNode) {
        
        var opponentColor = _color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        var possibleValidMoves = game.GetAllValidMovesForColor(opponentColor);
        var minEval = int.MaxValue;

        // Handle terminal states
        if (possibleValidMoves.Count == 0) {
            if (game.IsChecked(opponentColor)) {
                parentNode.Score = int.MaxValue; // Checkmate opponent - best outcome
                return (null!, int.MaxValue, parentNode);
            } else {
                parentNode.Score = int.MinValue; // Stalemate - treat as loss to avoid draws
                return (null!, int.MinValue, parentNode);
            }
        } else if (game.IsDraw()) {
            parentNode.Score = int.MinValue; // Treat draws as losses - avoid at all costs
            return (null!, int.MinValue, parentNode);
        }

        // Apply move ordering for better alpha-beta pruning
        var orderedMoves = OrderMoves(possibleValidMoves, game.GetBoard());
        var bestMove = orderedMoves[0].move;
        int moveIndex = 0;

        foreach (var (move, priority, orderReason) in orderedMoves) {
            // Simulate move first to capture board state
            var copiedGame = game.Clone(simulated: true);
            await copiedGame.DoMove(move);

            // Create child node with board state after move
            var childNode = new MiniMaxNode {
                Id = _nodeIdCounter++,
                Move = move,
                MoveNotation = GetMoveNotation(move),
                Depth = parentNode.Depth + 1,
                IsMaximizing = true,
                Alpha = alpha,
                Beta = beta,
                Parent = parentNode,
                VisitOrder = _visitOrderCounter++,
                BoardState = CaptureBoardState(copiedGame.GetBoard()),
                MoveOrderPriority = priority,
                MoveOrderReason = orderReason
            };
            parentNode.Children.Add(childNode);
            _nodesInVisitOrder.Add(childNode);

            // Recurse
            var (_, evalScore, _) = await Minimax(copiedGame, depth - 1, alpha, beta, true, childNode);
            childNode.Score = evalScore;

            if (evalScore < minEval) {
                minEval = evalScore;
                bestMove = move;
            }

            beta = Math.Min(beta, evalScore);

            // Alpha-beta pruning
            if (AlphaBetaPruningEnabled && beta <= alpha) {
                _totalCutoffs++;
                // Track if cutoff happened due to an ordered (high priority) move
                if (priority > 0) {
                    _cutoffsFromOrderedMoves++;
                }
                
                // Mark remaining moves as pruned
                var remainingMoves = orderedMoves.Skip(moveIndex + 1).ToList();
                foreach (var (prunedMove, prunedPriority, prunedReason) in remainingMoves) {
                    var prunedNode = new MiniMaxNode {
                        Id = _nodeIdCounter++,
                        Move = prunedMove,
                        MoveNotation = GetMoveNotation(prunedMove),
                        Depth = parentNode.Depth + 1,
                        IsMaximizing = true,
                        Alpha = alpha,
                        Beta = beta,
                        Parent = parentNode,
                        IsPruned = true,
                        PruneReason = $"β({beta}) ≤ α({alpha})",
                        VisitOrder = _visitOrderCounter++,
                        MoveOrderPriority = prunedPriority,
                        MoveOrderReason = prunedReason
                    };
                    parentNode.Children.Add(prunedNode);
                    _nodesInVisitOrder.Add(prunedNode);
                }
                break;
            }
            moveIndex++;
        }

        parentNode.Score = minEval;
        return (bestMove, minEval, parentNode);
    }

    private void MarkBestMovePath(MiniMaxNode root, Move bestMove) {
        if (bestMove == null) return;

        // Find the child with the best move
        var bestChild = root.Children.FirstOrDefault(c => 
            c.Move != null && 
            c.Move.From == bestMove.From && 
            c.Move.To == bestMove.To);
        
        if (bestChild != null) {
            bestChild.IsBestMove = true;
        }
    }

    private int CountPrunedNodes(MiniMaxNode node) {
        int count = node.IsPruned ? 1 : 0;
        foreach (var child in node.Children) {
            count += CountPrunedNodes(child);
        }
        return count;
    }

    private string GetMoveNotation(Move move) {
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

    private MiniBoardPiece?[,] CaptureBoardState(Board board) {
        var state = new MiniBoardPiece?[8, 8];
        for (int row = 0; row < 8; row++) {
            for (int col = 0; col < 8; col++) {
                var piece = board.GetPieceAtPosition(new Position(row, col));
                if (piece != null) {
                    state[row, col] = new MiniBoardPiece {
                        Type = piece.Type,
                        Color = piece.Color
                    };
                }
            }
        }
        return state;
    }
}
