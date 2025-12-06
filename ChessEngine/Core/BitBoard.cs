namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core.Extensions;
using System.Net;
using System.Resources;

/// <summary>
/// A bitboard implementation of the chess board.
/// This is a 64 bit integer that represents the board.
/// The board class is responsible for:
/// 1. Store the position in the form of bitboards
/// 2. Apply / Undo moves
/// 3. Generate pseudo-legal moves
/// 4. "Is square attacked" checks
/// 5. "What piece is on square x" checks
/// 
/// Some special features in this code:
/// 1. Uses bitboards to represent the board, which is a more efficient way to represent the board than a 2D array.
/// 2. Precomputes the knight and king attacks for all positions so in the future, a lookup is O(1) time complexity.
/// 3. Magic bitboards are used to generate the attacks for the rooks and bishops.
/// </summary>
public class BitBoard : IVisualizedBoard {
    
    private ulong[,]  _pieces = new ulong[2, 6];
    
    // Aggregates
    private ulong _occupiedSquares;
    private ulong _whitePieces;
    private ulong _blackPieces;

    private const ulong NOT_A_FILE = 0xFEFEFEFEFEFEFEFE;
    private const ulong NOT_B_FILE = 0xFDFDFDFDFDFDFDFD;
    private const ulong NOT_G_FILE = 0xBFBFBFBFBFBFBFBF;
    private const ulong NOT_H_FILE = 0x7F7F7F7F7F7F7F7F;

    // Rank masks (for double push logic)
    private const ulong RANK_1 = 0x00000000000000FF;
    private const ulong RANK_2 = 0x000000000000FF00;
    private const ulong RANK_3 = 0x0000000000FF0000;
    private const ulong RANK_4 = 0x00000000FF000000;
    private const ulong RANK_5 = 0x000000FF00000000;
    private const ulong RANK_6 = 0x0000FF0000000000;
    private const ulong RANK_7 = 0x00FF000000000000;
    private const ulong RANK_8 = 0xFF00000000000000;

    private static ulong[] KNIGHT_ATTACKS = new ulong[64];
    private static ulong[] KING_ATTACKS = new ulong[64];

    private static ulong[] ROOK_MASKS = new ulong[64];
    private static ulong[] ROOK_MAGICS = new ulong[64];
    private static int[] ROOK_SHIFTS = new int[64];
    private static ulong[][] ROOK_ATTACKS = new ulong[64][];

    static BitBoard() {
        PrecomputeKnightAttacks();
        PrecomputeKingAttacks();
        PrecomputeRookAttacks();
    }

    public BitBoard() {
        _occupiedSquares = 0;

        _pieces[(int)PieceColor.White, (int)PieceType.Pawn] = 0xff00;
        _pieces[(int)PieceColor.White, (int)PieceType.Knight] = 0x42;
        _pieces[(int)PieceColor.White, (int)PieceType.Bishop] = 0x24;
        _pieces[(int)PieceColor.White, (int)PieceType.Rook] = 0x81;
        _pieces[(int)PieceColor.White, (int)PieceType.Queen] = 0x08;
        _pieces[(int)PieceColor.White, (int)PieceType.King] = 0x10;

        _pieces[(int)PieceColor.Black, (int)PieceType.Pawn] = 0xff000000000000;
        _pieces[(int)PieceColor.Black, (int)PieceType.Knight] = 0x4200000000000000;
        _pieces[(int)PieceColor.Black, (int)PieceType.Bishop] = 0x2400000000000000;
        _pieces[(int)PieceColor.Black, (int)PieceType.Rook] = 0x8100000000000000;
        _pieces[(int)PieceColor.Black, (int)PieceType.Queen] = 0x0800000000000000;
        _pieces[(int)PieceColor.Black, (int)PieceType.King] = 0x1000000000000000;

        _whitePieces = _pieces[(int)PieceColor.White, (int)PieceType.Pawn] | _pieces[(int)PieceColor.White, (int)PieceType.Knight] | _pieces[(int)PieceColor.White, (int)PieceType.Bishop] | _pieces[(int)PieceColor.White, (int)PieceType.Rook] | _pieces[(int)PieceColor.White, (int)PieceType.Queen] | _pieces[(int)PieceColor.White, (int)PieceType.King];
        _blackPieces = _pieces[(int)PieceColor.Black, (int)PieceType.Pawn] | _pieces[(int)PieceColor.Black, (int)PieceType.Knight] | _pieces[(int)PieceColor.Black, (int)PieceType.Bishop] | _pieces[(int)PieceColor.Black, (int)PieceType.Rook] | _pieces[(int)PieceColor.Black, (int)PieceType.Queen] | _pieces[(int)PieceColor.Black, (int)PieceType.King];
        
        _occupiedSquares = _whitePieces | _blackPieces;
    }

    /// <summary>
    /// Sets the bit at the given position in the bitboard.
    /// </summary>
    /// <param name="bitboard">The bitboard to set the bit in</param>
    /// <param name="position">The position to set the bit at</param>
    /// <returns>The bitboard with the bit set</returns>
    private static ulong SetBit(ulong bitboard, int position) {
        return bitboard | (1UL << position);
    }

    /// <summary>
    /// Checks if the bit at the given position in the bitboard is set.
    /// </summary>
    /// <param name="bitboard">The bitboard to check the bit in</param>
    /// <param name="position">The position to check the bit at</param>
    /// <returns>True if the bit is set, false otherwise</returns>
    private static bool IsBitSet(ulong bitboard, int position) {
        return (bitboard & (1UL << position)) != 0;
    }

    /// <summary>
    /// Clears the bit at the given position in the bitboard.
    /// </summary>
    /// <param name="bitboard">The bitboard to clear the bit in</param>
    /// <param name="position">The position to clear the bit at</param>
    /// <returns>The bitboard with the bit cleared</returns>
    private static ulong ClearBit(ulong bitboard, int position) {
        return bitboard & ~(1UL << position);
    }

    /// <summary>
    /// Should:
    /// 1. Update specific piece bitboard
    /// 2. Update occupied squares bitboard
    /// 3. Update white/black pieces bitboards
    /// </summary>
    /// <param name="color">Color of the piece to place</param>
    /// <param name="type">Type of the piece to place</param>
    /// <param name="position">Position to place the piece</param>
    private void PlacePiece(PieceColor color, PieceType type, int position) {
        _pieces[(int)color, (int)type] = SetBit(_pieces[(int)color, (int)type], position);
        _occupiedSquares = SetBit(_occupiedSquares, position);

        if(color == PieceColor.White) {
            _whitePieces = SetBit(_whitePieces, position);
        } else {
            _blackPieces = SetBit(_blackPieces, position);
        }
    }

    /// <summary>
    /// Should:
    /// 1. Update specific piece bitboard
    /// 2. Update occupied squares bitboard
    /// 3. Update white/black pieces bitboards
    /// </summary>
    /// <param name="color">Color of the piece to remove</param>
    /// <param name="type">Type of the piece to remove</param>
    /// <param name="position">Position to remove the piece</param>
    private void RemovePiece(PieceColor color, PieceType type, int position) {
        _pieces[(int)color, (int)type] = ClearBit(_pieces[(int)color, (int)type], position);
        _occupiedSquares = ClearBit(_occupiedSquares, position);

        if(color == PieceColor.White) {
            _whitePieces = ClearBit(_whitePieces, position);
        } else {
            _blackPieces = ClearBit(_blackPieces, position);
        }
    }

    /// <summary>
    /// Applies a move to the board.
    /// This will not check if the move is valid, it will just apply it.
    /// If the move is a capture, the captured piece will be removed from the board.
    /// </summary>
    /// <param name="move">The move to apply</param>
    /// <exception cref="InvalidOperationException">Thrown if there is no piece at the from position</exception>
    /// <exception cref="InvalidOperationException">Thrown if there no at the to from position</exception>
    public void ApplyMove(Move move) {
        var fromPosition = move.From.ToBitPosition();
        var toPosition = move.To.ToBitPosition();

        var (fromColor, fromType) = GetPieceAtPosition(fromPosition);
        var (toColor, toType) = GetPieceAtPosition(toPosition);

        if(fromColor == null || fromType == null) {
            throw new InvalidOperationException($"No piece at position {fromPosition}");
        }

        if(toColor != null && toType != null) {
            RemovePiece(toColor.Value, toType.Value, toPosition);
        }

        RemovePiece(fromColor.Value, fromType.Value, fromPosition);
        PlacePiece(fromColor.Value, fromType.Value, toPosition);
    }

    /// <summary>
    /// This will do 8 bit checks to get the piece at the position.
    /// First: 
    /// 1. Determine the color of the piece
    /// 2. Determine the type of the piece
    /// 3. Return the piece
    /// </summary>
    /// <param name="position"></param>
    /// <exception cref="InvalidOperationException">Thrown if there is no piece at the position but a color was found</exception>
    /// <returns>The color and type of the piece at the position, or null if there is no piece at the position</returns>
    private (PieceColor? color, PieceType? type) GetPieceAtPosition(int position) {
        var isWhite = IsBitSet(_whitePieces, position);
        var isBlack = IsBitSet(_blackPieces, position);

        PieceColor? color = isWhite ? PieceColor.White : isBlack ? PieceColor.Black : null;

        if(color == null) {
            return (null, null);
        }

        PieceType? type = IsBitSet(_pieces[(int)color, (int)PieceType.Pawn], position) ? PieceType.Pawn :
                    IsBitSet(_pieces[(int)color, (int)PieceType.Knight], position) ? PieceType.Knight :
                    IsBitSet(_pieces[(int)color, (int)PieceType.Bishop], position) ? PieceType.Bishop :
                    IsBitSet(_pieces[(int)color, (int)PieceType.Rook], position) ? PieceType.Rook :
                    IsBitSet(_pieces[(int)color, (int)PieceType.Queen], position) ? PieceType.Queen :
                    IsBitSet(_pieces[(int)color, (int)PieceType.King], position) ? PieceType.King
                    : throw new InvalidOperationException($"No valid piece at position {position} for color {color}");

        return (color, type);
    }

    /// <summary>
    /// Precomputes the knight attacks for all positions so in the future, a lookup is O(1) time complexity.
    /// </summary>
    private static void PrecomputeKnightAttacks() {
        for(int i = 0; i < 64; i++) {
            KNIGHT_ATTACKS[i] = GetKnightAttacks(i);
        }
    }

    /// <summary>
    /// Gets the knight attacks for a given position.
    /// </summary>
    /// <param name="position">The position to get the knight attacks for</param>
    /// <returns>A bitboard of all squares the knight can attack</returns>
    private static ulong GetKnightAttacks(int position) {
        var knight = 1UL << position;

        var NOT_AB_FILE = NOT_A_FILE & NOT_B_FILE;
        var NOT_GH_FILE = NOT_G_FILE & NOT_H_FILE;

        var possibleMoveFour = (knight & NOT_A_FILE) << 15;
        var possibleMoveFive = (knight & NOT_A_FILE) >> 17;
        var possibleMoveEight = (knight & NOT_H_FILE) >> 15;
        var possibleMoveOne = (knight & NOT_H_FILE) << 17;
        var possibleMoveSix = (knight & NOT_AB_FILE) >> 10;
        var possibleMoveThree = (knight & NOT_AB_FILE) << 6;
        var possibleMoveTwo = (knight & NOT_GH_FILE) << 10;
        var possibleMoveSeven = (knight & NOT_GH_FILE) >> 6;

        return possibleMoveOne 
            | possibleMoveTwo 
            | possibleMoveThree 
            | possibleMoveFour 
            | possibleMoveFive 
            | possibleMoveSix 
            | possibleMoveSeven 
            | possibleMoveEight;
    }

    /// <summary>
    /// Precomputes the king attacks for all positions so in the future, a lookup is O(1) time complexity.
    /// </summary>
    private static void PrecomputeKingAttacks() {
        for(int i = 0; i < 64; i++) {
            KING_ATTACKS[i] = GetKingAttacks(i);
        }
    }

    /// <summary>
    /// Gets the king attacks for a given position.
    /// </summary>
    /// <param name="position">The position to get the king attacks for</param>
    /// <returns>A bitboard of all squares the king can attack</returns>
    private static ulong GetKingAttacks(int position) {
        var king = 1UL << position;
   
        var possibleMoveOne = (king & NOT_H_FILE) << 1;
        var possibleMoveTwo = (king & NOT_A_FILE) >> 1;
        var possibleMoveThree = (king & NOT_H_FILE) << 9;
        var possibleMoveFive = (king & NOT_A_FILE) >> 9;
        var possibleMoveSix = (king & NOT_A_FILE) << 7;
        var possibleMoveSeven = (king & NOT_H_FILE) >> 7;
        var possibleMoveFour = (king) >> 8;
        var possibleMoveEight = (king) << 8;

        return possibleMoveOne 
            | possibleMoveTwo 
            | possibleMoveThree 
            | possibleMoveFour 
            | possibleMoveFive 
            | possibleMoveSix 
            | possibleMoveSeven 
            | possibleMoveEight;
    }


    /// <summary>
    /// Returns a bitboard of all squares white pawns can push to.
    /// </summary>
    public ulong GetWhitePawnPushes() {
        var pawns = _pieces[(int)PieceColor.White, (int)PieceType.Pawn];
        var emptySquares = ~_occupiedSquares;
        var singlePush = (pawns << 8) & emptySquares;
        var doublePush = (singlePush << 8) & emptySquares & RANK_4;

        return singlePush | doublePush;
    }

    /// <summary>
    /// Returns a bitboard of all squares white pawns ATTACK (not necessarily capture).
    /// </summary>
    public ulong GetWhitePawnAttacks() {
        var pawns = _pieces[(int)PieceColor.White, (int)PieceType.Pawn];
        
        var leftAttacks = (pawns & NOT_A_FILE) << 7;
        var rightAttacks = (pawns & NOT_H_FILE) << 9;
        
        return leftAttacks | rightAttacks;
    }

    /// <summary>
    /// Returns a bitboard of all squares black pawns can push to.
    /// </summary>
    public ulong GetBlackPawnPushes() {
        var pawns = _pieces[(int)PieceColor.Black, (int)PieceType.Pawn];

        var emptySquares = ~_occupiedSquares;
        var singlePush = (pawns >> 8) & emptySquares;
        var doublePush = (singlePush >> 8) & emptySquares & RANK_5;

        return singlePush | doublePush;
    }

    /// <summary>
    /// Returns a bitboard of all squares black pawns ATTACK (not necessarily capture).
    /// </summary>
    public ulong GetBlackPawnAttacks() {
        var pawns = _pieces[(int)PieceColor.Black, (int)PieceType.Pawn];
        
        var leftAttacks = (pawns & NOT_H_FILE) >> 7;
        var rightAttacks = (pawns & NOT_A_FILE) >> 9;
        
        return leftAttacks | rightAttacks;
    }

    /// <summary>
    /// Precomputes the attacks for all rooks so in the future, a lookup is O(1) time complexity.
    /// </summary>
    /// <remarks>
    /// We use a mask to generate the attacks for the rook.
    /// We then use a blocker to calculate the attacks for the rook.
    /// We then use a lookup table to store the attacks for the rook.
    /// </remarks>
    private static void PrecomputeRookAttacks() {
        Console.WriteLine("Precomputing rook attacks...");

        for(int i = 0; i < 64; i++) {
            ROOK_MASKS[i] = GenerateRookMask(i);

            var blockers = GenerateRookBlockers(i);

            ulong[] attacks = new ulong[blockers.Length];

            for(int j = 0; j < blockers.Length; j++) {
                attacks[j] = CalculateRookAttacks(i, blockers[j]);
            }

            ROOK_MAGICS[i] = FindMagicNumber(i, blockers, attacks);

            int bitCount = CountBits(ROOK_MASKS[i]);
            ROOK_SHIFTS[i] = 64 - bitCount;
            ROOK_ATTACKS[i] = new ulong[1 << bitCount];

            for (int j = 0; j < blockers.Length; j++) {
                int index = (int)((blockers[j] * ROOK_MAGICS[i]) >> ROOK_SHIFTS[i]);
                ROOK_ATTACKS[i][index] = attacks[j];
            }

            Console.WriteLine($"Finished precomputing rook attacks for square {i}...");
        }
        Console.WriteLine("--------------------------------");
    }

    /// <summary>
    /// Simple, isn't it?
    /// We use the magic number to index into the lookup table.
    /// The blocker is used to calculate the index.
    /// </summary>
    /// <param name="square">The square to get the attacks for</param>
    /// <param name="blocker">The blocker to calculate the index for</param>
    /// <returns>A bitboard of all squares the rook can attack</returns>
    private static ulong GetRookAttacks(int square, ulong blocker) {
        int index = (int)((blocker * ROOK_MAGICS[square]) >> ROOK_SHIFTS[square]);
        return ROOK_ATTACKS[square][index];
    }

    private static ulong FindMagicNumber(int square, ulong[] blockers, ulong[] attacks) {
        int bitCount = CountBits(ROOK_MASKS[square]);
        int tableSize = 1 << bitCount;
        var random = new Random();
        
        while (true) {
            ulong candidate = RandomSparseUlong(random);
            
            ulong[] table = new ulong[tableSize];
            bool works = true;
            
            for (int i = 0; i < blockers.Length; i++) {

                int index = (int)((blockers[i] * candidate) >> (64 - bitCount));
                
                if (table[index] == 0) {
                    table[index] = attacks[i];
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

    /// <summary>
    /// Calculates the attacks for the rook at the given position with the given blocker.
    /// </summary>
    /// <param name="position">The position to calculate the attacks for</param>
    /// <param name="blocker">The blocker to calculate the attacks for</param>
    /// <returns>A bitboard of all squares the rook can attack</returns>
    private static ulong CalculateRookAttacks(int position, ulong blocker) {
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
    private static ulong GenerateRookMask(int position) {
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

    /// <summary>
    /// Generate all possible blockers for the rook at the given position.
    /// We use binary to calculate the amount of combinations of blockers.
    /// </summary>
    /// <param name="position">The position to generate the blockers for</param>
    /// <returns>An array of all possible blockers</returns>
    private static ulong[] GenerateRookBlockers(int position) {
        var mask = ROOK_MASKS[position];

        int bitCount = CountBits(mask);
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

    private static int CountBits(ulong mask) {
        int count = 0;
        while(mask != 0) {
            count += (int)(mask & 1);
            mask >>= 1;
        }
        return count;
    }

    /// <summary>
    /// For debugging purposes, logs the board in a human readable format.
    /// </summary>
    public void LogBoard() {
        var occupiedSquares = _occupiedSquares;

        Console.WriteLine("\n\nOccupied Squares:");
        LogSquare(occupiedSquares);

        Console.WriteLine("\n\nBlack Pieces:");
        LogSquare(_blackPieces);

        Console.WriteLine("\n\nBlack Pawn Pushes:");
        LogSquare(GetBlackPawnPushes());

        for(int i = 0; i < 64; i++) {
            Console.WriteLine("\n\nKnight Attacks:");
            var king = 1UL << i;

            // Add knight as well (index)
            var kingAttacks = KING_ATTACKS[i] | king;
            
            LogSquare(kingAttacks);
        }

        Console.WriteLine("\n\n Rook mask for position 27:");
        LogSquare(GenerateRookMask(27));

        ulong blocker = 1UL << 35;
        blocker |= 1UL << 29;

        Console.WriteLine("\n\nBlocker for position 27 (0UL <<  28 + 8):");
        LogSquare(blocker);

        // Test rook attacks
        Console.WriteLine("\n\nRook attacks:");
        LogSquare(GetRookAttacks(27, blocker));
    }

    private void LogSquare(ulong bitboard) {
        Console.WriteLine("    A  B  C  D  E  F  G  H");
        Console.WriteLine("   -------------------------");
        for(int i = 0; i < 8; i++) {
            Console.Write($"{8 - i} |");
            for(int j = 0; j < 8; j++) {
                Console.Write(IsBitSet(bitboard, (7 - i) * 8 + j) ? " x " : "   ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("   -------------------------");
        Console.WriteLine("    A  B  C  D  E  F  G  H");
    }
}