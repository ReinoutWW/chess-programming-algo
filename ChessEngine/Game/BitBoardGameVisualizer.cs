namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Core;

public class BitBoardGameVisualizer {
    public void Visualize(IVisualizedBoard board) {
        board.LogBoard();
    }
}