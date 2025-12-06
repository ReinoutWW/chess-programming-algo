namespace Chess.Programming.Ago.Core.Extensions;

public static class BitBoardExtensions {
    public static int ToBitPosition(this Position position) {
        return position.Row * 8 + position.Column;
    }
}