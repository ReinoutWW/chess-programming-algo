namespace Chess.Programming.Ago.ChessEngines.Ordering;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// Aggressive move ordering strategy optimized for iterative deepening.
/// Orders moves by: Captures (MVV-LVA) > Promotions > Central Control > Development
/// </summary>
public class AggressiveOrdering : IMoveOrdering {
    
    // Central squares bonus (e4, d4, e5, d5 and surrounding)
    private static readonly int[,] CentralBonus = {
        {  0,  0,  0,  0,  0,  0,  0,  0 },
        {  0,  5, 10, 10, 10, 10,  5,  0 },
        {  0, 10, 20, 30, 30, 20, 10,  0 },
        {  0, 10, 30, 50, 50, 30, 10,  0 },
        {  0, 10, 30, 50, 50, 30, 10,  0 },
        {  0, 10, 20, 30, 30, 20, 10,  0 },
        {  0,  5, 10, 10, 10, 10,  5,  0 },
        {  0,  0,  0,  0,  0,  0,  0,  0 }
    };

    private static readonly Dictionary<PieceType, int> PieceValues = new() {
        { PieceType.Pawn, 100 },
        { PieceType.Knight, 320 },
        { PieceType.Bishop, 330 },
        { PieceType.Rook, 500 },
        { PieceType.Queen, 900 },
        { PieceType.King, 20000 }
    };

    public List<Move> OrderMoves(List<Move> moves, IGame game) {
        var orderedMoves = new List<(Move move, int priority)>();

        foreach (var move in moves) {
            int priority = 0;
            
            var capturedPiece = game.GetPieceAtPosition(move.To);
            var movingPiece = game.GetPieceAtPosition(move.From);

            // 1. Captures - MVV-LVA (Most Valuable Victim - Least Valuable Attacker)
            if (capturedPiece != null && movingPiece != null) {
                int victimValue = PieceValues.GetValueOrDefault(capturedPiece.Type, 0);
                int attackerValue = PieceValues.GetValueOrDefault(movingPiece.Type, 0);
                // High priority for captures: base 10000 + bonus for good trades
                priority = 10000 + victimValue * 10 - attackerValue;
            }

            // 2. Promotions - very high priority
            if (move.PromotedTo.HasValue) {
                int promotionValue = PieceValues.GetValueOrDefault(move.PromotedTo.Value, 0);
                priority = Math.Max(priority, 8000 + promotionValue);
            }

            // 3. Central control bonus
            if (movingPiece != null) {
                int fromCentral = CentralBonus[move.From.Row, move.From.Column];
                int toCentral = CentralBonus[move.To.Row, move.To.Column];
                int centralImprovement = toCentral - fromCentral;
                
                // Only add bonus for moves that improve central control
                if (centralImprovement > 0) {
                    priority += centralImprovement;
                }
            }

            // 4. Piece development bonus (moving off back rank)
            if (movingPiece != null && movingPiece.Type != PieceType.Pawn && movingPiece.Type != PieceType.King) {
                bool isWhiteBackRank = movingPiece.Color == PieceColor.White && move.From.Row == 7;
                bool isBlackBackRank = movingPiece.Color == PieceColor.Black && move.From.Row == 0;
                
                if (isWhiteBackRank || isBlackBackRank) {
                    // Bonus for developing pieces
                    priority += 50;
                }
            }

            // 5. Pawn push bonus (advancing pawns is often good)
            if (movingPiece != null && movingPiece.Type == PieceType.Pawn) {
                int advancement = movingPiece.Color == PieceColor.White 
                    ? move.From.Row - move.To.Row  // White pawns move up (decreasing row)
                    : move.To.Row - move.From.Row; // Black pawns move down (increasing row)
                
                if (advancement > 0) {
                    priority += advancement * 10;
                }
            }

            orderedMoves.Add((move, priority));
        }

        return orderedMoves
            .OrderByDescending(m => m.priority)
            .Select(m => m.move)
            .ToList();
    }
}

