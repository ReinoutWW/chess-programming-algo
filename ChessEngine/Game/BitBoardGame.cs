namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Core;

public class BitBoardGame : IGame {
    public BitBoardGame(IPlayer whitePlayer, IPlayer blackPlayer, int _delayPerMoveInMilliseconds = 100) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
        this._delayPerMoveInMilliseconds = _delayPerMoveInMilliseconds;
    }
    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer;
    private readonly IPlayer blackPlayer;
    private int _delayPerMoveInMilliseconds = 100;
    private string starterPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public Func<Task>? NextMoveHandler { get; set; }
    public IGameVisualizer Visualizer { get; set; } = new GameVisualizer();
    public IPlayer? Winner { get; private set; } = null;
    private Stack<(Move, UndoMoveInfo)> _moveHistory = new();

    public bool IsChecked(PieceColor color) {
        return board.IsInCheck(color);
    }
    private BitBoard board = new BitBoard();

    public BitBoardGame() {
        this.board = new BitBoard();
    }

    public async Task Start() {
        await Task.CompletedTask;
    }

    public void Visualize() {
        Console.WriteLine(board.ToString());
    }

    public async Task DoMove(Move move) {
        var undoMoveInfo = board.ApplyMove(move);
        
        _moveHistory.Push((move, undoMoveInfo));
    }

    public bool IsValidMove(Move move) {
        throw new NotImplementedException();
    }

    public IPlayer GetCurrentPlayer() {
        return currentColor == PieceColor.White ? whitePlayer : blackPlayer;
    }

    public List<Move> GetValidMovesForPosition(Position position) {
        throw new NotImplementedException();
    }

    public bool IsFinished() {
        return false;
    }

    public Board GetBoard() {
        throw new NotImplementedException();
    }

    public bool IsPawnPromotionMove(Move move) {
        throw new NotImplementedException();
    }

    public List<Move> GetAllValidMovesForColor(PieceColor color) {
        throw new NotImplementedException();
    }

    public string GetGameEndReason() {
        return string.Empty;
    }

    public bool IsDraw() {
        return false;
    }

    public Move? GetLastMove() {
        return null;
    }

    public void LoadForsythEdwardsNotation(string notation) {
        throw new NotImplementedException();
    }

    public IGame Clone(bool simulated = false) {
        throw new NotImplementedException();
    }
}