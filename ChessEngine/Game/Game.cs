using System.Threading.Tasks;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

namespace Chess.Programming.Ago.Game;

public class Game : IGame {
    public Game(IPlayer whitePlayer, IPlayer blackPlayer, int _delayPerMoveInMilliseconds = 100, string? starterPosition = null)
    {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
        this._delayPerMoveInMilliseconds = _delayPerMoveInMilliseconds;

        if(starterPosition != null) {
            this.starterPosition = starterPosition;
        }

        LoadForsythEdwardsNotation(this.starterPosition);
    }

    private int _delayPerMoveInMilliseconds = 100;
    private string starterPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public Func<Task>? NextMoveHandler { get; set; }
    private bool IsGameActive = true;
    public IGameVisualizer? Visualizer { get; set; } = new GameVisualizer();
    public IPlayer? Winner { get; private set; } = null;
    private IPlayer _currentPlayer 
        => currentColor == PieceColor.White 
            ? whitePlayer 
            : blackPlayer;

    public bool IsSimulated { get; private set; } = false;

    public string GetGameEndReason() => _gameEndReason.GetDescription();

    private int _movesWithoutCapture = 0;
    private const int MAX_MOVES_WITHOUT_CAPTURE = 50;
    private GameEndReason _gameEndReason = GameEndReason.None;

    public bool IsChecked(PieceColor color) {
        return board.IsCheck(color);
    }

    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer;
    private readonly IPlayer blackPlayer;
    private Board board = new Board();

    public IPlayer GetCurrentPlayer() => _currentPlayer;

    public async Task DoMove(Move move) {
        if(!IsValidMove(move)) {
            Console.WriteLine("Invalid move, try again...");
            Visualize();
            throw new InvalidOperationException("Invalid move");
        }

        var movedPiece = board.GetPieceAtPosition(move.From);
        var moveResult = board.ApplyMove(move);

        if(moveResult.WasCapture || movedPiece!.Type == PieceType.Pawn) {
            _movesWithoutCapture = 0;
        } else {
            _movesWithoutCapture++;
        }

        Visualize();

        if(NextMoveHandler != null) {
            await NextMoveHandler();
        }

        await NextTurn();
    }

    public Move? GetLastMove() => board.GetLastMove();

    public Board GetBoard() => board;

    public bool IsPawnPromotionMove(Move move) 
        => board.IsPawnPromotionMove(move);

    public async Task Start() {
        Visualize();

        ValidateIfGameIsOver();

        if(!IsGameActive) {
            return;
        }

        await RunNextMoveIfAI();
    }

    public bool IsFinished() => !IsGameActive;

    private async Task NextTurn() {
        currentColor = 
                currentColor == PieceColor.White 
                    ? PieceColor.Black 
                    : PieceColor.White;

        ValidateIfGameIsOver();

        if(!IsGameActive) {
            return;
        }

        await RunNextMoveIfAI();
    }

    private async Task RunNextMoveIfAI() {
        if(GetCurrentPlayer().IsAI() && !IsSimulated) {
            await Task.Delay(_delayPerMoveInMilliseconds);

            var move = await GetCurrentPlayer().GetMove(this);
            await DoMove(move);
        }
    }

    public List<Move> GetAllValidMovesForColor(PieceColor color) {
        var pieces = board.GetPiecesForColor(color);

        var validMoves = new List<Move>();
        
        foreach(var piece in pieces) {
            var validMovesForPiece = GetValidMovesForPosition(piece.Item2);
            validMoves.AddRange(validMovesForPiece);
        }

        return validMoves;
    }

    public List<Piece> GetCapturedPieces() {
        return board.GetCapturedPieces();
    }

    public List<(Piece, Position)> GetPiecesForColor(PieceColor color) {
        return board.GetPiecesForColor(color);
    }

    private void ValidateIfGameIsOver() {

        if(_movesWithoutCapture >= MAX_MOVES_WITHOUT_CAPTURE) {
            Winner = null;
            IsGameActive = false;
            _gameEndReason = GameEndReason.FiftyMovesWithoutCapture;
            return;
        }

        if(!this.HasAnyLegalMoves(currentColor)) {
            if(board.IsCheck(currentColor)) {

                Winner = currentColor == PieceColor.White ? blackPlayer : whitePlayer;
                _gameEndReason = GameEndReason.Checkmate;
                IsGameActive = false;
            } else {
                Winner = null;
                _gameEndReason = GameEndReason.Stalemate;
                IsGameActive = false;
            }
        }

        if(board.HasInsufficientMaterialForDraw()) {
            Winner = null;
            _gameEndReason = GameEndReason.InsufficientMaterial;
            IsGameActive = false;
        }
    }

    /// <summary>
    /// 1. The board validates: Color, Empty, Out of bounds, Piece logic.
    /// 2. The game validates: Is the move valid for the current player? 
    /// 3. Is the user checked after the move?
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public bool IsValidMove(Move move) {
        var isValidOnBoard = board.IsValidMove(move, currentColor);
        
        if(!isValidOnBoard) {
            return false;
        }

        var currentPlayerIsCheckedAfterMove = board.IsCurrentPlayerCheckedAfterMove(move, currentColor);

        return isValidOnBoard
            && !currentPlayerIsCheckedAfterMove;
    }

    public void Visualize() {
        Visualizer?.Visualize(board);
    }

    public Piece? GetPieceAtPosition(Position position) {
        return board.GetPieceAtPosition(position);
    }

    public List<Move> GetValidMovesForPosition(Position position) {
        var piece = board.GetPieceAtPosition(position);

        var validmoves = new List<Move>();
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(position == new Position(i, j)) {
                    continue;
                }

                if(IsPawnPromotionMove(new Move(position, new Position(i, j)))) {
                    var promotionChoices = new List<PieceType> { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };
                    foreach(var promotionChoice in promotionChoices) {
                        if(IsValidMove(new Move(position, new Position(i, j), promotionChoice))) {
                            validmoves.Add(new Move(position, new Position(i, j), promotionChoice));
                        }
                    }
                } else if(IsValidMove(new Move(position, new Position(i, j)))) {
                    validmoves.Add(new Move(position, new Position(i, j)));
                }
            }
        }

        return validmoves;
    }

    public IGame Clone(bool simulated = false) {
        var clonedBoard = board.Clone();
        return new Game(whitePlayer, blackPlayer, 0) {
            board = clonedBoard,
            currentColor = currentColor,
            _movesWithoutCapture = _movesWithoutCapture,
            _gameEndReason = _gameEndReason,
            Winner = Winner,
            NextMoveHandler = null,
            IsGameActive = IsGameActive,
            Visualizer = null,
            IsSimulated = simulated
        };
    }

    public bool IsDraw() {
        return _gameEndReason == GameEndReason.InsufficientMaterial 
            || _gameEndReason == GameEndReason.FiftyMovesWithoutCapture;
    }

    public void LoadForsythEdwardsNotation(string notation) {
        var parts = notation.Split(' ');
        var turn = parts[1];

        currentColor = turn == "w" ? PieceColor.White : PieceColor.Black;
        
        var board = new Board();

        board.LoadForsythEdwardsNotation(notation);
        
        this.board = board;
    }
}