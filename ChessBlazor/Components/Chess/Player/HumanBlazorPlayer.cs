public class HumanBlazorPlayer(PieceColor color) : IPlayer {
    public PieceColor Color => color;
    public Move? SelectedMove { get; set; }

    public Move GetMove(IGame game) {
        return SelectedMove 
            ?? throw new InvalidOperationException("No move selected");
    }
}
