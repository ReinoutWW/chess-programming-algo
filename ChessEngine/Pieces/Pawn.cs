namespace Chess.Programming.Ago.Pieces;

using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Chess.Programming.Ago.Core;

public class Pawn(PieceColor color) : Piece(color, PieceType.Pawn) {

    private readonly int _direction = color == PieceColor.White ? 1 : -1;

    public override bool IsValidMove(Board board, Move move) {
        bool isAllowedForwardMove = 
            move.From.Row + _direction == move.To.Row &&
            move.From.Column == move.To.Column &&
            IsEmptySquare(board, move.To);

        bool isAllowedDiagonalCapture = 
            IsOneColumnAway(move.From, move.To) &&
            move.From.Row + _direction == move.To.Row &&
            board.GetPieceAtPosition(move.To) != null &&
            board.GetPieceAtPosition(move.To)!.Color != Color;

        bool isAllowedToMoveTwoSquares = 
            IsStarterPositionForColor(color, move.From.Row) &&
            IsNotJumpingOverPiece(board, move.From, move.To) &&
            IsEmptySquare(board, move.To) &&
            move.To.Column == move.From.Column;

        bool legalTwoSquareMove = 
            isAllowedToMoveTwoSquares && 
            Math.Abs(move.From.Row - move.To.Row) == 2 &&
            move.To.Column == move.From.Column;

        return isAllowedForwardMove 
            || isAllowedDiagonalCapture 
            || legalTwoSquareMove;
    }

    private bool IsEmptySquare(Board board, Position position) {
        return board.GetPieceAtPosition(position) == null;
    }

    private bool IsNotJumpingOverPiece(Board board, Position from, Position to) {
        var step = from.Row + _direction;
        while(step != to.Row) {
            if(step < 0 || step > 7) {
                return false;
            }

            if(board.GetPieceAtPosition(new Position(step, from.Column)) != null) {
                return false;
            }
            step += _direction;
        }
        return true;
    }

    private bool IsOneColumnAway(Position from, Position to) {
        return Math.Abs(from.Column - to.Column) == 1;
    }

    private bool IsStarterPositionForColor(PieceColor color, int row) {
        return color == PieceColor.White ? row == 1 : row == 6;
    }

    public override Piece Clone() {
        return new Pawn(Color) {
            HasMoved = HasMoved
        };
    }
}