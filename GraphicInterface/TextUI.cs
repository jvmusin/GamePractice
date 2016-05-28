using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Battleship.Interfaces;

namespace GraphicInterface
{
    public class TextUI : IGraphicUserInterface
    {
        private readonly IGameController controller;
        private readonly TextWriter writer;

        public TextUI(IGameController controller, TextWriter writer)
        {
            if (controller.Rules.FieldSize != new Size(10, 10))
                throw new ArgumentException("This UI works only with 10x10 fields");
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            this.controller = controller;
            this.writer = writer;
        }

        public void Update()
        {
            writer.WriteLine(
                PrepareHeader() + "\n" +
                "\n" +
                PreparePlayerState(controller.FirstPlayer) + 
                "\n" +
                "".PadRight(55, '-') +
                "\n");
        }

        private string PrepareHeader()
        {
            return $"Turns first? {controller.FirstPlayerTurns}\n" +
                   $"Game over?   {controller.GameFinished}";
        }

        private static string PreparePlayerState(IPlayer player)
        {
            var self = PrepareField(player.SelfField, "My field");
            var opponent = PrepareField(player.OpponentFieldKnowledge, "Opponent field");
            return MergeFields(self, opponent);
        }

        private static string MergeFields(string field1, string field2)
        {
            var firstField = field1.Split('\n').Select(x => x.PadRight(30));
            var secondField = field2.Split('\n');
            return string.Join("\n", firstField.Zip(secondField, (s1, s2) => s1 + s2));
        }

        private static string PrepareField<T>(IRectangularReadonlyField<T> field, string name)
        {
            return $"{name}:\n" +
                   $"{EnumerateRowsAndColumns(field.ToString())}";
        }

        private static string EnumerateRowsAndColumns(string field)
        {
            var rows = field.Split('\n').Select((row, i) => $"{i} {row}");
            var lastString = "  0123456789";
            return string.Join("\n", rows) + "\n" + lastString;
        }
    }
}
