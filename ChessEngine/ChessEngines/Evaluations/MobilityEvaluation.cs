namespace Chess.Programming.Ago.ChessEngines.Evaluations;

using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Core;

public class MobilityEvaluation : IEvaluationFunction {
    public int Evaluate(IGame game, PieceColor color) {
        var blackMobility = MobilityForColor(game, PieceColor.Black);
        var whiteMobility = MobilityForColor(game, PieceColor.White);
        
        return color == PieceColor.White 
            ? whiteMobility - blackMobility 
            : blackMobility - whiteMobility;
    }

    public int MobilityForColor(IGame game, PieceColor color) {
        var allPieces = game.GetBoard().GetPiecesForColor(color);
        var mobility = 0;
        foreach(var piece in allPieces) {
            var validMoves = game.GetValidMovesForPosition(piece.Item2);
            mobility += validMoves.Count;
        }
        return mobility;
    }
}