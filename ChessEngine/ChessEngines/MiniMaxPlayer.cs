namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.ChessEngines.Extensions;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Optimized MiniMax with alpha-beta pruning using make/unmake pattern.
/// No game cloning - much faster than the naive implementation.
/// </summary>
public class MiniMaxPlayer(PieceColor color, IEvaluationFunction? evaluationFunction = null, int depth = 4) : IPlayer {
    public PieceColor Color => color;
    private readonly IEvaluationFunction _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
    private readonly int _searchDepth = depth;
    private readonly PieceColor _opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
    
    public bool IsAI() => true;
    
    public Task<Move> GetMove(IGame game) {
        var bestMove = Search(game, _searchDepth);
        return Task.FromResult(bestMove);
    }

    private Move Search(IGame game, int depth) {
        var board = game.GetBoard();
        var moves = game.GetAllValidMovesForColor(color);
        
        if (moves.Count == 0) {
            throw new InvalidOperationException("No valid moves available");
        }

        // Order moves for better alpha-beta pruning (captures first)
        moves = OrderMoves(moves, board);

        Move? bestMove = null;
        int bestScore = int.MinValue;
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        foreach (var move in moves) {
            var undo = board.MakeMove(move);
            int score = -Negamax(board, depth - 1, -beta, -alpha, _opponentColor);
            board.UnmakeMove(undo);

            if (score > bestScore) {
                bestScore = score;
                bestMove = move;
            }
            alpha = Math.Max(alpha, score);
        }

        return bestMove ?? moves[0];
    }

    /// <summary>
    /// Negamax with alpha-beta pruning.
    /// Uses the make/unmake pattern - no cloning needed!
    /// </summary>
    private int Negamax(Board board, int depth, int alpha, int beta, PieceColor currentColor) {
        var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        
        // Generate moves for current color
        var moves = GenerateMovesForColor(board, currentColor);
        
        // Check for terminal positions
        if (moves.Count == 0) {
            if (board.IsCheck(currentColor)) {
                return -100000 + (_searchDepth - depth); // Checkmate (prefer faster mates)
            }
            return 0; // Stalemate
        }

        if (depth == 0) {
            return EvaluateForColor(board, currentColor);
        }

        // Order moves for better pruning
        moves = OrderMoves(moves, board);

        int bestScore = int.MinValue;

        foreach (var move in moves) {
            var undo = board.MakeMove(move);
            int score = -Negamax(board, depth - 1, -beta, -alpha, opponentColor);
            board.UnmakeMove(undo);

            bestScore = Math.Max(bestScore, score);
            alpha = Math.Max(alpha, score);
            
            if (alpha >= beta) {
                break; // Beta cutoff
            }
        }

        return bestScore;
    }

    /// <summary>
    /// Generate all legal moves for a color (considers check).
    /// </summary>
    private List<Move> GenerateMovesForColor(Board board, PieceColor colorToMove) {
        var pieces = board.GetPiecesForColor(colorToMove);
        var legalMoves = new List<Move>();

        foreach (var (piece, position) in pieces) {
            var pseudoLegalMoves = piece.GetPossibleMoves(board, position);
            
            foreach (var move in pseudoLegalMoves) {
                // Verify move doesn't leave king in check
                var undo = board.MakeMove(move);
                bool inCheck = board.IsCheck(colorToMove);
                board.UnmakeMove(undo);
                
                if (!inCheck) {
                    legalMoves.Add(move);
                }
            }
        }

        return legalMoves;
    }

    /// <summary>
    /// Order moves for better alpha-beta pruning.
    /// Captures and promotions first, then other moves.
    /// </summary>
    private static List<Move> OrderMoves(List<Move> moves, Board board) {
        return moves
            .OrderByDescending(m => {
                int score = 0;
                var capturedPiece = board.GetPieceAtPosition(m.To);
                
                // MVV-LVA: Most Valuable Victim - Least Valuable Attacker
                if (capturedPiece != null) {
                    var attackingPiece = board.GetPieceAtPosition(m.From);
                    score += capturedPiece.GetMaterialValue() * 10 - (attackingPiece?.GetMaterialValue() ?? 0);
                }
                
                // Promotions are good
                if (m.PromotedTo != null) {
                    score += m.PromotedTo == PieceType.Queen ? 800 : 300;
                }
                
                return score;
            })
            .ToList();
    }

    /// <summary>
    /// Evaluate position from the perspective of the given color.
    /// </summary>
    private int EvaluateForColor(Board board, PieceColor colorToMove) {
        int whiteMaterial = 0;
        int blackMaterial = 0;

        for (int row = 0; row < 8; row++) {
            for (int col = 0; col < 8; col++) {
                var piece = board.GetPieceAtPosition(new Position(row, col));
                if (piece != null) {
                    int value = piece.GetMaterialValue();
                    if (piece.Color == PieceColor.White) {
                        whiteMaterial += value;
                    } else {
                        blackMaterial += value;
                    }
                }
            }
        }

        int score = whiteMaterial - blackMaterial;
        return colorToMove == PieceColor.White ? score : -score;
    }
}