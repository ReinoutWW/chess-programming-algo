namespace Chess.Programming.Ago.Core;

public class Position {
    public int Row { get; }
    public int Column { get; }

    public Position(int row, int column) {
        Row = row;
        Column = column;
    }

    public override bool Equals(object? obj) {
        if(obj == null) {
            return false;
        }
        
        if(obj is Position position) {
            return Row == position.Row && Column == position.Column;
        }

        return false;
    }

    public static bool operator ==(Position left, Position right) {
        if(left is null && right is null) {
            return true;
        }

        if(left is null || right is null) {
            return false;
        }

        return left?.Equals(right) ?? false;
    }

    public static bool operator !=(Position left, Position right) {
        return !(left?.Equals(right) ?? false);
    }
}