namespace Chess.Programming.Ago.ChessEngines.Extensions;

using Chess.Programming.Ago.Pieces;

public static class PieceExtensions {

    public static int GetMaterialValue(this Piece piece) {
        return piece.Type switch {
            PieceType.Pawn => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 100,
            _ => 0,
        };
    }
}