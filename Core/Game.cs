using System;
using System.Collections.Generic;
using System.Linq;
using ChessProgrammingAlgo.Players;
using ChessProgrammingAlgo.Pieces;

namespace ChessProgrammingAlgo.Core
{
    public class Game
    {
        public Board Board { get; private set; }
        public Player WhitePlayer { get; private set; }
        public Player BlackPlayer { get; private set; }
        public PieceColor CurrentTurn { get; private set; }
        public bool IsGameOver { get; private set; }
        public string GameResult { get; private set; }

        public Game(Player white, Player black)
        {
            Board = new Board();
            Board.SetupStandardBoard();
            WhitePlayer = white;
            BlackPlayer = black;
            CurrentTurn = PieceColor.White;
        }

        public IEnumerable<Move> GetLegalMoves(Piece piece)
        {
            var moves = piece.GetValidMoves(Board);
            return moves.Where(IsLegalMove);
        }

        public void Start()
        {
            while (!IsGameOver)
            {
                PrintBoard();
                if (WhitePlayer is not HumanPlayer && BlackPlayer is not HumanPlayer)
                {
                    System.Threading.Thread.Sleep(500); // Delay for watchability
                }
                
                Player currentPlayer = 
                    CurrentTurn == PieceColor.White 
                        ? WhitePlayer 
                        : BlackPlayer;
                        
                Console.WriteLine($"{currentPlayer.Color}'s turn.");
                Move move = currentPlayer.GetMove(this);

                if (move == null)
                {
                    // No moves available
                    if (IsKingInCheck(CurrentTurn))
                    {
                        IsGameOver = true;
                        GameResult = $"{GetOpponentColor(CurrentTurn)} Wins by Checkmate!";
                    }
                    else
                    {
                        IsGameOver = true;
                        GameResult = "Draw by Stalemate!";
                    }
                    break;
                }

                // Double check validity (including check)
                if (IsLegalMove(move))
                {
                    Board.ApplyMove(move);
                    CurrentTurn = GetOpponentColor(CurrentTurn);
                }
                else
                {
                    Console.WriteLine("Illegal Move (Leaves King in Check or Invalid).");
                }
            }
            Console.WriteLine($"Game Over: {GameResult}");
        }

        public bool IsLegalMove(Move move)
        {
            // Simulate move
            Piece originalTarget = Board.Grid[move.To.Row, move.To.Col];
            Piece movingPiece = Board.Grid[move.From.Row, move.From.Col];
            Position originalPos = movingPiece.Position;

            // Apply temp
            Board.Grid[move.To.Row, move.To.Col] = movingPiece;
            Board.Grid[move.From.Row, move.From.Col] = null;
            movingPiece.Position = move.To;

            bool inCheck = IsKingInCheck(movingPiece.Color);

            // Revert
            Board.Grid[move.From.Row, move.From.Col] = movingPiece;
            movingPiece.Position = originalPos;
            Board.Grid[move.To.Row, move.To.Col] = originalTarget;

            return !inCheck;
        }

        public bool IsKingInCheck(PieceColor color)
        {
            // Find King
            var king = Board.Grid.Cast<Piece>().FirstOrDefault(p => p != null && p.Color == color && p.Type == PieceType.King);
            if (king == null) return true; // Should not happen

            // Check if any opponent piece attacks king's position
            var opponentColor = GetOpponentColor(color);
            foreach (var piece in Board.Grid)
            {
                if (piece != null && piece.Color == opponentColor)
                {
                    // Note: This is computationally expensive and infinite recursion risk if GetValidMoves checks IsKingInCheck
                    // We need GetValidMoves (Pseudo) vs GetLegalMoves.
                    // Piece.GetValidMoves currently returns Pseudo-legal moves (geometry only).
                    if (piece.GetValidMoves(Board).Any(m => m.To == king.Position))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private PieceColor GetOpponentColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        private void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine("  a b c d e f g h");
            for (int row = 7; row >= 0; row--)
            {
                Console.Write(row + 1 + " ");
                for (int col = 0; col < 8; col++)
                {
                    var piece = Board.Grid[row, col];
                    if (piece == null)
                    {
                        Console.Write(". ");
                    }
                    else
                    {
                        Console.Write(GetPieceSymbol(piece) + " ");
                    }
                }
                Console.WriteLine(row + 1);
            }
            Console.WriteLine("  a b c d e f g h");
        }

        private string GetPieceSymbol(Piece piece)
        {
            // Simple text representation
            string symbol = piece.Type.ToString().Substring(0, 1);
            if (piece.Type == PieceType.Knight) symbol = "N"; // K is King
            return piece.Color == PieceColor.White ? symbol.ToUpper() : symbol.ToLower();
        }
    }
}

