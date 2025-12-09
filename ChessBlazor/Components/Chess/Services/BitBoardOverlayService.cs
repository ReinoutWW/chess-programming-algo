using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

namespace ChessBlazor.Components.Chess.Services;

/// <summary>
/// Service to manage bitboard overlay state and refresh logic.
/// Handles tracking of which overlay is active and refreshing it when the board changes.
/// </summary>
public class BitBoardOverlayService
{
    // Current overlay state
    public ulong? ActiveBitboard { get; private set; }
    public string ActiveBitboardName { get; private set; } = "";
    public MagicBitboardInfo? CurrentMagicInfo { get; private set; }
    public bool AttackModeEnabled { get; set; }

    // Overlay type tracking for refresh
    private string? _overlayType;
    private Position? _overlayPiecePosition;
    private Piece? _overlayPiece;

    public event Action? OnStateChanged;

    /// <summary>
    /// Sets the active bitboard overlay based on a predefined type.
    /// </summary>
    public void SetOverlay(ulong bitboard, string name, string overlayType)
    {
        ActiveBitboard = bitboard;
        ActiveBitboardName = name;
        CurrentMagicInfo = null;
        _overlayType = overlayType;
        _overlayPiecePosition = null;
        _overlayPiece = null;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Sets the active bitboard overlay for a piece's attack pattern.
    /// </summary>
    public void SetPieceAttackOverlay(ulong attacks, string name, Position position, Piece piece, MagicBitboardInfo? magicInfo = null)
    {
        ActiveBitboard = attacks;
        ActiveBitboardName = name;
        CurrentMagicInfo = magicInfo;
        _overlayType = $"pieceattacks:{piece.Type}:{piece.Color}";
        _overlayPiecePosition = position;
        _overlayPiece = piece;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Clears the current overlay.
    /// </summary>
    public void Clear()
    {
        ActiveBitboard = null;
        ActiveBitboardName = "";
        CurrentMagicInfo = null;
        _overlayType = null;
        _overlayPiecePosition = null;
        _overlayPiece = null;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Refreshes the current overlay based on the new board state.
    /// Should be called after each move.
    /// </summary>
    public void Refresh(IVisualizedBoard? board, Func<Position, Piece?> getPieceAtPosition)
    {
        if (_overlayType == null || !ActiveBitboard.HasValue || board == null) return;

        switch (_overlayType)
        {
            case "occupied":
                ActiveBitboard = board.OccupiedSquares;
                break;
            case "white":
                ActiveBitboard = board.WhitePieces;
                break;
            case "black":
                ActiveBitboard = board.BlackPieces;
                break;
            case "allattacks:White":
                ActiveBitboard = board.GetAllAttacksForColor(PieceColor.White);
                break;
            case "allattacks:Black":
                ActiveBitboard = board.GetAllAttacksForColor(PieceColor.Black);
                break;
            default:
                if (_overlayType.StartsWith("piece:"))
                {
                    RefreshPieceBitboard(board);
                }
                else if (_overlayType.StartsWith("pieceattacks:") && _overlayPiecePosition != null)
                {
                    RefreshPieceAttacks(board, getPieceAtPosition);
                }
                break;
        }
        
        OnStateChanged?.Invoke();
    }

    private void RefreshPieceBitboard(IVisualizedBoard board)
    {
        // e.g., "piece:W Knight" or "piece:B Pawn"
        var parts = _overlayType!.Substring(6).Split(' ');
        if (parts.Length == 2)
        {
            var color = parts[0] == "W" ? PieceColor.White : PieceColor.Black;
            if (Enum.TryParse<PieceType>(parts[1], out var pieceType))
            {
                ActiveBitboard = board.GetPieceBitboard(color, pieceType);
            }
        }
    }

    private void RefreshPieceAttacks(IVisualizedBoard board, Func<Position, Piece?> getPieceAtPosition)
    {
        var currentPiece = getPieceAtPosition(_overlayPiecePosition!);
        
        if (currentPiece != null && _overlayPiece != null &&
            currentPiece.Type == _overlayPiece.Type && currentPiece.Color == _overlayPiece.Color)
        {
            // Piece still there, refresh attacks
            int square = _overlayPiecePosition!.Row * 8 + _overlayPiecePosition.Column;
            
            switch (currentPiece.Type)
            {
                case PieceType.Rook:
                    ActiveBitboard = board.GetRookAttacksForSquare(square);
                    CurrentMagicInfo = board.GetRookMagicInfo(square);
                    break;
                case PieceType.Bishop:
                    ActiveBitboard = board.GetBishopAttacksForSquare(square);
                    CurrentMagicInfo = board.GetBishopMagicInfo(square);
                    break;
                case PieceType.Queen:
                    ActiveBitboard = board.GetQueenAttacksForSquare(square);
                    CurrentMagicInfo = null;
                    break;
                case PieceType.Knight:
                    ActiveBitboard = BitBoard.GetKnightAttacksForSquare(square);
                    CurrentMagicInfo = null;
                    break;
                case PieceType.King:
                    ActiveBitboard = BitBoard.GetKingAttacksForSquare(square);
                    CurrentMagicInfo = null;
                    break;
                case PieceType.Pawn:
                    ActiveBitboard = BitBoard.GetPawnAttacksForSquare(square, currentPiece.Color);
                    CurrentMagicInfo = null;
                    break;
            }
        }
        else
        {
            // Piece moved or captured, clear the overlay
            Clear();
        }
    }

    /// <summary>
    /// Gets the current overlay type for external tracking.
    /// </summary>
    public string? GetOverlayType() => _overlayType;

    /// <summary>
    /// Determines the overlay type from the name (for use with BitBoardVisualizer callbacks).
    /// </summary>
    public static string? DetermineOverlayType(string name)
    {
        return name switch
        {
            "Occupied" => "occupied",
            "White Pieces" => "white",
            "Black Pieces" => "black",
            "All White Attacks" => "allattacks:White",
            "All Black Attacks" => "allattacks:Black",
            _ when name.StartsWith("W ") || name.StartsWith("B ") => $"piece:{name}",
            _ => null
        };
    }
}

