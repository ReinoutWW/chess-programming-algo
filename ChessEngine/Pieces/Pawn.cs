namespace Chess.Programming.Ago.Pieces;

using Chess.Programming.Ago.Core;

public class Pawn(PieceColor color) : Piece(color, PieceType.Pawn) {

    private readonly int _direction = color == PieceColor.White ? -1 : 1;
    private readonly int _startRow = color == PieceColor.White ? 6 : 1;
    private readonly int _promotionRow = color == PieceColor.White ? 0 : 7;

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
            || legalTwoSquareMove
            || IsAllowedEnPassant(move, board);
    }

    public override IEnumerable<Move> GetPossibleMoves(Board board, Position from) {
        var moves = new List<Move>();
        
        // 1. Single square forward
        int forwardRow = from.Row + _direction;
        if (IsOnBoard(forwardRow, from.Column)) {
            var forwardPos = new Position(forwardRow, from.Column);
            if (IsEmptySquare(board, forwardPos)) {
                AddMoveWithPromotionCheck(moves, from, forwardPos);

                // 2. Double square forward from starting position
                if (from.Row == _startRow) {
                    int doubleRow = from.Row + (2 * _direction);
                    var doublePos = new Position(doubleRow, from.Column);
                    if (IsEmptySquare(board, doublePos)) {
                        moves.Add(new Move(from, doublePos));
                    }
                }
            }
        }

        // 3. Diagonal captures (left and right)
        int[] captureColumns = [from.Column - 1, from.Column + 1];
        foreach (int col in captureColumns) {
            if (!IsOnBoard(forwardRow, col)) continue;
            
            var capturePos = new Position(forwardRow, col);
            var targetPiece = board.GetPieceAtPosition(capturePos);
            
            if (targetPiece != null && targetPiece.Color != Color) {
                AddMoveWithPromotionCheck(moves, from, capturePos);
            }
        }

        // 4. En passant
        var enPassantMove = GetEnPassantMove(board, from);
        if (enPassantMove != null) {
            moves.Add(enPassantMove);
        }

        return moves;
    }

    private void AddMoveWithPromotionCheck(List<Move> moves, Position from, Position to) {
        if (to.Row == _promotionRow) {
            // Add all promotion options
            moves.Add(new Move(from, to, PieceType.Queen));
            moves.Add(new Move(from, to, PieceType.Rook));
            moves.Add(new Move(from, to, PieceType.Bishop));
            moves.Add(new Move(from, to, PieceType.Knight));
        } else {
            moves.Add(new Move(from, to));
        }
    }

    private Move? GetEnPassantMove(Board board, Position from) {
        var lastMove = board.GetLastMove();
        if (lastMove == null) return null;

        // Last move must be a pawn double-move
        var movedCells = Math.Abs(lastMove.From.Row - lastMove.To.Row);
        if (movedCells != 2) return null;

        var lastMovePiece = board.GetPieceAtPosition(lastMove.To);
        if (lastMovePiece == null || lastMovePiece.Type != PieceType.Pawn) return null;

        // Our pawn must be adjacent to the enemy pawn
        if (from.Row != lastMove.To.Row) return null;
        if (Math.Abs(from.Column - lastMove.To.Column) != 1) return null;

        // The target square is behind the enemy pawn
        var direction = lastMovePiece.Color == PieceColor.White ? 1 : -1;
        return new Move(from, new Position(lastMove.To.Row + direction, lastMove.To.Column));
    }

    private bool IsAllowedEnPassant(Move move, Board board) {
        var lastMove = board.GetLastMove();

        if(lastMove == null) {
            return false;
        }

        var movedCells = Math.Abs(lastMove.From.Row - lastMove.To.Row);

        if(lastMove == null || movedCells != 2) {
            return false;
        }

        var lastMovePiece = board.GetPieceAtPosition(lastMove.To);

        if(lastMovePiece == null || lastMovePiece.Type != PieceType.Pawn) {
            return false;
        }

        var possibleOpponentEnPassantPositions = new List<Position>();

        if(lastMove.To.Column < 7) {
            possibleOpponentEnPassantPositions.Add(new Position(lastMove.To.Row, lastMove.To.Column + 1));        
        }

        if(lastMove.To.Column > 0) {
            possibleOpponentEnPassantPositions.Add(new Position(lastMove.To.Row, lastMove.To.Column - 1));
        }

        var direction = lastMovePiece.Color == PieceColor.White ? 1 : -1;

        foreach(var possibleOpponentEnPassantPosition in possibleOpponentEnPassantPositions) {
            var pieceAtPosition = board.GetPieceAtPosition(possibleOpponentEnPassantPosition);

            if(pieceAtPosition != null 
                && pieceAtPosition.Color != lastMovePiece.Color 
                && pieceAtPosition.Type == PieceType.Pawn
                && possibleOpponentEnPassantPosition == move.From
            ) {
                if(move.To == new Position(lastMove.To.Row + direction, lastMove.To.Column)) {
                    return true;
                }
            }
        }

        return false;
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
        return color == PieceColor.White ? row == 6 : row == 1;
    }

    public override bool CanAttackSquare(Board board, Move move) {
        // Pawns attack diagonally one square forward, regardless of whether a piece is there
        return IsOneColumnAway(move.From, move.To) &&
               move.From.Row + _direction == move.To.Row;
    }

    public override Piece Clone() {
        return new Pawn(Color) {
            HasMoved = HasMoved
        };
    }
}