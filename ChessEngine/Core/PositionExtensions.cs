namespace Chess.Programming.Ago.Core;

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public static class PositionExtensions {
    /// <summary>
    /// Position strings are: from,to. So: a1a2 = from a1 to a2.
    /// </summary>
    /// <param name="position">string in the format of from,to</param>
    /// <returns></returns>
    public static (Position From, Position To) GetPosition(this string position) {
        if(!IsValidPosition(position)) {
            throw new InvalidOperationException("Invalid position string");
        }

        // a2a3 -> fromColumn = 0, fromRow = 2, toColumn = 0, toRow = 3
        var fromColumn = ColumnToIndex(position[0]); // a -> 0
        var fromRow = int.Parse(position[1].ToString()) - 1; // 2 -> 1
        var toColumn = ColumnToIndex(position[2]); // a -> 0
        var toRow = int.Parse(position[3].ToString()) - 1; // 3 -> 2

        var fromPosition = new Position(fromRow, fromColumn);
        var toPosition = new Position(toRow, toColumn);

        return (fromPosition, toPosition);
    }

    private static readonly Regex PositionRegex =
        new(@"^[a-h][1-8][a-h][1-8]$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static bool IsValidPosition(string positionString)
        => PositionRegex.IsMatch(positionString);

    private static readonly List<char> Columns 
        = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

    private static int ColumnToIndex(char column) 
        => Columns.IndexOf(column);
}