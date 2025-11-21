using ChessProgrammingAlgo.Core;

namespace ChessProgrammingAlgo.Players
{
    public abstract class Player
    {
        public PieceColor Color { get; }

        protected Player(PieceColor color)
        {
            Color = color;
        }

        public abstract Move GetMove(Game game);
    }
}

