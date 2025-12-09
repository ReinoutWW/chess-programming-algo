namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using System.Threading.Tasks;
using Chess.Programming.Ago.ChessEngines.Evaluations;
using Chess.Programming.Ago.ChessEngines.Ordering;

public class MiniMaxPlayerOrdering(PieceColor color, IEvaluationFunction evaluationFunction) : IPlayer {
    public PieceColor Color => color;
    private readonly IEvaluationFunction _evaluationFunction = evaluationFunction ?? new MaterialEvaluation();
    private readonly IMoveOrdering _moveOrdering = new MVVLVAOrdering();
    public bool IsAI() => true;
    public Task<Move> GetMove(IGame game) {
        var bestMove = Minimax(game, 5, int.MinValue, int.MaxValue, true);
        return Task.FromResult(bestMove.move);
    }

    // Building the tree of moves and scores
    private (Move move, int score) Minimax(IGame game, int depth, int alpha, int beta, bool maximizingPlayer) {        
        if(depth == 0) {
            return (null, _evaluationFunction.Evaluate(game, color));
        } else {
            // Get all valid moves
            if(maximizingPlayer)
            {
                return Maximize(game, depth, ref alpha, beta);
            }
            else
            {
                return Minimize(game, depth, alpha, ref beta);
            }
        }
    }

    private (Move move, int score) Minimize(IGame game, int depth, int alpha, ref int beta)
    {
        var opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        var possibleValidMoves = game.GetAllValidMovesForColor(opponentColor);
        possibleValidMoves = OrderMoves(possibleValidMoves, game);
        var minEval = int.MaxValue;

        if (possibleValidMoves.Count == 0)
        {
            if (game.IsChecked(opponentColor))
            {
                return (null, int.MaxValue);
            }
            else
            {
                return (null, int.MinValue);
            }
        }
        else if (game.IsDraw())
        {
            return (null, int.MinValue);
        }

        var bestMove = possibleValidMoves.Any()
            ? possibleValidMoves[Random.Shared.Next(possibleValidMoves.Count)]
            : possibleValidMoves.First();

        foreach (var move in possibleValidMoves)
        {
            var undoInfo = game.DoMoveForSimulation(move);
            var eval = Minimax(game, depth - 1, alpha, beta, true);
            game.UndoMoveForSimulation(undoInfo);

            if (eval.score < minEval)
            {
                minEval = eval.score;
                bestMove = move;
            }

            beta = Math.Min(beta, eval.score);
            if (beta <= alpha)
            {
                break;
            }
        }

        return (bestMove, minEval);
    }

    private (Move move, int score) Maximize(IGame game, int depth, ref int alpha, int beta)
    {
        // For each move, build the tree of moves and scores
        var possibleValidMoves = game.GetAllValidMovesForColor(color);
        possibleValidMoves = OrderMoves(possibleValidMoves, game);
        var maxEval = int.MinValue;

        if (possibleValidMoves.Count == 0)
        {
            if (game.IsChecked(color))
            {
                return (null, int.MinValue);
            }
            else
            {
                return (null, int.MaxValue);
            }
        }
        else if (game.IsDraw())
        {
            return (null, int.MinValue);
        }

        var bestMove = possibleValidMoves.Any()
            ? possibleValidMoves[Random.Shared.Next(possibleValidMoves.Count)]
            : possibleValidMoves.First();

        foreach (var move in possibleValidMoves)
        {
            var undoInfo = game.DoMoveForSimulation(move);
            var eval = Minimax(game, depth - 1, alpha, beta, false);
            game.UndoMoveForSimulation(undoInfo);

            if (eval.score > maxEval)
            {
                maxEval = eval.score;
                bestMove = move;
            }

            alpha = Math.Max(alpha, eval.score);
            if (beta <= alpha)
            {
                break;
            }
        }
        return (bestMove, maxEval);
    }

    /// <summary>
    /// Orders the moves based on the killer-first moves.
    /// </summary>
    /// <param name="moves">The list of moves to order.</param>
    /// <param name="game">The game to use to find the killer-first moves.</param>
    /// <returns>The ordered list of moves.</returns>
    private List<Move> OrderMoves(List<Move> moves, IGame game) {
        return _moveOrdering.OrderMoves(moves, game);
    }
}