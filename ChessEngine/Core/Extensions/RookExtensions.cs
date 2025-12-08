namespace Chess.Programming.Ago.Core.Extensions;

public static class RookExtensions {

    /// <summary>
    /// Calculates the attacks for the rook at the given position with the given blocker.
    /// </summary>
    /// <param name="position">The position to calculate the attacks for</param>
    /// <param name="blocker">The blocker to calculate the attacks for</param>
    /// <returns>A bitboard of all squares the rook can attack</returns>
    public static ulong CalculateRookAttacks(int position, ulong blocker) {
        var attacks = 0UL;

        int rank = position / 8;
        int file = position % 8;

        for (int r = rank + 1; r <= 7; r++) {
            int pos = r * 8 + file;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        for (int r = rank - 1; r >= 0; r--) {
            int pos = r * 8 + file;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        for (int f = file + 1; f <= 7; f++) {
            int pos = rank * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        for (int f = file - 1; f >= 0; f--) {
            int pos = rank * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        return attacks;
    }

    /// <summary>
    /// Generates a mask for the rook at the given position.
    /// This is used to generate the attacks for the rook.
    /// </summary>
    /// <param name="position">The position to generate the mask for</param>
    /// <returns>A bitboard of all squares the rook can attack</returns>
    public static ulong GenerateRookMask(int position) {
        var mask = 0UL;
        int rank = position / 8;
        int file = position % 8;

        for (int r = rank + 1; r < 7; r++) {
            mask |= 1UL << (r * 8 + file);
        }

        for (int r = rank - 1; r > 0; r--) {
            mask |= 1UL << (r * 8 + file);
        }

        for (int f = file + 1; f < 7; f++) {
            mask |= 1UL << (rank * 8 + f);
        }

        for (int f = file - 1; f > 0; f--) {
            mask |= 1UL << (rank * 8 + f);
        }
        
        return mask;
    }
}