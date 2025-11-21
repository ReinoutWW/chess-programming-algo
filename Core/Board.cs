using System;
using System.Collections.Generic;
using ChessProgrammingAlgo.Pieces;

namespace ChessProgrammingAlgo.Core
{
    public class Board
    {
        public Piece[,] Grid { get; private set; }
        public const int Size = 8;
        
        public List<Piece> WhitePieces { get; private set; } = new List<Piece>();
        public List<Piece> BlackPieces { get; private set; } = new List<Piece>();

        public Board()
        {
            Grid = new Piece[Size, Size];
        }

        public Piece GetPieceAt(Position pos)
        {
            if (!IsValidPosition(pos)) return null;
            return Grid[pos.Row, pos.Col];
        }

        public void SetPieceAt(Position pos, Piece piece)
        {
            if (IsValidPosition(pos))
            {
                // If overwriting a piece, remove it from lists (logic might be better in ApplyMove)
                Grid[pos.Row, pos.Col] = piece;
                if (piece != null)
                {
                    piece.Position = pos;
                }
            }
        }

        public void ApplyMove(Move move)
        {
            // Simple move application
            // Does not validate validity, assumes valid move passed
            var piece = Grid[move.From.Row, move.From.Col];
            Grid[move.From.Row, move.From.Col] = null;
            
            if (move.PieceCaptured != null)
            {
                 // Capture logic if we were tracking lists strictly
                 if (move.PieceCaptured.Color == PieceColor.White) WhitePieces.Remove(move.PieceCaptured);
                 else BlackPieces.Remove(move.PieceCaptured);
            }

            Grid[move.To.Row, move.To.Col] = piece;
            piece.Position = move.To;
            piece.HasMoved = true;
        }

        public bool IsValidPosition(Position pos)
        {
            return pos.Row >= 0 && pos.Row < Size && pos.Col >= 0 && pos.Col < Size;
        }

        public bool IsEmpty(Position pos)
        {
            return IsValidPosition(pos) && Grid[pos.Row, pos.Col] == null;
        }

        public void SetupStandardBoard()
        {
            Grid = new Piece[Size, Size];
            WhitePieces.Clear();
            BlackPieces.Clear();

            // White Setup
            SetupRow(0, PieceColor.White);
            SetupPawns(1, PieceColor.White);

            // Black Setup
            SetupRow(7, PieceColor.Black);
            SetupPawns(6, PieceColor.Black);
        }

        private void SetupRow(int row, PieceColor color)
        {
            AddPiece(new Rook(color), new Position(row, 0));
            AddPiece(new Knight(color), new Position(row, 1));
            AddPiece(new Bishop(color), new Position(row, 2));
            AddPiece(new Queen(color), new Position(row, 3));
            AddPiece(new King(color), new Position(row, 4));
            AddPiece(new Bishop(color), new Position(row, 5));
            AddPiece(new Knight(color), new Position(row, 6));
            AddPiece(new Rook(color), new Position(row, 7));
        }

        private void SetupPawns(int row, PieceColor color)
        {
            for (int col = 0; col < 8; col++)
            {
                AddPiece(new Pawn(color), new Position(row, col));
            }
        }

        private void AddPiece(Piece piece, Position pos)
        {
            SetPieceAt(pos, piece);
            if (piece.Color == PieceColor.White) WhitePieces.Add(piece);
            else BlackPieces.Add(piece);
        }
    }
}
