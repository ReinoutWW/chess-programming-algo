namespace Chess.Programming.Ago.ChessEngines.Ordering;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public class MVVLVAOrdering : IMoveOrdering {
    public List<Move> OrderMoves(List<Move> moves, IGame game) {
        var orderedMoves = new List<(Move move, int priority)>();

        foreach (var move in moves) {
            int priority = 0;

            var capturedPiece = game.GetPieceAtPosition(move.To);
            var movingPiece = game.GetPieceAtPosition(move.From);
            
            if (capturedPiece != null && movingPiece != null) {
                int victimValue = PieceValues.GetValueOrDefault(capturedPiece.Type, 0);
                int attackerValue = PieceValues.GetValueOrDefault(movingPiece.Type, 0);
                priority = victimValue * 10 - attackerValue + 10000; 
            }

            // Check for promotion
            if (move.PromotedTo.HasValue) {
                int promotionValue = PieceValues.GetValueOrDefault(move.PromotedTo.Value, 0);
                priority = Math.Max(priority, promotionValue + 5000);
            }

            orderedMoves.Add((move, priority));
        }

        return orderedMoves.OrderByDescending(m => m.priority)
            .Select(m => m.move)
            .ToList();
    }

    private static readonly Dictionary<Pieces.PieceType, int> PieceValues = new() {
        { Pieces.PieceType.Pawn, 100 },
        { Pieces.PieceType.Knight, 320 },
        { Pieces.PieceType.Bishop, 330 },
        { Pieces.PieceType.Rook, 500 },
        { Pieces.PieceType.Queen, 900 },
        { Pieces.PieceType.King, 20000 }
    };
}