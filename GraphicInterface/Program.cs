using System;
using System.IO;
using Battleship.Implementations;
using Battleship.Interfaces;
using Ninject;
using Ninject.Parameters;

namespace GraphicInterface
{
    public class Program
    {
        private readonly IGameController controller;
        private readonly TextUI gui;

        public Program(IGameController controller, TextUI gui)
        {
            this.controller = controller;
            this.gui = gui;
        }

        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            kernel.Bind<IGameFieldBuilder>().To<GameFieldBuilder>();
            kernel.Bind<IGameField>().ToMethod(context => context.Kernel.Get<IGameFieldBuilder>().GenerateRandomField());
            kernel.Bind<IPlayer>().To<Player>();

            var me = kernel.Get<IPlayer>(new ConstructorArgument("nextTarget", (Func<CellPosition>)ReadCellPositionFromConsole));
            var opponent = kernel.Get<IPlayer>();
            
            kernel.Bind<IGameController>().ToConstant(new GameController(me, opponent));

            kernel.Bind<TextWriter>().ToConstant(Console.Out);

            kernel.Get<Program>().Run();
        }

        public void Run()
        {
            gui.DrawCurrentState();
            while (!controller.GameFinished)
            {
                var target = controller.CurrentPlayer.NextTarget;
                if (controller.Shoot(target) != null)
                    gui.DrawCurrentState();
            }
        }

        private static CellPosition ReadCellPositionFromConsole()
        {
            // ReSharper disable once PossibleNullReferenceException
            var input = Console.ReadLine().Split();
            var row = int.Parse(input[0]);
            var column = int.Parse(input[1]);
            return new CellPosition(row, column);
        }
    }
}
