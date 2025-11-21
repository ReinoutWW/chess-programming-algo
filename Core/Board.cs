namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core.Exceptions;
using System.ComponentModel;

public class Board {
    private readonly Piece[,] pieces = new Piece[8, 8];
    private readonly List<Piece> capturedPieces = new List<Piece>();

    public Board() {
        SetupInitialPieces();
    }

    public List<(Piece, Position)> GetPiecesForColor(PieceColor color) {
        var pieces = new List<(Piece, Position)>();
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(this.pieces[i, j] != null && this.pieces[i, j].Color == color) {
                    pieces.Add((this.pieces[i, j], new Position(i, j)));
                }
            }
        }

        return pieces;
    }

    public Piece[,] GetPieces() => pieces;

    private void SetupInitialPieces() {
        SetupForColor(PieceColor.White);
        SetupForColor(PieceColor.Black);
    }

    public void ApplyMove(Move move) {
        var fromPiece = pieces[move.From.Row, move.From.Column];
        var toPiece = pieces[move.To.Row, move.To.Column];

        if(fromPiece == null) {
            throw new InvalidMoveException("No piece at from position");
        }

        if(toPiece != null) {
            capturedPieces.Add(toPiece);
        }

        pieces[move.To.Row, move.To.Column] = fromPiece;
        pieces[move.From.Row, move.From.Column] = null;

        fromPiece.HasMoved = true;
    }

    /// <summary>
    /// Check if this is valid:
    /// 1. Is the piece empty?
    /// 2. Is the piece the color of the current player?
    /// 3. Is the move valid for the piece?
    /// 4. Is the move on the board?
    /// 5. Is the to piece the color of the opponent?
    /// </summary>
    /// <param name="move"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public bool IsValidMove(Move move, PieceColor color) {
        var fromPiece = pieces[move.From.Row, move.From.Column];
        var toPiece = pieces[move.To.Row, move.To.Column];

        if(fromPiece == null) {
            return false;
        }

        if(fromPiece.Color != color) {
            return false;
        }

        if(IsOutOfBounds(move)) {
            return false;
        }

        if(toPiece != null && toPiece.Color == color) {
            return false;
        }

        return fromPiece.IsValidMove(this, move);
    }

    private static bool IsOutOfBounds(Move move) {
        return 
            move.From.Row < 0 || 
            move.From.Row > 7 || 
            move.From.Column < 0 || 
            move.From.Column > 7 ||
            move.To.Row < 0 || 
            move.To.Row > 7 || 
            move.To.Column < 0 || 
            move.To.Column > 7;
    }

    public Piece? GetPieceAtPosition(Position position) {
        return pieces[position.Row, position.Column];
    }

    private void SetupForColor(PieceColor color) {
        int pawnRow = 
            color == PieceColor.White 
                ? 1 
                : 6;
        
        for (int i = 0; i < 8; i++) {
            pieces[pawnRow, i] = new Pawn(color);
        }

        int row = 
            color == PieceColor.White 
                ? 0 
                : 7;

        pieces[row, 0] = new Rook(color);
        pieces[row, 1] = new Knight(color);
        pieces[row, 2] = new Bishop(color);
        pieces[row, 3] = new Queen(color);
        pieces[row, 4] = new King(color);
        pieces[row, 5] = new Bishop(color);
        pieces[row, 6] = new Knight(color);
        pieces[row, 7] = new Rook(color);
    }

    public Board Clone() {
        var clonedBoard = new Board();

        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                clonedBoard.pieces[i, j] = pieces[i, j];
            }
        }

        clonedBoard.capturedPieces.AddRange(capturedPieces);

        return clonedBoard;
    }

}
