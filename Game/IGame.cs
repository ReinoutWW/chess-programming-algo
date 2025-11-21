namespace Chess.Programming.Ago.Game;

public interface IGame {
    IGameVisualizer Visualizer { get; }

    void Start();

    void Visualize();
}