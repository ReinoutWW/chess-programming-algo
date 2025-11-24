namespace Chess.Programming.Ago.Core;

using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core.Exceptions;

public class Board {
    private readonly Piece[,] pieces = new Piece[8, 8];
    private readonly List<Piece> capturedPieces = new List<Piece>();
    private Move? lastMove = null;

    public Board() {
        SetupInitialPieces();
    }

    public Move? GetLastMove() => lastMove;

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

    /// <summary>
    /// Applies a move to the board and returns true if it was a capture
    /// If it was a capture, the captured piece is added to the capturedPieces list
    /// </summary>
    /// <param name="move"></param>
    /// <returns>True if it was a capture, false otherwise</returns>
    /// <exception cref="InvalidMoveException"></exception>
    public MoveResult ApplyMove(Move move) {
        var fromPiece = pieces[move.From.Row, move.From.Column];

        if(fromPiece == null) {
            throw new InvalidMoveException("No piece at from position");
        }

        var moveResult = new MoveResult(false, null);

        if(IsCastlingMove(move, fromPiece.Color)) {
            moveResult = ApplyCastlingMove(move);
        } else if(IsEnPassantMove(move)) {
            moveResult = ApplyEnPassantMove(move);
        } else {
            moveResult = ApplyRegularMove(move);
        }

        lastMove = move;

        return moveResult;
    }

    private bool IsEnPassantMove(Move move) {
        var movingPiece = GetPieceAtPosition(move.From);
        var destinationPiece = GetPieceAtPosition(move.To);
        
        // Is it a pawn moving diagonally to an empty square?
        if(movingPiece == null || movingPiece.Type != PieceType.Pawn) {
            return false;
        }
        
        // Is it moving diagonally?
        bool isDiagonal = move.From.Column != move.To.Column;
        
        // Is the destination empty?
        bool destinationEmpty = destinationPiece == null;
        
        return isDiagonal && destinationEmpty;
    }

    private MoveResult ApplyEnPassantMove(Move move) {
        var lastMoveThatWillBeEnpassanted = GetLastMove();
        
        if(lastMoveThatWillBeEnpassanted == null) {
            throw new InvalidMoveException("Invalid en passant move: no last move");
        }
        
        var lastMovedPawn = GetPieceAtPosition(lastMoveThatWillBeEnpassanted.To);
        var movingPawn = GetPieceAtPosition(move.From);

        if(lastMovedPawn == null || movingPawn == null) {
            throw new InvalidMoveException("Invalid en passant move");
        }

        pieces[lastMoveThatWillBeEnpassanted.To.Row, lastMoveThatWillBeEnpassanted.To.Column] = null;
        pieces[move.From.Row, move.From.Column] = null;
        pieces[move.To.Row, move.To.Column] = movingPawn;

        movingPawn.HasMoved = true;

        return new MoveResult(true, lastMovedPawn);
    }

    private MoveResult ApplyCastlingMove(Move move) {
        var fromPiece = pieces[move.From.Row, move.From.Column];
        var moveResult = new MoveResult(false, null);

        var isKingSideCastling = move.From.Column < move.To.Column;
        var rookColumn = isKingSideCastling ? 7 : 0;
        var rook = pieces[move.From.Row, rookColumn];
    
        pieces[move.To.Row, move.To.Column] = fromPiece;
        pieces[move.From.Row, move.From.Column] = null;

        pieces[move.From.Row, rookColumn] = null;
        pieces[move.From.Row, isKingSideCastling ? 5 : 3] = rook;

        fromPiece.HasMoved = true;
        rook.HasMoved = true;

        return moveResult;
    }

    private MoveResult ApplyRegularMove(Move move) {
        var fromPiece = pieces[move.From.Row, move.From.Column];
        var toPiece = pieces[move.To.Row, move.To.Column];
        var moveResult = new MoveResult(false, null);

        if(toPiece != null) {
            capturedPieces.Add(toPiece);
            moveResult = new MoveResult(true, toPiece);
        }

        pieces[move.To.Row, move.To.Column] = fromPiece;
        pieces[move.From.Row, move.From.Column] = null;

        fromPiece.HasMoved = true;

        return moveResult;
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
        if(IsOutOfBounds(move)) {
            return false;
        }

        var fromPiece = pieces[move.From.Row, move.From.Column];
        var toPiece = pieces[move.To.Row, move.To.Column];

        if(fromPiece == null) {
            return false;
        }

        if(fromPiece.Color != color) {
            return false;
        }

        if(toPiece != null && toPiece.Color == color) {
            return false;
        }

        return fromPiece.IsValidMove(this, move);
    }

    private static bool IsOutOfBounds(Move move) {
        if(move == null || move.From == null || move.To == null) {
            return true;
        }
        
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
        if(position.Row < 0 || position.Row > 7 || position.Column < 0 || position.Column > 7) {
            return null;
        }

        return pieces[position.Row, position.Column];
    }

    public List<Piece> GetCapturedPieces() {
        return capturedPieces;
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

                if(pieces[i, j] != null) {
                    clonedBoard.pieces[i, j] = pieces[i, j].Clone();
                }
                else {
                    clonedBoard.pieces[i, j] = null;
                }
            }
        }

        clonedBoard.capturedPieces.AddRange(capturedPieces);
        clonedBoard.lastMove = lastMove;

        return clonedBoard;
    }

    public bool IsCastlingMove(Move move, PieceColor color) {
        var fromPiece = pieces[move.From.Row, move.From.Column];
        var toPiece = pieces[move.To.Row, move.To.Column];

        var isTwoSquaresAway = Math.Abs(move.From.Column - move.To.Column) == 2;

        if(!isTwoSquaresAway) {
            return false;
        }

        // Is there even a peice, and is it the correct color, and has it not moved yet?
        if(fromPiece == null || toPiece != null || fromPiece.Color != color || fromPiece.HasMoved) {
            return false;
        }

       // Rook check: Has the rook in the direction of the move been moved yet? 
       // Warning: The rook can either be on A1 or H1, or mirrored
       // Warning: keep both colors in mind
       var rookRow = move.From.Row;
       var isKingSideCastling = move.From.Column < move.To.Column;

       var rookColumn = isKingSideCastling ? 7 : 0;
       var rook = pieces[rookRow, rookColumn];

       if(rook == null || rook.Color != color || rook.HasMoved) {
            return false;
       }

       var squaresThatMayNotBeAttacked = isKingSideCastling ? new List<Position> {
            new Position(move.From.Row, move.From.Column),
            new Position(move.From.Row, move.From.Column + 1),
            new Position(move.From.Row, move.From.Column + 2),
        } : new List<Position> {
            new Position(move.From.Row, move.From.Column),
            new Position(move.From.Row, move.From.Column - 1),
            new Position(move.From.Row, move.From.Column - 2),
            new Position(move.From.Row, move.From.Column - 3),
       };

       foreach(var square in squaresThatMayNotBeAttacked) {
            if(IsSquareUnderAttack(square, color)) {
                return false;
            } else if(!IsSquareEmpty(square) && square != move.From) {
                return false;
            }
       }

       return true;
    }

    private bool IsSquareEmpty(Position position) {
        return GetPieceAtPosition(position) == null;
    }

    private bool IsSquareUnderAttack(Position position, PieceColor color) {
        var pieces = GetPiecesForColor(color == PieceColor.White ? PieceColor.Black : PieceColor.White);

        foreach(var piece in pieces) {
            if(piece.Item1.IsValidMove(this, new Move(piece.Item2, position))) {
                return true;
            }
        }

        return false;
    }
}
