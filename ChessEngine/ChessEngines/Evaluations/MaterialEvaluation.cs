namespace Chess.Programming.Ago.ChessEngines.Evaluations;

using Chess.Programming.Ago.ChessEngines.Extensions;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public class MaterialEvaluation : IEvaluationFunction {
    public int Evaluate(IGame game, PieceColor color) {
        var allBlackPieces = game.GetBoard().GetPiecesForColor(PieceColor.Black);
        var allWhitePieces = game.GetBoard().GetPiecesForColor(PieceColor.White);

        var blackMaterialValue = allBlackPieces.Sum(piece => piece.Item1.GetMaterialValue());
        var whiteMaterialValue = allWhitePieces.Sum(piece => piece.Item1.GetMaterialValue());

        return color == PieceColor.White 
            ? whiteMaterialValue - blackMaterialValue 
            : blackMaterialValue - whiteMaterialValue;
    }
}