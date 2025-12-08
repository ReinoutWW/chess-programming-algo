using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Game;

public static class GameExtensions
{
    public static bool IsCheck(this Board board, PieceColor currentColor)
    {
        var hasValidMoveOnKing = false;

        var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

        var enemyPieces = board.GetPiecesForColor(opponentColor);

        var king = board.GetPiecesForColor(currentColor)
            .FirstOrDefault(piece => piece.Item1.Type == PieceType.King);

        foreach (var enemyPiece in enemyPieces)
        {
            var canMakeMoveToKing = board.IsValidMove(new Move(enemyPiece.Item2, king.Item2), opponentColor);

            if (canMakeMoveToKing)
            {
                hasValidMoveOnKing = true;
                break;
            }
        }

        return hasValidMoveOnKing;
    }

    public static bool HasAnyLegalMoves(this IGame game, PieceColor color)
    {
        var pieces = game.GetPiecesForColor(color);

        foreach(var piece in pieces) {
            var validMoves = game.GetValidMovesForPosition(piece.Item2);
            if(validMoves.Count > 0) {
                return true;
            }
        }

        return false;
    }

    public static bool IsCurrentPlayerCheckedAfterMove(this Board board, Move move, PieceColor currentColor)
    {
        var simulatedBoard = board.Clone();

        simulatedBoard.ApplyMove(move);

        if (simulatedBoard.IsCheck(currentColor))
        {
            return true;
        }

        return false;
    }

    public static bool HasInsufficientMaterialForDraw(this Board board)
    {
        var whitePieces = board.GetPiecesForColor(PieceColor.White);
        var blackPieces = board.GetPiecesForColor(PieceColor.Black);

        return whitePieces.HasInsufficientMaterial() && blackPieces.HasInsufficientMaterial();
    }
}

