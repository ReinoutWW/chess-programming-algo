using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Game;

public class Game(IPlayer whitePlayer, IPlayer blackPlayer) : IGame {
    private bool IsGameActive = true;
    public IGameVisualizer Visualizer { get; } = new GameVisualizer();

    private IPlayer currentPlayer 
        => currentColor == PieceColor.White 
            ? whitePlayer 
            : blackPlayer;

    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer = whitePlayer;
    private readonly IPlayer blackPlayer = blackPlayer;
    private readonly Board board = new Board();

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

    public void Start() {
        Console.WriteLine("Game started");

        Visualize();
        Console.WriteLine("Waiting for move...");

        while (IsGameActive) {
            var move = currentPlayer.GetMove(this);

            if(!IsValidMove(move)) {
                Console.WriteLine("Invalid move, try again...");
                continue;
            }

            board.ApplyMove(move);
            Visualize();

            NextTurn();
        }
    }

    private bool HasAnyLegalMoves(PieceColor color) {
        var pieces = board.GetPiecesForColor(color);
        var hasValidMoves = false;

        foreach(var piece in pieces) {
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (IsValidMove(new Move(piece.Item2, new Position(i, j)))) {
                        return true;
                    }
                }
            }   
        }

        return hasValidMoves;
    }

    private void NextTurn() {
        currentColor = 
                currentColor == PieceColor.White 
                    ? PieceColor.Black 
                    : PieceColor.White;

        if(!HasAnyLegalMoves(currentColor)) {
            if(IsCheck(board, currentColor)) {
                Console.WriteLine("Game over, {0} wins!", currentColor == PieceColor.White ? "Black": "White");
                IsGameActive = false;
            } else {
                Console.WriteLine("Game over, draw by stalemate");
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

}