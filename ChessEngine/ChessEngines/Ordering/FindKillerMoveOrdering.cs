namespace Chess.Programming.Ago.ChessEngines.Ordering;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public class FindKillerMoveOrdering : IMoveOrdering {
    public List<Move> OrderMoves(List<Move> moves, IGame game) {
        var killerMoves = FindKillerMoves(moves, game);

        return moves.OrderByDescending(m => killerMoves.Contains(m)).ToList();
    }

    private List<Move> FindKillerMoves(List<Move> moves, IGame game) {
        var killerMoves = new List<Move>();
        foreach (var move in moves)
        {
            if (HasCaptureOpportunity(move, game))
            {
                killerMoves.Add(move);
            }
        }
        return killerMoves;
    }

    private bool HasCaptureOpportunity(Move move, IGame game) {
        var capturedPiece = game.GetPieceAtPosition(move.To);
        var movingPiece = game.GetPieceAtPosition(move.From);
        
        return capturedPiece != null && movingPiece != null && capturedPiece.Color != movingPiece.Color;
    }
}