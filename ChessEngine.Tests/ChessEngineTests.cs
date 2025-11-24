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
    /// </summary>
    /// <param name="game">The game state</param>
    /// <param name="depth">The depth to search</param>
    /// <returns>Total number of positions at the given depth</returns>
    private long Perft(IGame game, int depth)
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
            // Clone the game to avoid modifying the original
            var clonedGame = game.Clone(simulated: true);
            
            // Make the move
            clonedGame.DoMove(move).Wait();
            
            // Recurse to the next depth
            totalMoves += Perft(clonedGame, depth - 1);
        }

        return totalMoves;
    }

    /// <summary>
    /// Perft with detailed breakdown per move at the root level
    /// </summary>
    private Dictionary<string, long> PerftDivide(IGame game, int depth)
    {
        var results = new Dictionary<string, long>();
        var currentPlayer = game.GetCurrentPlayer();
        var moves = game.GetAllValidMovesForColor(currentPlayer.Color);

        foreach (var move in moves)
        {
            var clonedGame = game.Clone(simulated: true);
            clonedGame.DoMove(move).Wait();
            
            long count = depth > 1 ? Perft(clonedGame, depth - 1) : 1;
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
    public void Perft_ShouldReturnCorrectMoves(int depth, long expectedMoves)
    {
        var game = new Game(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.Start();

        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves);
    }

    [Theory]
    [InlineData(1, 44)]
    [InlineData(2, 1486)]
    [InlineData(3, 62379)]
    [InlineData(4, 89941194)]
    public void Perft_Position5ShouldReturnCorrectMoves(int depth, int expectedMoves)
    {
        const string fen = "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8";
        var game = new Game(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black), 0, fen);
        
        long totalMoves = Perft(game, depth);
        
        Assert.Equal(expectedMoves, totalMoves); // Known perft(3) for this position
    }

    [Fact]
    public void PerftDivide_Depth1_ShowsAllInitialMoves()
    {
        var game = new Game(new DummyPlayer(PieceColor.White), new DummyPlayer(PieceColor.Black));
        game.Start();

        var results = PerftDivide(game, 1);
        
        _output.WriteLine("Perft Divide at depth 1:");
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
}
