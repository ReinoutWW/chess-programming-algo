namespace Chess.Programming.Ago.ChessEngines;

using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Game;

/// <summary>
/// 
/// </summary>
/// <param name="color"></param>
public class MinMaxPlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public bool IsAI() => true;
    public async Task<Move> GetMove(IGame game) {
        throw new NotImplementedException();
    }
}