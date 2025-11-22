using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Game;

public class Game(IPlayer whitePlayer, IPlayer blackPlayer) : IGame {
    private bool IsGameActive = true;
    public IGameVisualizer Visualizer { get; } = new GameVisualizer();
    public IPlayer? Winner { get; private set; } = null;
    private IPlayer _currentPlayer 
        => currentColor == PieceColor.White 
            ? whitePlayer 
            : blackPlayer;

    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer = whitePlayer;
    private readonly IPlayer blackPlayer = blackPlayer;
    private readonly Board board = new Board();

    public IPlayer GetCurrentPlayer() => _currentPlayer;

    private bool IsCheck(Board board, PieceColor currentColor) {
        var hasValidMoveOnKing = false;

        var opponentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

        var enemyPieces = board.GetPiecesForColor(opponentColor);

        var king = board.GetPiecesForColor(currentColor)
            .FirstOrDefault(piece => piece.Item1.Type == PieceType.King);
        
        foreach(var enemyPiece in enemyPieces) {
            var canMakeMoveToKing = board.IsValidMove(new Move(enemyPiece.Item2, king.Item2), opponentColor);
            
            if(canMakeMoveToKing) {
                hasValidMoveOnKing = true;
                break;
            }
        }

        return hasValidMoveOnKing;
    }

    public void DoMove(Move move) {
        if(!IsValidMove(move)) {
            Console.WriteLine("Invalid move, try again...");
            Visualize();
            throw new InvalidOperationException("Invalid move");
        }

        board.ApplyMove(move);
        Visualize();

        NextTurn();
    }

    public Board GetBoard() => board;

    public void Start() {
        Visualize();
    }

    public bool IsFinished() => !IsGameActive;

    private bool HasAnyLegalMoves(PieceColor color) {
        var pieces = board.GetPiecesForColor(color);

        foreach(var piece in pieces) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (IsValidMove(new Move(piece.Item2, new Position(i, j)))) {
                        return true;
                    }
                }
            }   
        }

        return false;
    }

    private void NextTurn() {
        currentColor = 
                currentColor == PieceColor.White 
                    ? PieceColor.Black 
                    : PieceColor.White;

        ValidateIfGameIsOver();
    }

    private void ValidateIfGameIsOver() {
        if(!HasAnyLegalMoves(currentColor)) {
            if(IsCheck(board, currentColor)) {
                Winner = currentColor == PieceColor.White ? blackPlayer : whitePlayer;
                
                IsGameActive = false;
            } else {
                Winner = null;
                IsGameActive = false;
            }
        }
    }

    /// <summary>
    /// 1. The board validates: Color, Empty, Out of bounds, Piece logic.
    /// 2. The game validates: Is the move valid for the current player? 
    /// 3. Is the user checked after the move?
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private bool IsValidMove(Move move) {
        var isValidOnBoard = board.IsValidMove(move, currentColor);
        
        if(!isValidOnBoard) {
            return false;
        }

        var currentIserIsCheckedAfterMove = CurrentIserIsCheckedAfterMove(board, move);

        return isValidOnBoard 
            && !currentIserIsCheckedAfterMove;
    }

    private bool CurrentIserIsCheckedAfterMove(Board board, Move move) {
        var simulatedBoard = board.Clone();

        simulatedBoard.ApplyMove(move);

        if(IsCheck(simulatedBoard, currentColor)) {
            return true;
        }

        return false;
    }

    public void Visualize() {
        Visualizer.Visualize(board);
    }

    public List<Position> GetValidMovesForPosition(Position position)
        => board.GetValidMovesForPosition(position);
}