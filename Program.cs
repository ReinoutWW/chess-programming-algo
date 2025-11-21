using System;
using ChessProgrammingAlgo.Core;
using ChessProgrammingAlgo.Players;

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
            
            Player white = null;
            Player black = null;

            switch (choice)
            {
                case "1":
                    white = new HumanPlayer(PieceColor.White);
                    black = new HumanPlayer(PieceColor.Black);
                    break;
                case "2":
                    white = new HumanPlayer(PieceColor.White);
                    black = new RandomPlayer(PieceColor.Black);
                    break;
                case "3":
                    white = new RandomPlayer(PieceColor.White);
                    black = new RandomPlayer(PieceColor.Black);
                    break;
                default:
                    Console.WriteLine("Invalid choice, defaulting to Human vs Random.");
                    white = new HumanPlayer(PieceColor.White);
                    black = new RandomPlayer(PieceColor.Black);
                    break;
            }

            Game game = new Game(white, black);
            game.Start();
        }
    }
}

