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

    public PieceColor Color => _color;
    public bool AlphaBetaPruningEnabled { get; set; } = true;
    public int SearchDepth { get; set; } = 3;

    public MiniMaxPlayerWithStatistics(PieceColor color, IEvaluationFunction? evaluationFunction = null) {
        _color = color;
        _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
    }

    public bool IsAI() => true;

    public MiniMaxSearchResult? GetLastSearchResult() => _lastSearchResult;

    public async Task<Move> GetMove(IGame game) {
        // Reset counters for new search
        _nodeIdCounter = 0;
        _visitOrderCounter = 0;
        _nodesInVisitOrder = new List<MiniMaxNode>();

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

        var bestMove = possibleValidMoves[Random.Shared.Next(possibleValidMoves.Count)];

        foreach (var move in possibleValidMoves) {
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
                BoardState = CaptureBoardState(copiedGame.GetBoard())
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
                // Mark remaining moves as pruned
                var remainingMoves = possibleValidMoves.SkipWhile(m => m != move).Skip(1).ToList();
                foreach (var prunedMove in remainingMoves) {
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
                        VisitOrder = _visitOrderCounter++
                    };
                    parentNode.Children.Add(prunedNode);
                    _nodesInVisitOrder.Add(prunedNode);
                }
                break;
            }
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

        var bestMove = possibleValidMoves[Random.Shared.Next(possibleValidMoves.Count)];

        foreach (var move in possibleValidMoves) {
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
                BoardState = CaptureBoardState(copiedGame.GetBoard())
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
                // Mark remaining moves as pruned
                var remainingMoves = possibleValidMoves.SkipWhile(m => m != move).Skip(1).ToList();
                foreach (var prunedMove in remainingMoves) {
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
                        VisitOrder = _visitOrderCounter++
                    };
                    parentNode.Children.Add(prunedNode);
                    _nodesInVisitOrder.Add(prunedNode);
                }
                break;
            }
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
