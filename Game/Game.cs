using Chess.Programming.Ago.Core;

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

    private void NextTurn() {
        currentColor = 
                currentColor == PieceColor.White 
                    ? PieceColor.Black 
                    : PieceColor.White;
    }

    /// <summary>
    /// 1. The board validates: Color, Empty, Out of bounds, Piece logic.
    /// 2. The game validates: Is the move valid for the current player? 
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private bool IsValidMove(Move move) {
        var isValidOnBoard = board.IsValidMove(move, currentColor);

        return isValidOnBoard;
    }

    public void Visualize() {
        Visualizer.Visualize(board);
    }

}