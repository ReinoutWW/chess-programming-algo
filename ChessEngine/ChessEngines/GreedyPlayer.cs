namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Pieces;

/// <summary>
/// The GreedyPlayer is a simple AI that will choose the move that will capture the most valuable piece.
/// If there are no captures, it will choose a random move.
/// It will not consider the checkmate or stalemate.
/// </summary>
/// <param name="color"></param>
public class GreedyPlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public bool IsAI() => true;

    private int GetValueOfPiece(Piece piece) {
        return piece.Type switch {
            PieceType.Pawn => 1,
            PieceType.Knight => 3,
            PieceType.Bishop => 3,
            PieceType.Rook => 5,
            PieceType.Queen => 9,
            PieceType.King => 100,
            _ => 0,
        };
    }

    public async Task<Move> GetMove(IGame game) {
        var pieces = game.GetBoard().GetPiecesForColor(color);

        Move? bestMove = null;
        var bestValue = 0;
        
        foreach(var piece in pieces) {
            var validMoves = game.GetValidMovesForPosition(piece.Item2);

            foreach(var toPosition in validMoves) {
                var enemyPiece = game.GetBoard().GetPieceAtPosition(toPosition);

                if(enemyPiece != null && enemyPiece.Color != color) {
                    var value = GetValueOfPiece(enemyPiece);

                    if(value > bestValue) {
                        bestValue = value;
                        bestMove = new Move(piece.Item2, toPosition);
                    }
                }
            }
        }

        if(bestMove is null && bestValue == 0) {
            var allValidMoves = game.GetAllValidMovesForColor(color);
            bestMove = allValidMoves[Random.Shared.Next(allValidMoves.Count)];
        }

        if(bestMove == null) {
            throw new InvalidOperationException("No valid moves found");
        }

        return bestMove;
    }
}