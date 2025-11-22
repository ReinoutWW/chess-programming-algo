using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Core;
using ChessConsole;

Console.WriteLine("Welcome to C# Chess!");

var gameActive = true;
IGame game = new Game(new HumanConsolePlayer(PieceColor.White), new HumanConsolePlayer(PieceColor.Black));

game.Start();

while (gameActive) {
    Move move;
    
    try{
        move = game.GetCurrentPlayer().GetMove(game);
        
        game.DoMove(move);
    } catch (Exception ex) {
        Console.WriteLine(ex.Message);
        continue;
    }

    if(game.IsFinished()) {

        var winner = game.Winner;

        if(winner == null) {
            Console.WriteLine("Game over! Draw!");
        } else {
            Console.WriteLine($"Game over! {winner.Color} wins!");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
        gameActive = false;
    }
}