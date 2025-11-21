using ChessProgrammingAlgo.Pieces;

namespace ChessProgrammingAlgo.Core
{
    public class Move
    {
        public Position From { get; }
        public Position To { get; }
        public Piece PieceMoved { get; }
        public Piece PieceCaptured { get; }

        public Move(Position from, Position to, Piece pieceMoved, Piece pieceCaptured = null)
        {
            From = from;
            To = to;
            PieceMoved = pieceMoved;
            PieceCaptured = pieceCaptured;
        }
        
        public override string ToString()
        {
             return $"{PieceMoved.Color} {PieceMoved.Type} from {From} to {To}" + (PieceCaptured != null ? $" capturing {PieceCaptured.Type}" : "");
        }
    }
}

