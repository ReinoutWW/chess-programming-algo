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
        return IsCheck(board, color);
    }

    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer;
    private readonly IPlayer blackPlayer;
    private Board board = new Board();

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

    public async Task Start() {
        Visualize();

        ValidateIfGameIsOver();

        if(!IsGameActive) {
            return;
        }

        await RunNextMoveIfAI();
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
            for(int i = 0; i < 8; i++) {
                for(int j = 0; j < 8; j++) {
                    if(IsValidMove(new Move(piece.Item2, new Position(i, j)))) {
                        validMoves.Add(new Move(piece.Item2, new Position(i, j)));
                    }
                }
            }
        }
        
        return validMoves;
    }

    private void ValidateIfGameIsOver() {

        if(_movesWithoutCapture >= MAX_MOVES_WITHOUT_CAPTURE) {
            Winner = null;
            IsGameActive = false;
            _gameEndReason = GameEndReason.FiftyMovesWithoutCapture;
            return;
        }

        if(!HasAnyLegalMoves(currentColor)) {
            if(IsCheck(board, currentColor)) {

                Winner = currentColor == PieceColor.White ? blackPlayer : whitePlayer;
                _gameEndReason = GameEndReason.Checkmate;
                IsGameActive = false;
            } else {
                Winner = null;
                _gameEndReason = GameEndReason.Stalemate;
                IsGameActive = false;
            }
        }

        ValidateIfInsufficientMaterial();
    }

    private void ValidateIfInsufficientMaterial() {
        var whitePieces = board.GetPiecesForColor(PieceColor.White);
        var blackPieces = board.GetPiecesForColor(PieceColor.Black);

        if(whitePieces.HasInsufficientMaterial() && blackPieces.HasInsufficientMaterial()) {
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
        Visualizer?.Visualize(board);
    }

    public List<Position> GetValidMovesForPosition(Position position) {
        var piece = board.GetPieceAtPosition(position);

        var validmoves = new List<Move>();
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                if(position == new Position(i, j)) {
                    continue;
                }

                if(IsValidMove(new Move(position, new Position(i, j)))) {
                    validmoves.Add(new Move(position, new Position(i, j)));
                }
            }
        }

        return validmoves
            .Select(move => move.To)
                .ToList();
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