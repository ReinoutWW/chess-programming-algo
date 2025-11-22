namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

public static class InsufficientMaterialExtensions {

    public static bool HasInsufficientMaterial(this List<(Piece, Position)> pieces) {
        var onlyKingFails = HasOnlyKing(pieces);
        
        return onlyKingFails || HasOnlyKingAndBishop(pieces) || HasOnlyKingAndKnight(pieces);
    }

    public static bool HasOnlyKing(this List<(Piece, Position)> pieces) {
        return pieces.All(piece => piece.Item1.Type == PieceType.King);
    }

    public static bool HasOnlyKingAndBishop(this List<(Piece, Position)> pieces) {
        return pieces.Count == 2 
            && pieces.All(piece => piece.Item1.Type == PieceType.King || piece.Item1.Type == PieceType.Bishop)
            && pieces.Count(piece => piece.Item1.Type == PieceType.Bishop) == 1;
    }

    public static bool HasOnlyKingAndKnight(this List<(Piece, Position)> pieces) {
        return pieces.Count == 2 
            && pieces.All(piece => piece.Item1.Type == PieceType.King || piece.Item1.Type == PieceType.Knight)
            && pieces.Count(piece => piece.Item1.Type == PieceType.Knight) == 1;
    }
}