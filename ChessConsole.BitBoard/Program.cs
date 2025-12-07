using Chess.Programming.Ago.Core;

int StringToBitPosition(string position) {
    var column = (int)Enum.Parse(typeof(Columns), position[0].ToString().ToUpper());
    var row = int.Parse(position[1].ToString()) - 1;
    return row * 8 + column;
}

string IndexToColumnString(int index) {
    return ((Columns)index).ToString().ToUpper();
}

string IndexToRowString(int index) {
    return (index + 1).ToString();
}

string MoveToPositionString(Move move) {
   // 0 = A, 1 = B, 2 = C, 3 = D, 4 = E, 5 = F, 6 = G, 7 = H
   // 0 = 1, 1 = 2, 2 = 3, 3 = 4, 4 = 5, 5 = 6, 6 = 7, 7 = 8

   var fromString = $"{IndexToColumnString(move.From.Column)}{IndexToRowString(move.From.Row)}";
   var toString = $"{IndexToColumnString(move.To.Column)}{IndexToRowString(move.To.Row)}";

   return $"{fromString} -> {toString}";
}

var board = new BitBoard();
board.LogBoard();  // Initial position

var move = new Move(new Position(1, 4), new Position(7, 4));  // e2 -> e4
var undoInfo = board.ApplyMove(move);
board.LogBoard();  // Pawn moved

board.UndoMove(undoInfo);
board.LogBoard();  // Should be back to initial!

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