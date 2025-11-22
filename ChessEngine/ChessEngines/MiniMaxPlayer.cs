namespace Chess.Programming.Ago.ChessEngines;

using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Chess.Programming.Ago.ChessEngines.Extensions;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;


/// <summary>
/// Maximize the score of the player, and minimize the score of the opponent.
/// It will consider future moves and the score of the board after the move.
/// </summary>
/// <param name="color"></param>
public class MiniMaxPlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public bool IsAI() => true;
    public async Task<Move> GetMove(IGame game) {
        var bestMove = await Minimax(game, 3, int.MinValue, int.MaxValue, true);
        return bestMove.move;
    }

    // Building the tree of moves and scores
    private async Task<(Move move, int score)> Minimax(IGame game, int depth, int alpha, int beta, bool maximizingPlayer) {        
        if(depth == 0) {
            return (null, EvaluateBoardState(game, color));
        } else {
            // Get all valid moves
            if(maximizingPlayer) {
                // For each move, build the tree of moves and scores
                var possibleValidMoves = game.GetAllValidMovesForColor(color);
                var maxEval = int.MinValue;

                if(possibleValidMoves.Count == 0) {
                     if(game.IsChecked(color)) {
                        return (null, -10000);
                    } else {
                        return (null, 0);
                    }
                }

                var bestMove = possibleValidMoves.First();

                foreach(var move in possibleValidMoves) {
                    var copiedGame = game.Clone(simulated: true);
                    await copiedGame.DoMove(move);
                    var eval = await Minimax(copiedGame, depth - 1, alpha, beta, false);
                    
                    if(eval.score > maxEval) {
                        maxEval = eval.score;
                        bestMove = move;
                    }

                    alpha = Math.Max(alpha, eval.score);
                    if(beta <= alpha) {
                        break;
                    }
                }
                return (bestMove, maxEval);
            } else {
                var opponentColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
                var possibleValidMoves = game.GetAllValidMovesForColor(opponentColor);
                var minEval = int.MaxValue;

                if(possibleValidMoves.Count == 0) {
                    if(game.IsChecked(opponentColor)) {
                        return (null, 10000);
                    } else {
                        return (null, 0);
                    }
                }
                var bestMove = possibleValidMoves.First();

                foreach(var move in possibleValidMoves) {
                    var copiedGame = game.Clone(simulated: true);
                    await copiedGame.DoMove(move);
                    var eval = await Minimax(copiedGame, depth - 1, alpha, beta, true);
                    
                    if(eval.score < minEval) {
                        minEval = eval.score;
                        bestMove = move;
                    }
                    
                    beta = Math.Min(beta, eval.score);
                    if(beta <= alpha) {
                        break;
                    }
                }

                return (bestMove, minEval);
            }
        }
    }

    // Evaluating the board state

    private int EvaluateBoardState(IGame game, PieceColor color) {
        var allBlackPieces = game.GetBoard().GetPiecesForColor(PieceColor.Black);
        var allWhitePieces = game.GetBoard().GetPiecesForColor(PieceColor.White);

        var blackMaterialValue = allBlackPieces.Sum(piece => piece.Item1.GetMaterialValue());
        var whiteMaterialValue = allWhitePieces.Sum(piece => piece.Item1.GetMaterialValue());

        return color == PieceColor.White 
            ? whiteMaterialValue - blackMaterialValue 
            : blackMaterialValue - whiteMaterialValue;
    }
}