namespace Chess.Programming.Ago.ChessEngines.Evaluations;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Combined evaluation function based on the formula:
/// f(p) = 200(K-K') + 9(Q-Q') + 5(R-R') + 3(B-B' + N-N') + 1(P-P')
///        - 0.5(D-D' + S-S' + I-I') + 0.1(M-M')
/// Where:
/// KQRBNP = number of kings, queens, rooks, bishops, knights and pawns
/// D,S,I = doubled, blocked and isolated pawns
/// M = Mobility (the number of legal moves)
/// </summary>
public class MaterialMobilityFunction : IEvaluationFunction {
    public int Evaluate(IGame game, PieceColor color) {
        var allCurrentColorPieces = game.GetBoard().GetPiecesForColor(color);
        var allOtherColorPieces = game.GetBoard().GetPiecesForColor(color == PieceColor.White ? PieceColor.Black : PieceColor.White);

        var materialScore = MaterialScore(allCurrentColorPieces, allOtherColorPieces);
        var mobilityScore = MobilityScore(game, allCurrentColorPieces, allOtherColorPieces);

        return materialScore + mobilityScore;
    }

    private static int MaterialScore(List<(Piece, Position)> allCurrentColorPieces, List<(Piece, Position)> allOtherColorPieces) {
        var materialScore = 0;

        var kingScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.King) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.King)) * 200;
        var queenScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.Queen) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.Queen)) * 9;
        var rookScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.Rook) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.Rook)) * 5;
        var bishopScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.Bishop) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.Bishop)) * 3;
        var knightScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.Knight) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.Knight)) * 3;
        var pawnScore = (allCurrentColorPieces.Count(piece => piece.Item1.Type == PieceType.Pawn) - allOtherColorPieces.Count(piece => piece.Item1.Type == PieceType.Pawn)) * 1;

        materialScore 
            += queenScore 
            + rookScore 
            + bishopScore 
            + knightScore 
            + pawnScore
            + kingScore;

        return materialScore;   
    }

    private static int MobilityScore(IGame game, List<(Piece, Position)> allCurrentColorPieces, List<(Piece, Position)> allOtherColorPieces) {
        var currentColorMobility = allCurrentColorPieces.Sum(piece => game.GetValidMovesForPosition(piece.Item2).Count);
        var otherColorMobility = allOtherColorPieces.Sum(piece => game.GetValidMovesForPosition(piece.Item2).Count);

        var mobilityScore = (int)Math.Round((currentColorMobility - otherColorMobility) * 0.1);

        return mobilityScore;
    }
}