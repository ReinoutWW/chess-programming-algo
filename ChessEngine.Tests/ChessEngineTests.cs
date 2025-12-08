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

    [Theory]
    [InlineData(1, 44)]
    [InlineData(2, 1486)]
    [InlineData(3, 62379)]
    [InlineData(4, 2103487)]
    public void BitBoard_Perft_Position5ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        const string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
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
