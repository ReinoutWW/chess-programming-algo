namespace Chess.Programming.Ago.Game;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;
using Chess.Programming.Ago.Core.Extensions;

public class BitBoardGame : IGame {
    public BitBoardGame(IPlayer whitePlayer, IPlayer blackPlayer, int _delayPerMoveInMilliseconds = 100) {
        this.whitePlayer = whitePlayer;
        this.blackPlayer = blackPlayer;
        this._delayPerMoveInMilliseconds = _delayPerMoveInMilliseconds;
    }

    private const int MAX_MOVES_WITHOUT_CAPTURE = 50;
    private int _movesWithoutCapture = 0;
    private bool IsGameActive = true;
    private GameEndReason _gameEndReason = GameEndReason.None;
    private Dictionary<ulong, int> _positionCounts = new();
    public bool IsSimulated { get; private set; } = false;
    private PieceColor currentColor = PieceColor.White;
    private readonly IPlayer whitePlayer;
    private readonly IPlayer blackPlayer;
    private int _delayPerMoveInMilliseconds = 100;
    private string starterPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public Func<Task>? NextMoveHandler { get; set; }
    public Func<Task>? BeforeAIMoveHandler { get; set; }
    public BitBoardGameVisualizer Visualizer { get; set; } = new BitBoardGameVisualizer();
    public IPlayer? Winner { get; private set; } = null;
    private Stack<(Move move, UndoMoveInfo undoMoveInfo)> _moveHistory = new();

    public bool IsChecked(PieceColor color) {
        return board.IsInCheck(color);
    }
    private IVisualizedBoard board = new BitBoard();

    public BitBoardGame() {
        this.board = new BitBoard();
    }

    public async Task Start() {
        RecordPosition(); // Record starting position
        Visualize();

        if(GetCurrentPlayer().IsAI()) {
            await RunNextMoveIfAI();
        }
    }
    
    private ulong GetPositionHash() => board.OccupiedSquares ^ (board.WhitePieces << 1) ^ (ulong)currentColor;
    
    private void RecordPosition() {
        var hash = GetPositionHash();
        _positionCounts[hash] = _positionCounts.GetValueOrDefault(hash) + 1;
    }
    
    private void UnrecordPosition() {
        var hash = GetPositionHash();
        if (_positionCounts.TryGetValue(hash, out var count) && count > 1)
            _positionCounts[hash] = count - 1;
        else
            _positionCounts.Remove(hash);
    }
    
    private bool IsThreefoldRepetition() => _positionCounts.GetValueOrDefault(GetPositionHash()) >= 3;

    public List<(Piece, Position)> GetPiecesForColor(PieceColor color) {
        return board.GetPiecesForColor(color);
    }

    public void Visualize() {
        Visualizer.Visualize(board);
    }

    public async Task DoMove(Move move) {
        var undoMoveInfo = board.ApplyMove(move);
        
        _moveHistory.Push((move, undoMoveInfo));

        if(NextMoveHandler != null) {
            await NextMoveHandler();
        }

        if(undoMoveInfo.CapturedColor != null || undoMoveInfo.MovedType == PieceType.Pawn) {
            _movesWithoutCapture = 0;
        } else {
            _movesWithoutCapture++;
        }

        Visualize();

        await NextTurn();
    }

    public UndoMoveInfo DoMoveForSimulation(Move move) {
        var undoInfo = board.ApplyMove(move);
        currentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        RecordPosition();
        return undoInfo;
    }

    public void UndoMoveForSimulation(UndoMoveInfo undoInfo) {
        UnrecordPosition();
        board.UndoMove(undoInfo);
        currentColor = currentColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
    }

    private async Task NextTurn() {
        currentColor = 
                currentColor == PieceColor.White 
                    ? PieceColor.Black 
                    : PieceColor.White;

        RecordPosition(); // Record position after color change
        ValidateIfGameIsOver();

        if(IsFinished()) {
            return;
        }

        await RunNextMoveIfAI();
    }

    private async Task RunNextMoveIfAI() {
        if(GetCurrentPlayer().IsAI() && !IsSimulated) {
            if (BeforeAIMoveHandler != null) {
                await BeforeAIMoveHandler();
            }
            
            await Task.Delay(_delayPerMoveInMilliseconds);

            var move = await GetCurrentPlayer().GetMove(this);
            await DoMove(move);
        }
    }

    private void ValidateIfGameIsOver() {
        if(_movesWithoutCapture >= MAX_MOVES_WITHOUT_CAPTURE) {
            Winner = null;
            IsGameActive = false;
            _gameEndReason = GameEndReason.FiftyMovesWithoutCapture;
            return;
        }
        
        if(IsThreefoldRepetition()) {
            Winner = null;
            IsGameActive = false;
            _gameEndReason = GameEndReason.ThreefoldRepetition;
            return;
        }

        if(!GetAllValidMovesForColor(currentColor).Any()) {
            if(IsChecked(currentColor)) {
                Winner = currentColor == PieceColor.White ? blackPlayer : whitePlayer;
                IsGameActive = false;
                _gameEndReason = GameEndReason.Checkmate;
            } else {
                Winner = null;
                IsGameActive = false;
                _gameEndReason = GameEndReason.Stalemate;
            }
        }
    }

    public bool IsValidMove(Move move) {
        return GetValidMovesForPosition(move.From).Any(m => m.To == move.To);
    }

    public IPlayer GetCurrentPlayer() {
        return currentColor == PieceColor.White ? whitePlayer : blackPlayer;
    }

    public List<Move> GetValidMovesForPosition(Position position) {
        var validMoves = GetAllValidMovesForColor(currentColor);
        return validMoves.Where(m => m.From == position).ToList();
    }

    public bool IsFinished() {
        return !IsGameActive;
    }

    public Board GetBoard() {
        throw new NotImplementedException();
    }

    public bool IsPawnPromotionMove(Move move) {
        return false;
    }

    public List<Move> GetAllValidMovesForColor(PieceColor color) {
        var pseudoLegal = board.GenerateMoves(color);
        var legal = new List<Move>(pseudoLegal.Count); // Pre-allocate based on pseudo-legal count

        foreach (var move in pseudoLegal) {
            var undoInfo = board.ApplyMove(move);
            
            if (!board.IsInCheck(color)) {
                legal.Add(move);
            }
            
            board.UndoMove(undoInfo);
        }
        
        return legal;
    }

    public string GetGameEndReason() {
        return _gameEndReason.GetDescription();
    }

    public bool IsDraw() {
        return _gameEndReason == GameEndReason.FiftyMovesWithoutCapture 
            || _gameEndReason == GameEndReason.Stalemate
            || _gameEndReason == GameEndReason.ThreefoldRepetition
            || IsThreefoldRepetition(); 
    }

    public List<Piece> GetCapturedPieces() {
        return _moveHistory
            .Where(m => m.undoMoveInfo.CapturedColor != null 
                    && m.undoMoveInfo.CapturedType != null)
            .Select(m => PieceExtensions.CreatePiece(
                m.undoMoveInfo.CapturedColor!.Value, 
                m.undoMoveInfo.CapturedType!.Value))
            .ToList();
    }

    public Piece? GetPieceAtPosition(Position position) {
        return board.GetPieceAtPosition(position);
    }

    public Move? GetLastMove() {

        if(_moveHistory.Count == 0) {
            return null;
        }

        return _moveHistory.Peek().move;
    }

    public void LoadForsythEdwardsNotation(string notation) {
        var parts = notation.Split(' ');
        var turn = parts.Length > 1 ? parts[1] : "w";

        currentColor = turn == "w" ? PieceColor.White : PieceColor.Black;
        
        board.LoadForsythEdwardsNotation(notation);
    }

    public IVisualizedBoard? GetVisualizedBoard() => board;

    public IGame Clone(bool simulated = false) {
        return new BitBoardGame(whitePlayer, blackPlayer, _delayPerMoveInMilliseconds) {
            board = (IVisualizedBoard)board.Clone(),
            currentColor = currentColor,
            _moveHistory = new Stack<(Move move, UndoMoveInfo undoMoveInfo)>(_moveHistory),
            _positionCounts = new Dictionary<ulong, int>(_positionCounts),
            Winner = Winner,
            NextMoveHandler = NextMoveHandler,
            Visualizer = Visualizer,
            IsSimulated = simulated,
            _gameEndReason = _gameEndReason,
        };
    }
}