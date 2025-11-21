namespace Chess.Programming.Ago.Core.Exceptions;

public class InvalidMoveException : Exception {
    public InvalidMoveException(string message) : base(message) {
    }
}