namespace Chess.Programming.Ago.ChessEngines.Evaluations;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public interface IEvaluationFunction {
    int Evaluate(IGame game, PieceColor color);
}