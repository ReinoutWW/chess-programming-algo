namespace Chess.Programming.Ago.Core.Extensions;

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
}