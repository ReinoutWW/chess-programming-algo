using System;

namespace ChessProgrammingAlgo.Core
{
    public struct Position : IEquatable<Position>
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            return obj is Position pos && Equals(pos);
        }

        public bool Equals(Position other)
        {
            return Row == other.Row && Col == other.Col;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
    }
}

