namespace Chess.Programming.Ago.ChessEngines.Ordering;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

public interface IMoveOrdering {
    List<Move> OrderMoves(List<Move> moves, IGame game);
}