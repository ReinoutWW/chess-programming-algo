using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

int StringToBitPosition(string position) {
    var column = (int)Enum.Parse(typeof(Columns), position[0].ToString().ToUpper());
    var row = int.Parse(position[1].ToString()) - 1;
    return row * 8 + column;
}

var board = new BitBoard();
board.ApplyMove(new Move(new Position(1, 0), new Position(2, 0)));

board.LogBoard();

enum Columns {
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
}