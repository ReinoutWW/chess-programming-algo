namespace Chess.Programming.Ago.Core.Extensions;

public static class BishopExtensions {

    public static ulong GenerateBishopMask(int position) {
        var mask = 0UL;

        // Example position: 27 (D4)
    
        int rank = position / 8;
        int file = position % 8;

        for(int r = rank + 1, f = file + 1; r < 7 && f < 7; r++, f++) {
            int pos = r * 8 + f;
            mask |= 1UL << pos;
        }

        for(int r = rank + 1, f = file - 1; r < 7 && f > 0; r++, f--) {
            int pos = r * 8 + f;
            mask |= 1UL << pos;
        }

        for(int r = rank - 1, f = file + 1; r > 0 && f < 7; r--, f++) {
            int pos = r * 8 + f;
            mask |= 1UL << pos;
        }

        for(int r = rank - 1, f = file - 1; r > 0 && f > 0; r--, f--) {
            int pos = r * 8 + f;
            mask |= 1UL << pos;
        }

        return mask;
    }

    public static ulong CalculateBishopAttacks(int position, ulong blocker) {
        var attacks = 0UL;

        int rank = position / 8;
        int file = position % 8;

        for(int r = rank + 1, f = file + 1; r <= 7 && f <= 7; r++, f++) {
            int pos = r * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }
        
        for(int r = rank + 1, f = file - 1; r <= 7 && f >= 0; r++, f--) {
            int pos = r * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        for(int r = rank - 1, f = file + 1; r >= 0 && f <= 7; r--, f++) {
            int pos = r * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        for(int r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--) {
            int pos = r * 8 + f;
            attacks |= 1UL << pos;
            if ((blocker & (1UL << pos)) != 0) break;
        }

        return attacks;
    }


}