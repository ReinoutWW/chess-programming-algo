namespace Chess.Programming.Ago.Game;

using System.Text;
using Chess.Programming.Ago.Core;
using Chess.Programming.Ago.Pieces;

public class GameVisualizer : IGameVisualizer {
    public void Visualize(Board board) {

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("    A  B  C  D  E  F  G  H");
        sb.AppendLine("   -------------------------");

        var pieces = board.GetPieces();
        
        for (int row = 0; row < pieces.GetLength(0); row++) {
            sb.Append($"{row + 1} |");

            for (int column = 0; column < pieces.GetLength(1); column++) {
                sb.Append(pieces[row, column] != null ? $" {GetPieceSymbol(pieces[row, column])} " : " ` ");
            }
            sb.AppendLine();
        }

        sb.AppendLine("   -------------------------");
        sb.AppendLine("    A  B  C  D  E  F  G  H");

        Console.WriteLine(sb.ToString());
    }

    private string GetPieceSymbol(Piece piece) {
        
        var symbol = piece.Type switch {
            PieceType.Pawn => "P",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Rook => "R",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => " "
        };

        return piece.Color == PieceColor.White 
            ? symbol.ToUpper() 
            : symbol.ToLower();
    }
}