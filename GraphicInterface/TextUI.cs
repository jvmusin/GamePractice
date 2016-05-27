using System.IO;
using System.Linq;
using Battleship.Interfaces;

namespace GraphicInterface
{
    public class TextUI
    {
        private readonly IGameController controller;
        private readonly TextWriter writer;

        public TextUI(IGameController controller, TextWriter writer)
        {
            this.controller = controller;
            this.writer = writer;
        }

        public void DrawCurrentState()
        {
            DrawHeader();
            writer.WriteLine();
            DrawPlayerState(controller.FirstPlayer);
            writer.WriteLine("\n------------------------\n");
        }

        public void DrawHeader()
        {
            writer.WriteLine("Turns first? " + controller.FirstPlayerTurns);
            writer.WriteLine("Game over?   " + controller.GameFinished);
        }

        public void DrawPlayerState(IPlayer player)
        {
            DrawField(player.SelfField, "Self field");
            writer.WriteLine();
            DrawField(player.OpponentFieldKnowledge, "Opponent field");
        }

        private void DrawField<T>(IRectangularReadonlyField<T> field, string name)
        {
            writer.WriteLine(name + ":");
            writer.WriteLine(EnumerateRowsAndColumns(field.ToString()));
        }

        private static string EnumerateRowsAndColumns(string field)
        {
            var rows = field.Split('\n').Select((row, i) => $"{i} {row}");
            var lastString = "  0123456789";
            return string.Join("\n", rows) + "\n" + lastString;
        }
    }
}
