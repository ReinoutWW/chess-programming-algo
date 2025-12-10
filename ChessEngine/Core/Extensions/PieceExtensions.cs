namespace Chess.Programming.Ago.Core.Extensions;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core;

public static class PieceExtensions {
    public static ulong[] GeneratePieceBlockers(ulong mask) {
        int bitCount = BitBoardExtensions.CountBits(mask);
        int numCombinations = 1 << bitCount;  // 2^bitCount

        int[] bitPositions = new int[bitCount];

        int idx = 0;
        for (int i = 0; i < 64; i++) {
            if ((mask & (1UL << i)) != 0) {
                bitPositions[idx] = i;
                idx++;
            }
        }

        ulong[] blockers = new ulong[numCombinations];
        for (int index = 0; index < numCombinations; index++) {
            ulong blocker = 0;
            for (int i = 0; i < bitCount; i++) {
                if ((index & (1 << i)) != 0) {
                    blocker |= 1UL << bitPositions[i];
                }
            }
            blockers[index] = blocker;
        }
        
        return blockers;
    }

    public static Piece CreatePiece(this PieceColor color, PieceType type) {
        return type switch {
            PieceType.Pawn => new Pawn(color),
            PieceType.Knight => new Knight(color),
            PieceType.Bishop => new Bishop(color),
            PieceType.Rook => new Rook(color),
            PieceType.Queen => new Queen(color),
            PieceType.King => new King(color),
            _ => throw new InvalidOperationException("Invalid piece type"),
        };
    }

    /// <summary>
    /// Gets the opponent's color.
    /// </summary>
    public static PieceColor OpponentColor(this PieceColor color) 
        => color == PieceColor.White ? PieceColor.Black : PieceColor.White;
}