namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core.Extensions;

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

    // Castling rights
    private bool _whiteKingSideCastle = true;
    private bool _whiteQueenSideCastle = true;
    private bool _blackKingSideCastle = true;
    private bool _blackQueenSideCastle = true;

    // En passant target square (-1 means no en passant possible)
    private int _enPassantSquare = -1;

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

    private static ulong[] BISHOP_MASKS = new ulong[64];
    private static ulong[] BISHOP_MAGICS = new ulong[64];
    private static int[] BISHOP_SHIFTS = new int[64];
    private static ulong[][] BISHOP_ATTACKS = new ulong[64][];

    /// <summary>
    /// Static precomputations for the bitboard.
    /// </summary>
    static BitBoard() {
        PrecomputeKnightAttacks();
        PrecomputeKingAttacks();
        PrecomputeRookAttacks();
        PrecomputeBishopAttacks();
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
    /// Applies a move to the board. Does not validate - just executes.
    /// </summary>
    public UndoMoveInfo ApplyMove(Move move) {
        int from = move.From.ToBitPosition();
        int to = move.To.ToBitPosition();

        var (color, piece) = GetPieceAtPosition(from);
        var (capturedColor, capturedPiece) = GetPieceAtPosition(to);

        if (color == null || piece == null)
            throw new InvalidOperationException($"No piece at position {from}");

        var undoInfo = CreateUndoInfo(move, color.Value, piece.Value, capturedColor, capturedPiece);

        // Handle en passant capture
        if (piece == PieceType.Pawn && to == _enPassantSquare) {
            undoInfo.WasEnPassant = true;
            var enemyColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            int capturedSquare = color == PieceColor.White ? to - 8 : to + 8;
            RemovePiece(enemyColor, PieceType.Pawn, capturedSquare);
            undoInfo.CapturedType = PieceType.Pawn;
            undoInfo.CapturedColor = enemyColor;
        }

        // Remove captured piece (if any)
        if (capturedColor != null && capturedPiece != null)
            RemovePiece(capturedColor.Value, capturedPiece.Value, to);

        // Move the piece (or place promoted piece)
        RemovePiece(color.Value, piece.Value, from);
        PlacePiece(color.Value, move.PromotedTo ?? piece.Value, to);

        // Handle castling rook movement
        if (piece == PieceType.King && Math.Abs(move.To.Column - move.From.Column) == 2) {
            undoInfo.WasCastling = true;
            ApplyCastlingRookMove(color.Value, move.To.Column > move.From.Column);
        }

        UpdateCastlingRights(color.Value, piece.Value, from, capturedPiece, to);
        UpdateEnPassantSquare(color.Value, piece.Value, from, to);

        return undoInfo;
    }

    private UndoMoveInfo CreateUndoInfo(Move move, PieceColor color, PieceType piece, 
                                         PieceColor? capturedColor, PieceType? capturedPiece) {
        return new UndoMoveInfo {
            Move = move,
            MovedColor = color,
            MovedType = piece,
            CapturedColor = capturedColor,
            CapturedType = capturedPiece,
            WhiteKingSideCastle = _whiteKingSideCastle,
            WhiteQueenSideCastle = _whiteQueenSideCastle,
            BlackKingSideCastle = _blackKingSideCastle,
            BlackQueenSideCastle = _blackQueenSideCastle,
            PreviousEnPassantSquare = _enPassantSquare,
            WasCastling = false,
            WasEnPassant = false
        };
    }

    private void ApplyCastlingRookMove(PieceColor color, bool kingSide) {
        int row = color == PieceColor.White ? 0 : 7;
        int rookFrom = row * 8 + (kingSide ? 7 : 0);
        int rookTo = row * 8 + (kingSide ? 5 : 3);
        RemovePiece(color, PieceType.Rook, rookFrom);
        PlacePiece(color, PieceType.Rook, rookTo);
    }

    private void UpdateCastlingRights(PieceColor color, PieceType piece, int from, PieceType? capturedPiece, int to) {
        // King moved - lose both castling rights
        if (piece == PieceType.King) {
            if (color == PieceColor.White) { _whiteKingSideCastle = false; _whiteQueenSideCastle = false; }
            else { _blackKingSideCastle = false; _blackQueenSideCastle = false; }
        }

        // Rook moved - lose that side's castling right
        if (piece == PieceType.Rook) {
            if (from == 0) _whiteQueenSideCastle = false;
            if (from == 7) _whiteKingSideCastle = false;
            if (from == 56) _blackQueenSideCastle = false;
            if (from == 63) _blackKingSideCastle = false;
        }

        // Rook captured - lose that side's castling right
        if (capturedPiece == PieceType.Rook) {
            if (to == 0) _whiteQueenSideCastle = false;
            if (to == 7) _whiteKingSideCastle = false;
            if (to == 56) _blackQueenSideCastle = false;
            if (to == 63) _blackKingSideCastle = false;
        }
    }

    private void UpdateEnPassantSquare(PieceColor color, PieceType piece, int from, int to) {
        // Set en passant square only on pawn double push
        bool isDoublePush = piece == PieceType.Pawn && Math.Abs(to - from) == 16;
        _enPassantSquare = isDoublePush 
            ? (color == PieceColor.White ? to - 8 : to + 8) 
            : -1;
    }

    /// <summary>
    /// Undoes a move, restoring the board to its previous state.
    /// </summary>
    public void UndoMove(UndoMoveInfo undo) {
        int from = undo.Move.From.ToBitPosition();
        int to = undo.Move.To.ToBitPosition();

        // Restore the moved piece to its original square
        PlacePiece(undo.MovedColor, undo.MovedType, from);
        RemovePiece(undo.MovedColor, undo.Move.PromotedTo ?? undo.MovedType, to);

        // Restore captured piece
        if (undo.CapturedColor != null && undo.CapturedType != null) {
            int captureSquare = undo.WasEnPassant 
                ? (undo.MovedColor == PieceColor.White ? to - 8 : to + 8)
                : to;
            PlacePiece(undo.CapturedColor.Value, undo.CapturedType.Value, captureSquare);
        }

        // Undo castling rook movement
        if (undo.WasCastling) {
            bool kingSide = undo.Move.To.Column > undo.Move.From.Column;
            UndoCastlingRookMove(undo.MovedColor, kingSide);
        }

        // Restore state
        _whiteKingSideCastle = undo.WhiteKingSideCastle;
        _whiteQueenSideCastle = undo.WhiteQueenSideCastle;
        _blackKingSideCastle = undo.BlackKingSideCastle;
        _blackQueenSideCastle = undo.BlackQueenSideCastle;
        _enPassantSquare = undo.PreviousEnPassantSquare;
    }

    private void UndoCastlingRookMove(PieceColor color, bool kingSide) {
        int row = color == PieceColor.White ? 0 : 7;
        int rookFrom = row * 8 + (kingSide ? 7 : 0);
        int rookTo = row * 8 + (kingSide ? 5 : 3);
        RemovePiece(color, PieceType.Rook, rookTo);
        PlacePiece(color, PieceType.Rook, rookFrom);
    }

    public bool IsSquareAttacked(PieceColor color, Position position) {
        var square = position.ToBitPosition();
        var enemyColor = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        
        // Now reuse enemyColor:
        if ((KNIGHT_ATTACKS[square] & _pieces[(int)enemyColor, (int)PieceType.Knight]) != 0)
            return true;
        
        if ((KING_ATTACKS[square] & _pieces[(int)enemyColor, (int)PieceType.King]) != 0)
            return true;
        
        var bishopAttacks = GetBishopAttacks(square, _occupiedSquares);
        if ((bishopAttacks & _pieces[(int)enemyColor, (int)PieceType.Bishop]) != 0)
            return true;
    
        var rookAttacks = GetRookAttacks(square, _occupiedSquares);
        if ((rookAttacks & _pieces[(int)enemyColor, (int)PieceType.Rook]) != 0)
            return true;
        
        var queenAttacks = GetQueenAttacks(square, _occupiedSquares);
        if ((queenAttacks & _pieces[(int)enemyColor, (int)PieceType.Queen]) != 0)
            return true;

        ulong squareBB = 1UL << square;
        ulong enemyPawns = _pieces[(int)enemyColor, (int)PieceType.Pawn];

        if (enemyColor == PieceColor.White) {
            ulong attackerLeft = (squareBB & NOT_A_FILE) >> 9;
            ulong attackerRight = (squareBB & NOT_H_FILE) >> 7;
            if (((attackerLeft | attackerRight) & enemyPawns) != 0)
                return true;
        } else {
            ulong attackerLeft = (squareBB & NOT_A_FILE) << 7;
            ulong attackerRight = (squareBB & NOT_H_FILE) << 9;
            if (((attackerLeft | attackerRight) & enemyPawns) != 0)
                return true;
        }

        return false;
    }

    public bool IsInCheck(PieceColor color) {
        var kingPosition = GetKingPosition(color);

        return IsSquareAttacked(color, kingPosition);
    }

    private Position GetKingPosition(PieceColor color) {
        var king = _pieces[(int)color, (int)PieceType.King];
        var square = BitBoardExtensions.PopLsb(ref king);

        return new Position(square / 8, square % 8);
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
            ROOK_MASKS[i] = RookExtensions.GenerateRookMask(i);

            var blockers = PieceExtensions.GeneratePieceBlockers(ROOK_MASKS[i]);

            GenerateRookAttacks(i, blockers);

            Console.WriteLine($"Finished precomputing rook attacks for square {i}...");
        }
        Console.WriteLine("--------------------------------");
    }

    private static void GenerateRookAttacks(int position, ulong[] blockers) {
        ulong[] attacks = new ulong[blockers.Length];

        for(int j = 0; j < blockers.Length; j++) {
            attacks[j] = RookExtensions.CalculateRookAttacks(position, blockers[j]);
        }

        ROOK_MAGICS[position] = BitBoardExtensions.FindMagicNumber(ROOK_MASKS[position], blockers, attacks);

        int bitCount = BitBoardExtensions.CountBits(ROOK_MASKS[position]);
        ROOK_SHIFTS[position] = 64 - bitCount;
        ROOK_ATTACKS[position] = new ulong[1 << bitCount];

        for (int j = 0; j < blockers.Length; j++) {
            int index = (int)((blockers[j] * ROOK_MAGICS[position]) >> ROOK_SHIFTS[position]);
            ROOK_ATTACKS[position][index] = attacks[j];
        }
    }

    private static void PrecomputeBishopAttacks() {
        for(int i = 0; i < 64; i++) {
            BISHOP_MASKS[i] = BishopExtensions.GenerateBishopMask(i);

            var blockers = PieceExtensions.GeneratePieceBlockers(BISHOP_MASKS[i]);

            GenerateBishopAttacks(i, blockers);

            Console.WriteLine($"Finished precomputing bishop attacks for square {i}...");
        }
        Console.WriteLine("--------------------------------");
    }

    private static void GenerateBishopAttacks(int position, ulong[] blockers) {
        ulong[] attacks = new ulong[blockers.Length];

        for(int j = 0; j < blockers.Length; j++) {
            attacks[j] = BishopExtensions.CalculateBishopAttacks(position, blockers[j]);
        }

        BISHOP_MAGICS[position] = BitBoardExtensions.FindMagicNumber(BISHOP_MASKS[position], blockers, attacks);

        int bitCount = BitBoardExtensions.CountBits(BISHOP_MASKS[position]);
        BISHOP_SHIFTS[position] = 64 - bitCount;
        BISHOP_ATTACKS[position] = new ulong[1 << bitCount];

        for (int j = 0; j < blockers.Length; j++) {
            int index = (int)((blockers[j] * BISHOP_MAGICS[position]) >> BISHOP_SHIFTS[position]);
            BISHOP_ATTACKS[position][index] = attacks[j];
        }
    }

    /// <summary>
    /// Gets the attacks for the bishop at the given position with the given blocker.
    /// </summary>
    /// <param name="position">The position to get the attacks for</param>
    /// <param name="blocker">The blocker to calculate the attacks for</param>
    /// <returns>A bitboard of all squares the bishop can attack</returns>
    private static ulong GetBishopAttacks(int position, ulong blocker) {
        var relevantBlocker = blocker & BISHOP_MASKS[position];

        int index = (int)((relevantBlocker * BISHOP_MAGICS[position]) >> BISHOP_SHIFTS[position]);
        return BISHOP_ATTACKS[position][index];
    }

    /// <summary>
    /// Gets the attacks for the queen at the given position with the given blocker.
    /// </summary>
    /// <param name="position">The position to get the attacks for</param>
    /// <param name="blocker">The blocker to calculate the attacks for</param>
    /// <returns>A bitboard of all squares the queen can attack</returns>
    private static ulong GetQueenAttacks(int position, ulong blocker) {
        return GetBishopAttacks(position, blocker) | GetRookAttacks(position, blocker);
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
        var relevantBlocker = blocker & ROOK_MASKS[square];

        int index = (int)((relevantBlocker * ROOK_MAGICS[square]) >> ROOK_SHIFTS[square]);
        return ROOK_ATTACKS[square][index];
    }

    public List<Move> GenerateMoves(PieceColor color) {
        var moves = new List<Move>(48);

        GeneratePawnMoves(color, moves);
        GenerateKnightMoves(color, moves);
        GenerateBishopMoves(color, moves);
        GenerateRookMoves(color, moves);
        GenerateQueenMoves(color, moves);
        GenerateKingMoves(color, moves);
        
        return moves;
    }

    private void GeneratePawnMoves(PieceColor color, List<Move> moves) {
        var pawns = _pieces[(int)color, (int)PieceType.Pawn];
        var emptySquares = ~_occupiedSquares;

        GetPawnSinglePush(color, pawns, emptySquares, moves);
        GetPawnDoublePush(color, pawns, emptySquares, moves);
        GetPawnAttacks(color, pawns, moves);
        GetPawnPromotions(color, pawns, moves);
        GetEnPassantMoves(color, pawns, moves);
    }

    private void GetEnPassantMoves(PieceColor color, ulong pawns, List<Move> moves) {
        if(_enPassantSquare == -1) return;

        ulong epSquareBB = 1UL << _enPassantSquare;
        
        // Check if any pawn can capture en passant
        if(color == PieceColor.White) {
            // White pawns attack from below (ep square - 7 or ep square - 9)
            ulong leftAttacker = (epSquareBB & NOT_A_FILE) >> 9;
            ulong rightAttacker = (epSquareBB & NOT_H_FILE) >> 7;
            
            if((leftAttacker & pawns) != 0) {
                int fromSquare = _enPassantSquare - 9;
                moves.Add(new Move(new Position(fromSquare / 8, fromSquare % 8), 
                                   new Position(_enPassantSquare / 8, _enPassantSquare % 8)));
            }
            if((rightAttacker & pawns) != 0) {
                int fromSquare = _enPassantSquare - 7;
                moves.Add(new Move(new Position(fromSquare / 8, fromSquare % 8), 
                                   new Position(_enPassantSquare / 8, _enPassantSquare % 8)));
            }
        } else {
            // Black pawns attack from above (ep square + 7 or ep square + 9)
            ulong leftAttacker = (epSquareBB & NOT_H_FILE) << 9;
            ulong rightAttacker = (epSquareBB & NOT_A_FILE) << 7;
            
            if((leftAttacker & pawns) != 0) {
                int fromSquare = _enPassantSquare + 9;
                moves.Add(new Move(new Position(fromSquare / 8, fromSquare % 8), 
                                   new Position(_enPassantSquare / 8, _enPassantSquare % 8)));
            }
            if((rightAttacker & pawns) != 0) {
                int fromSquare = _enPassantSquare + 7;
                moves.Add(new Move(new Position(fromSquare / 8, fromSquare % 8), 
                                   new Position(_enPassantSquare / 8, _enPassantSquare % 8)));
            }
        }
    }

    private void GetPawnPromotions(PieceColor color, ulong pawns, List<Move> moves) {
        var emptySquares = ~_occupiedSquares;
        var enemySquares = color == PieceColor.White ? _blackPieces : _whitePieces;

        // Now check either capture or single push
        var singlePush = (pawns.Shift(color == PieceColor.White ? 8 : -8)) & emptySquares & (color == PieceColor.White ? RANK_8 : RANK_1);

        while(singlePush != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref singlePush);
            int fromSquare = toSquare - (color == PieceColor.White ? 8 : -8);
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            
            moves.Add(new Move(from, to, PieceType.Queen));
            moves.Add(new Move(from, to, PieceType.Rook));
            moves.Add(new Move(from, to, PieceType.Bishop));
            moves.Add(new Move(from, to, PieceType.Knight));
        }

        var leftAttack = pawns & (color == PieceColor.White ? NOT_A_FILE : NOT_H_FILE);
        leftAttack = leftAttack.Shift(color == PieceColor.White ? 7 : -7);
        leftAttack = leftAttack & enemySquares & (color == PieceColor.White ? RANK_8 : RANK_1);

        var rightAttack = pawns & (color == PieceColor.White ? NOT_H_FILE : NOT_A_FILE);
        rightAttack = rightAttack.Shift(color == PieceColor.White ? 9 : -9);
        rightAttack = rightAttack & enemySquares & (color == PieceColor.White ? RANK_8 : RANK_1);

        while(leftAttack != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref leftAttack);
            int fromSquare = toSquare - (color == PieceColor.White ? 7 : -7);
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to, PieceType.Queen));
            moves.Add(new Move(from, to, PieceType.Rook));
            moves.Add(new Move(from, to, PieceType.Bishop));
            moves.Add(new Move(from, to, PieceType.Knight));
        }

        while(rightAttack != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref rightAttack);
            int fromSquare = toSquare - (color == PieceColor.White ? 9 : -9);
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to, PieceType.Queen));
            moves.Add(new Move(from, to, PieceType.Rook));
            moves.Add(new Move(from, to, PieceType.Bishop));
            moves.Add(new Move(from, to, PieceType.Knight));
        }
    }

    private void GetPawnAttacks(PieceColor color, ulong pawns, List<Move> moves) {
        var leftShiftDirection = color == PieceColor.White ? 7 : -7;
        var rightShiftDirection = color == PieceColor.White ? 9 : -9;

        var leftPossibleAttack = pawns;
        var rightPossibleAttack =pawns;
        
        var enemies = color == PieceColor.White ? _blackPieces : _whitePieces;

        if(color == PieceColor.White) {
            leftPossibleAttack = leftPossibleAttack & NOT_A_FILE;
            rightPossibleAttack = rightPossibleAttack & NOT_H_FILE;
        } else {
            leftPossibleAttack = leftPossibleAttack & NOT_H_FILE;
            rightPossibleAttack = rightPossibleAttack & NOT_A_FILE;
        }

        leftPossibleAttack = leftPossibleAttack.Shift(leftShiftDirection) & (color == PieceColor.White ? ~RANK_8 : ~RANK_1);
        rightPossibleAttack = rightPossibleAttack.Shift(rightShiftDirection) & (color == PieceColor.White ? ~RANK_8 : ~RANK_1);

        var leftAttacks = leftPossibleAttack & enemies;
        var rightAttacks = rightPossibleAttack & enemies;

        while(leftAttacks != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref leftAttacks);
            int fromSquare = toSquare - leftShiftDirection;
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to));
        }
        
        while(rightAttacks != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref rightAttacks);
            int fromSquare = toSquare - rightShiftDirection;
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to));
        }
    }

    private void GetPawnSinglePush(PieceColor color, ulong pawns, ulong emptySquares, List<Move> moves) {
        var shiftDirection = color == PieceColor.White ? 8 : -8;
        var singlePush = (pawns.Shift(shiftDirection)) & emptySquares & ~(color == PieceColor.White ? RANK_8 : RANK_1);

        while(singlePush != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref singlePush);
            int fromSquare = toSquare - shiftDirection; 
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to));
        }
    }

    private void GetPawnDoublePush(PieceColor color, ulong pawns, ulong emptySquares, List<Move> moves) {
        var shiftDirection = color == PieceColor.White ? 16 : -16;
        var singleShift = color == PieceColor.White ? 8 : -8;
        var singlePush = pawns.Shift(singleShift) & emptySquares;
        var doublePush = singlePush.Shift(singleShift) & emptySquares & (color == PieceColor.White ? RANK_4 : RANK_5);

        while(doublePush != 0) {
            int toSquare = BitBoardExtensions.PopLsb(ref doublePush);
            int fromSquare = toSquare - shiftDirection;
            var from = new Position(fromSquare / 8, fromSquare % 8);
            var to = new Position(toSquare / 8, toSquare % 8);
            moves.Add(new Move(from, to));
        }
    }

    private void GenerateKnightMoves(PieceColor color, List<Move> moves) {
        var knights = _pieces[(int)color, (int)PieceType.Knight];

        while(knights != 0) {
          int fromSquare = BitBoardExtensions.PopLsb(ref knights);
            ulong attacks = KNIGHT_ATTACKS[fromSquare];
            attacks &= ~(color == PieceColor.White ? _whitePieces : _blackPieces);
            
            while (attacks != 0) {
                int toSquare = BitBoardExtensions.PopLsb(ref attacks);

                var from = new Position(fromSquare / 8, fromSquare % 8);
                var to = new Position(toSquare / 8, toSquare % 8);
                
                moves.Add(new Move(from, to));
            }
        }
    }

    private void GenerateBishopMoves(PieceColor color, List<Move> moves) {
        var bishops = _pieces[(int)color, (int)PieceType.Bishop];

        while(bishops != 0) {
            int fromSquare = BitBoardExtensions.PopLsb(ref bishops);
            ulong attacks = GetBishopAttacks(fromSquare, _occupiedSquares);
            attacks &= ~(color == PieceColor.White ? _whitePieces : _blackPieces);
            
            while(attacks != 0) {
                int toSquare = BitBoardExtensions.PopLsb(ref attacks);

                var from = new Position(fromSquare / 8, fromSquare % 8);
                var to = new Position(toSquare / 8, toSquare % 8);
                
                moves.Add(new Move(from, to));
            }
        }
    }

    private void GenerateRookMoves(PieceColor color, List<Move> moves) {
        var rooks = _pieces[(int)color, (int)PieceType.Rook];

        while(rooks != 0) {
            int fromSquare = BitBoardExtensions.PopLsb(ref rooks);
            ulong attacks = GetRookAttacks(fromSquare, _occupiedSquares);
            attacks &= ~(color == PieceColor.White ? _whitePieces : _blackPieces);
            
            while(attacks != 0) {
                int toSquare = BitBoardExtensions.PopLsb(ref attacks);

                var from = new Position(fromSquare / 8, fromSquare % 8);
                var to = new Position(toSquare / 8, toSquare % 8);
                
                moves.Add(new Move(from, to));
            }
        }
    }

    private void GenerateQueenMoves(PieceColor color, List<Move> moves) {
        var queens = _pieces[(int)color, (int)PieceType.Queen];

        while(queens != 0) {
            int fromSquare = BitBoardExtensions.PopLsb(ref queens);
            ulong attacks = GetQueenAttacks(fromSquare, _occupiedSquares);
            attacks &= ~(color == PieceColor.White ? _whitePieces : _blackPieces);
            
            while(attacks != 0) {
                int toSquare = BitBoardExtensions.PopLsb(ref attacks);

                var from = new Position(fromSquare / 8, fromSquare % 8);
                var to = new Position(toSquare / 8, toSquare % 8);
                
                moves.Add(new Move(from, to));
            }
        }
    }

    private void GenerateKingMoves(PieceColor color, List<Move> moves) {
        var king = _pieces[(int)color, (int)PieceType.King];
        var kingCopy = king;

        while(king != 0) {
            int fromSquare = BitBoardExtensions.PopLsb(ref king);
            ulong attacks = KING_ATTACKS[fromSquare];
            attacks &= ~(color == PieceColor.White ? _whitePieces : _blackPieces);
            
            while(attacks != 0) {
                int toSquare = BitBoardExtensions.PopLsb(ref attacks);

                var from = new Position(fromSquare / 8, fromSquare % 8);
                var to = new Position(toSquare / 8, toSquare % 8);
                
                moves.Add(new Move(from, to));
            }
        }

        // Generate castling moves
        GenerateCastlingMoves(color, kingCopy, moves);
    }

    private void GenerateCastlingMoves(PieceColor color, ulong king, List<Move> moves) {
        if(king == 0) return;
        
        int kingSquare = BitBoardExtensions.PopLsb(ref king);
        var kingPos = new Position(kingSquare / 8, kingSquare % 8);
        
        // Don't castle if in check
        if(IsSquareAttacked(color, kingPos)) return;

        if(color == PieceColor.White) {
            // King side castling (e1 to g1)
            if(_whiteKingSideCastle) {
                // Check squares f1(5) and g1(6) are empty
                if(!IsBitSet(_occupiedSquares, 5) && !IsBitSet(_occupiedSquares, 6)) {
                    // Check squares e1, f1, g1 are not attacked
                    if(!IsSquareAttacked(color, new Position(0, 5)) &&
                       !IsSquareAttacked(color, new Position(0, 6))) {
                        moves.Add(new Move(new Position(0, 4), new Position(0, 6)));
                    }
                }
            }
            // Queen side castling (e1 to c1)
            if(_whiteQueenSideCastle) {
                // Check squares b1(1), c1(2), d1(3) are empty
                if(!IsBitSet(_occupiedSquares, 1) && !IsBitSet(_occupiedSquares, 2) && !IsBitSet(_occupiedSquares, 3)) {
                    // Check squares e1, d1, c1 are not attacked
                    if(!IsSquareAttacked(color, new Position(0, 3)) &&
                       !IsSquareAttacked(color, new Position(0, 2))) {
                        moves.Add(new Move(new Position(0, 4), new Position(0, 2)));
                    }
                }
            }
        } else {
            // King side castling (e8 to g8)
            if(_blackKingSideCastle) {
                // Check squares f8(61) and g8(62) are empty
                if(!IsBitSet(_occupiedSquares, 61) && !IsBitSet(_occupiedSquares, 62)) {
                    // Check squares e8, f8, g8 are not attacked
                    if(!IsSquareAttacked(color, new Position(7, 5)) &&
                       !IsSquareAttacked(color, new Position(7, 6))) {
                        moves.Add(new Move(new Position(7, 4), new Position(7, 6)));
                    }
                }
            }
            // Queen side castling (e8 to c8)
            if(_blackQueenSideCastle) {
                // Check squares b8(57), c8(58), d8(59) are empty
                if(!IsBitSet(_occupiedSquares, 57) && !IsBitSet(_occupiedSquares, 58) && !IsBitSet(_occupiedSquares, 59)) {
                    // Check squares e8, d8, c8 are not attacked
                    if(!IsSquareAttacked(color, new Position(7, 3)) &&
                       !IsSquareAttacked(color, new Position(7, 2))) {
                        moves.Add(new Move(new Position(7, 4), new Position(7, 2)));
                    }
                }
            }
        }
    }

    /// <summary>
    /// For debugging purposes, logs the board in a human readable format.
    /// </summary>
    public void LogBoard() {
        var occupiedSquares = _occupiedSquares;

        Console.WriteLine("\n\nOccupied Squares:");
        LogSquare(occupiedSquares);

        Console.WriteLine("\n\n================================================");
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

    public Piece[,] GetPieces() {
        var pieces = new Piece[8, 8];

        for(int i = 7; i >= 0; i--) {
            for(int j = 7; j >= 0; j--) {
                var piece = GetPieceAtPosition(new Position(i, j));

                if(piece != null) {
                    pieces[i, j] = piece;
                }
            }
        }

        return pieces;
    }

    public List<(Piece, Position)> GetPiecesForColor(PieceColor color) {
        var pieces = new List<(Piece, Position)>();
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                var position = new Position(i, j);
                var piece = GetPieceAtPosition(position);
                if(piece != null && piece.Color == color) {
                    pieces.Add((piece, position));
                }
            }
        }
        return pieces;
    }

    public Piece? GetPieceAtPosition(Position position) {
         var (color, type) = GetPieceAtPosition(position.ToBitPosition());
    
        if (color == null || type == null) 
            return null;
        
        return PieceExtensions.CreatePiece(color.Value, type.Value);
    }

    public IBoard Clone() {
        return new BitBoard {
            _pieces = (ulong[,])_pieces.Clone(),
            _occupiedSquares = _occupiedSquares,
            _whitePieces = _whitePieces,
            _blackPieces = _blackPieces,
            _whiteKingSideCastle = _whiteKingSideCastle,
            _whiteQueenSideCastle = _whiteQueenSideCastle,
            _blackKingSideCastle = _blackKingSideCastle,
            _blackQueenSideCastle = _blackQueenSideCastle,
            _enPassantSquare = _enPassantSquare,
        };
    }

    public void LoadForsythEdwardsNotation(string notation) {
        // Clear all bitboards
        for(int color = 0; color < 2; color++) {
            for(int piece = 0; piece < 6; piece++) {
                _pieces[color, piece] = 0;
            }
        }
        _occupiedSquares = 0;
        _whitePieces = 0;
        _blackPieces = 0;

        var parts = notation.Split(' ');
        var boardPart = parts[0];
        var rows = boardPart.Split('/');
        var castlingRights = parts.Length > 2 ? parts[2] : "KQkq";

        // Parse board position (FEN is from rank 8 to rank 1)
        for(int fenRow = 0; fenRow < 8; fenRow++) {
            var row = rows[fenRow];
            int column = 0;

            foreach(char c in row) {
                if(char.IsDigit(c)) {
                    column += int.Parse(c.ToString());
                } else {
                    var color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                    var pieceType = CharToPieceType(char.ToLower(c));
                    
                    // Convert FEN position to bitboard position
                    // FEN row 0 = rank 8, row 7 = rank 1
                    int rank = 7 - fenRow;
                    int position = rank * 8 + column;
                    
                    PlacePiece(color, pieceType, position);
                    column++;
                }
            }
        }

        // Parse castling rights
        _whiteKingSideCastle = castlingRights.Contains('K');
        _whiteQueenSideCastle = castlingRights.Contains('Q');
        _blackKingSideCastle = castlingRights.Contains('k');
        _blackQueenSideCastle = castlingRights.Contains('q');

        // Parse en passant square
        var enPassantPart = parts.Length > 3 ? parts[3] : "-";
        if(enPassantPart != "-") {
            int file = enPassantPart[0] - 'a';
            int rank = enPassantPart[1] - '1';
            _enPassantSquare = rank * 8 + file;
        } else {
            _enPassantSquare = -1;
        }
    }

    private static PieceType CharToPieceType(char c) {
        return c switch {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new ArgumentException($"Invalid piece character: {c}")
        };
    }
}