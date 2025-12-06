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
/// </summary>
public class BitBoard : IVisualizedBoard {
    
    private ulong[,]  _pieces = new ulong[2, 6];
    
    // Aggregates
    private ulong _occupiedSquares;
    private ulong _whitePieces;
    private ulong _blackPieces;

    private const ulong NOT_A_FILE = 0xFEFEFEFEFEFEFEFE;
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
    private ulong SetBit(ulong bitboard, int position) {
        return bitboard | (1UL << position);
    }

    /// <summary>
    /// Checks if the bit at the given position in the bitboard is set.
    /// </summary>
    /// <param name="bitboard">The bitboard to check the bit in</param>
    /// <param name="position">The position to check the bit at</param>
    /// <returns>True if the bit is set, false otherwise</returns>
    private bool IsBitSet(ulong bitboard, int position) {
        return (bitboard & (1UL << position)) != 0;
    }

    /// <summary>
    /// Clears the bit at the given position in the bitboard.
    /// </summary>
    /// <param name="bitboard">The bitboard to clear the bit in</param>
    /// <param name="position">The position to clear the bit at</param>
    /// <returns>The bitboard with the bit cleared</returns>
    private ulong ClearBit(ulong bitboard, int position) {
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