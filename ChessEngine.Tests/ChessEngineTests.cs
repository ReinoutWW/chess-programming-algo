namespace ChessEngine.Tests;

using global::Chess.Programming.Ago.Game;
using global::Chess.Programming.Ago.Core;
using global::Chess.Programming.Ago.Pieces;
using Xunit;
using Xunit.Abstractions;

public class ChessEngineTests
{
    private readonly ITestOutputHelper _output;

    public ChessEngineTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void HasCorrectInitialMoves() {
        var game = new Game(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.Start();

        var whiteMoves = game.GetAllValidMovesForColor(PieceColor.White);

        Assert.Equal(20, whiteMoves.Count);
    }

    /// <summary>
    /// Perft (Performance Test) - counts the number of leaf nodes at a given depth
    /// Uses make/undo pattern for BitBoardGame (much faster than cloning)
    /// </summary>
    private long Perft(BitBoardGame game, int depth)
    {
        if (depth == 0)
        {
            return 1;
        }

        long totalMoves = 0;
        var currentPlayer = game.GetCurrentPlayer();
        var moves = game.GetAllValidMovesForColor(currentPlayer.Color);

        foreach (var move in moves)
        {
            var undoInfo = game.DoMoveForSimulation(move);
            totalMoves += Perft(game, depth - 1);
            game.UndoMoveForSimulation(undoInfo);
        }

        return totalMoves;
    }

    /// <summary>
    /// Perft with detailed breakdown per move at the root level
    /// Uses make/undo pattern for BitBoardGame
    /// </summary>
    private Dictionary<string, long> PerftDivide(BitBoardGame game, int depth)
    {
        var results = new Dictionary<string, long>();
        var currentPlayer = game.GetCurrentPlayer();
        var moves = game.GetAllValidMovesForColor(currentPlayer.Color);

        foreach (var move in moves)
        {
            var undoInfo = game.DoMoveForSimulation(move);
            long count = depth > 1 ? Perft(game, depth - 1) : 1;
            game.UndoMoveForSimulation(undoInfo);
            
            string moveStr = $"({move.From.Row},{move.From.Column}) -> ({move.To.Row},{move.To.Column})";
            results[moveStr] = count;
        }

        return results;
    }

    [Theory]
    [InlineData(1, 20)]
    [InlineData(2, 400)]
    [InlineData(3, 8902)]
    [InlineData(4, 197281)]
    public void BitBoard_Perft_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));

        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    /// <summary>
    /// Position 2 - Kiwipete by Peter McKenzie
    /// FEN: r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -
    /// </summary>
    [Theory]
    [InlineData(1, 48)]
    [InlineData(2, 2039)]
    [InlineData(3, 97862)]
    [InlineData(4, 4085603)]
    public void BitBoard_Perft_Position2_Kiwipete_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq -";
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.LoadForsythEdwardsNotation(fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    /// <summary>
    /// Position 3
    /// FEN: 8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1
    /// </summary>
    [Theory]
    [InlineData(1, 14)]
    [InlineData(2, 191)]
    [InlineData(3, 2812)]
    [InlineData(4, 43238)]
    [InlineData(5, 674624)]
    public void BitBoard_Perft_Position3_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1";
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.LoadForsythEdwardsNotation(fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    /// <summary>
    /// Position 4
    /// FEN: r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1
    /// </summary>
    [Theory]
    [InlineData(1, 6)]
    [InlineData(2, 264)]
    [InlineData(3, 9467)]
    [InlineData(4, 422333)]
    public void BitBoard_Perft_Position4_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1";
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.LoadForsythEdwardsNotation(fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    /// <summary>
    /// Position 5 - Discussed on Talkchess, caught bugs in engines several years old
    /// FEN: rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8
    /// </summary>
    [Theory]
    [InlineData(1, 44)]
    [InlineData(2, 1486)]
    [InlineData(3, 62379)]
    [InlineData(4, 2103487)]
    public void BitBoard_Perft_Position5_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.LoadForsythEdwardsNotation(fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    /// <summary>
    /// Position 6 - Alternative Perft by Steven Edwards
    /// FEN: r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10
    /// </summary>
    [Theory]
    [InlineData(1, 46)]
    [InlineData(2, 2079)]
    [InlineData(3, 89890)]
    [InlineData(4, 3894594)]
    public void BitBoard_Perft_Position6_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10";
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.LoadForsythEdwardsNotation(fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    [Fact]
    public void BitBoard_PerftDivide_Depth1_ShowsAllInitialMoves()
    {
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));

        var results = PerftDivide(game, 1);
        
        _output.WriteLine("BitBoard Perft Divide at depth 1:");
        long total = 0;
        foreach (var kvp in results)
        {
            _output.WriteLine($"  {kvp.Key}: {kvp.Value}");
            total += kvp.Value;
        }
        _output.WriteLine($"Total: {total}");
        
        Assert.Equal(20, results.Count);
        Assert.Equal(20, total);
    }

    [Fact]
    public void BitBoard_HasCorrectInitialMoves() {
        var game = new BitBoardGame(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));

        var whiteMoves = game.GetAllValidMovesForColor(PieceColor.White);

        Assert.Equal(20, whiteMoves.Count);
    }
}
