using System;
using System.IO;
using Battleship.Implementations;
using Battleship.Interfaces;

namespace GraphicInterface
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random();

            var myField = new GameFieldBuilder().GenerateRandomField(random);
            var me = new Player(myField);

            var opponentField = new GameFieldBuilder().GenerateRandomField(random);
            var opponent = new Player(opponentField);

            var controller = new GameController(me, opponent);
            var gui = new TextUI(controller, Console.Out);

            Func<IPlayer, CellPosition> getTarget = player
                => player == me
                    ? ReadCellPositionFrom(Console.In)
                    : CellPosition.Random(controller.Rules.FieldSize);

            Run(controller, getTarget, gui);
        }

        public static void Run(IGameController controller, Func<IPlayer, CellPosition> getTarget, TextUI gui)
        {
            gui.DrawCurrentState();
            while (!controller.GameFinished)
            {
                var target = getTarget(controller.CurrentPlayer);
                controller.Shoot(target);
                gui.DrawCurrentState();
            }
        }

        private static CellPosition ReadCellPositionFrom(TextReader reader)
        {
            // ReSharper disable once PossibleNullReferenceException
            var input = reader.ReadLine().Split();
            var row = int.Parse(input[0]);
            var column = int.Parse(input[1]);
            return new CellPosition(row, column);
        }
    }
}
