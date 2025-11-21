using System;
using Chess.Programming.Ago.Game;
using Chess.Programming.Ago.Core;

namespace ChessProgrammingAlgo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to C# Chess!");
            Console.WriteLine("Select Game Mode:");
            Console.WriteLine("1. Human vs Human");
            Console.WriteLine("2. Human vs Random (Computer)");
            Console.WriteLine("3. Random vs Random");
            
            string choice = Console.ReadLine();
            
            switch (choice) {
                case "1":
                    Console.WriteLine("Human vs Human");
                    break;
                case "2":
                    Console.WriteLine("Human vs Random (Computer)");
                    break;
                case "3":
                    Console.WriteLine("Random vs Random");
                    break;
            }

            IGame game = new Game(new HumanPlayer(PieceColor.White), new HumanPlayer(PieceColor.Black));
            game.Start();
        }
    }
}