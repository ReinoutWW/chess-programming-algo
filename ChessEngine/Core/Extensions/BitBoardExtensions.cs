namespace Chess.Programming.Ago.Core.Extensions;

using System.Numerics;

public static class BitBoardExtensions {
    public static int ToBitPosition(this Position position) {
        return position.Row * 8 + position.Column;
    }

    public static int CountBits(ulong mask) {
        int count = 0;
        while(mask != 0) {
            count += (int)(mask & 1);
            mask >>= 1;
        }
        return count;
    }

    public static ulong FindMagicNumber(ulong mask, ulong[] blockers, ulong[] attacks) {
        int bitCount = CountBits(mask);
        int tableSize = 1 << bitCount;
        var random = new Random();
        
        while (true) {
            ulong candidate = RandomSparseUlong(random);
            
            ulong[] table = new ulong[tableSize];
            bool[] used = new bool[tableSize];
            bool works = true;
            
            for (int i = 0; i < blockers.Length; i++) {

                int index = (int)((blockers[i] * candidate) >> (64 - bitCount));
                
                if (!used[index]) {
                    table[index] = attacks[i];
                    used[index] = true;
                } else if (table[index] != attacks[i]) {
                    works = false;  // Collision!
                    break;
                }
            }
            
            if (works) return candidate;
        }
    }

    private static ulong RandomUlong(Random rand) {
        byte[] buf = new byte[8];
        rand.NextBytes(buf);
        return BitConverter.ToUInt64(buf, 0);
    }

    private static ulong RandomSparseUlong(Random rand) {
        // ANDing 3 randoms keeps only ~1/8 of bits → much better for magics!
        return RandomUlong(rand) & RandomUlong(rand) & RandomUlong(rand);
    }

    public static int PopLsb(ref ulong bitboard) {
        int square = BitOperations.TrailingZeroCount(bitboard);
        bitboard &= bitboard - 1;
        return square;
    }

    public static ulong Shift(this ulong value, int amount) {
        return amount > 0 ? value << amount : value >> -amount;
    }
}