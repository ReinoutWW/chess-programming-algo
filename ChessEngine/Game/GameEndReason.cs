namespace Chess.Programming.Ago.Game;

using System.ComponentModel;

public enum GameEndReason {
    [Description("None")]
    None,
    [Description("Checkmate")]
    Checkmate,
    [Description("Stalemate")]
    Stalemate,
    [Description("Insufficient material")]
    InsufficientMaterial,
    [Description("50 moves without capture")]
    FiftyMovesWithoutCapture,
    [Description("Draw by stalemate")]
    DrawByStalemate,
    [Description("Draw by insufficient material")]
    DrawByInsufficientMaterial,
}
